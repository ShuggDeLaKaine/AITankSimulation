using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FSMTank : AITank
{
    //    
    public Dictionary<GameObject, float> targDict = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> consDict = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> baseDict = new Dictionary<GameObject, float>();

    public GameObject currentTarget;
    public GameObject currentConsumable;
    public GameObject currentBase;

    //creating a Dictionary to contain all the states
    Dictionary<Type, GroundState> states = new Dictionary<Type, GroundState>();

    //bools and floats for critical levels of health, ammo or fuel.
    public bool critHealth;
    private float critHealthLevel;

    public bool critAmmo;
    private int critAmmoLevel;

    public bool critFuel;
    private float critFuelLevel;

    //bools for whether the target is found and in range.
    public bool targetFound;
    public bool targetInRange;
    public bool baseFound;
    public bool baseInRange;
    public bool consumableFound;
    
    //floats for ranges and time counter. 
    public float inRange = 20.0f;
    public float tooClose = 5.0f;
    public bool stopRange;
    private float time = 0.0f;

    //these just for the checker, to be shown in the inspector. To be removed
    public bool exploreState = false;
    public bool exploreConsumables = false;
    public bool pursueState = false;
    public bool pursueBase = false;
    public bool attackState = false;
    public bool attackBases = false;
    public bool fleeState = false;


    public override void AITankStart()
    {
        //calling func to initalise all states and get FSM script component.
        InitFSM();
        //check whether target found, in range or too close.
        AllTheChecks();
        //update the dictionaries containing lists of found targets, bases and consumerables.
        TargetsFoundUpdate();
    }

    public override void AITankUpdate()
    {
        //check whether target found, in range or too close.
        AllTheChecks();
        //update the dictionaries containing lists of found targets, bases and consumerables.
        TargetsFoundUpdate();
    }


    //function to initialise the states of the desired state machine and add them to the FSM script SetStates function.
    private void InitFSM()
    {
        //adding the states and adding to Dictionary 'states'
        states.Add(typeof(ExploreState), new ExploreState(this));
        states.Add(typeof(ExploreConsumable), new ExploreConsumable(this));
        states.Add(typeof(PursueState), new PursueState(this));
        states.Add(typeof(PursueBase), new PursueBase(this));
        states.Add(typeof(AttackState), new AttackState(this));
        states.Add(typeof(AttackBases), new AttackBases(this));
        states.Add(typeof(FleeState), new FleeState(this));

        //get module component 'FSM' script and using it's SetStates function to add the above Dictionary 'states' to.
        GetComponent<FSM>().SetStates(states);
    }


    //function to setup the bools for the ammo, fuel and health meter levels.
    void InitMeters()
    {
        //setting critical health & ammo level to 20% of their maxs. Critical Ammo set to 0. 
        critHealthLevel = ((healthMax / 100) * 30);
        critFuelLevel = ((fuelMax / 100) * 30);
        critAmmoLevel = 2;
        //calling function to intialise the bools set to each of these levels.
        MeterLevelsCheck();
    }


    //function to randomly pick points in world and move towards them.
    public void ExploreForTargets()
    {
        //Move to random point at, parameter multiplier for tank speed, just keep to standard hence * 1.0f.
        FollowPathToRandomPoint(1.0f);

        //moving to target point; if there's no interaction, a countdown, then pick new point to move to and reset timer.
        time += Time.deltaTime;
        if (time > 10.0f) {
            FindAnotherRandomPoint();
            time = 0.0f;
        }
    }

    //function to have tank pursue other tank
    public void PursueTarget()
    {
        if(currentTarget != null) { 
            if(stopRange == false) { 
                //move to point occupied by current target at a speed multiplier of 1.0f.
                FollowPathToPointInWorld(currentTarget, 1.0f);
            }
        }
        //keep checking that other tank still found and whether they are in range yet.
        AllTheChecks();
    }

    public void MoveToBase()
    {
        if(currentBase != null) { 
            //check that there is no tank found or in range AND that a base has been found.
            if ((targetFound == false && targetInRange == false) && baseFound == true) {
                //check whether base is in firing range.
                if (baseInRange == false) {
                    //move towards base.
                    FollowPathToPointInWorld(currentBase, 1.0f);
                }
            }    
        }
        //check the sensors for updates.
        AllTheChecks();
    }

    public void MoveToConsumable()
    {
        //check that there is something on the consumable list and check first entry is not null; therefore something there.
        if (consDict.Count > 0 && consDict.First().Key != null) { 
            //go to location of this consumable.
            FollowPathToPointInWorld(currentConsumable, 1.0f);
        }
        //otherwise no consumable in the dictionary.
        else {
            //so pick another random point and go there until a consumable is added to dictionary.
            ExploreForTargets();
        }
    }

    //function to attack the current target.
    public void AttackTarget()
    {
        if(currentTarget != null) { 
            //fire at the position that the current target is in.
            FireAtPointInWorld(currentTarget);
        }
        //keep checking that other tank still found and still in range yet.
        AllTheChecks();
    }

    //function to attack the current base
    public void AttackBase()
    {
        if(baseDict.Count > 0) { 
            if(currentBase != null) { 
            //check that there is no tank found or in range AND that a base has been found.
                if((targetFound == false && targetInRange == false) && baseFound == true) {
                    //check whether base is in firing range.
                    if(baseInRange == true) { 
                        //fire at base.
                        FireAtPointInWorld(currentBase);
                        currentBase = null;
                        baseDict.Clear();
                    }
                }
            }
            //check the sensors for updates.
            AllTheChecks();
        }
    }

    //function to flee away from the current target.
    public void FleeTarget()
    {
        ExploreForTargets();
        AllTheChecks();
    }


    //function to fill and update Dictionaries targets, consumables and bases.
    void TargetsFoundUpdate()
    {
        //Using the functions in the sensor view in AITank script.
        //Uses Dictionarys targetsFound, basesFound, consumablesFound (plus their Get functions) and FindVisibleTargets()
        targDict = GetTargetsFound;
        consDict = GetConsumablesFound;
        baseDict = GetBasesFound;
    }


    //function to check whether a target has been found by the the sensors in AITank.
    public void CheckTargetFound()
    {
        //check that there are targets in the list, ie count of list is more than 0.
        //check that the key of the first item on the list is not null, so therefore there definitely is something there.
        if (targDict.Count > 0 && targDict.First().Key != null) {
            //set current target to the first target in the dictionary targets.
            currentTarget = targDict.First().Key;
            //check that current target has a value, ie assigned to something.
            if (currentTarget != null) {
                //then targetFound bool set to true.
                targetFound = true;
            } else {
                //otherwise targetFound is false.
                targetFound = false;
            }
        }

        //same as above but for target bases.
        if (baseDict.Count > 0 && baseDict.First().Key != null) {
            //set currentBase to the first target in the dictionary bases.
            currentBase = baseDict.First().Key;
            //check that currentBase has a value, ie assigned to something.
            if (currentBase != null) {
                //then baseFound bool set to true.
                baseFound = true;
            } else {
                //otherwise baseFound is false.
                baseFound = false;
            }
        } else {
            currentBase = null;
        }

        //same as above but for target consumables.
        if (consDict.Count > 0 && consDict.First().Key != null) {
            //set currentConsumable to the first target in the dictionary consumables.
            currentConsumable = consDict.First().Key;
            //check that currentConsumable has a value, ie assigned to something.
            if (currentConsumable != null) {
                //then consumableFound bool set to true.
                consumableFound = true;
            } else {
                //otherwise consumableFound is false.
                consumableFound = false;
            }
        }
    }


    //function to check whether target is in range.
    public void CheckTargetInRange()
    {
        //do the following only if the currentTarget has a value, otherwise calls it in next bit and kicks an ERROR
        if(currentTarget != null) {
            //check vector between tank and other tank is less than fireRange
            if (Vector3.Distance(transform.position, currentTarget.transform.position) < inRange) {
                targetInRange = true;
            } else {
                targetInRange = false;
            }
        }

        //same as above but for bases.
        if (currentBase != null) {
            //check vector between tank and base is less than firing range
            if (Vector3.Distance(transform.position, currentBase.transform.position) < inRange) {
                baseInRange = true;
            } else {
                baseInRange = false;
            }
        }
    }


    //function to stop tank getting too close to other tank
    public void CheckTooClose()
    {
        //do the following only if the currentTarget has a value, otherwise calls it in next bit and kicks an ERROR
        if (currentTarget != null) {
            //checking whether the tank is too close to the other tank
            if (Vector3.Distance(transform.position, currentTarget.transform.position) < tooClose) {
                stopRange = true;
            } else {
                stopRange = false;
            }
        }
    }

    public void AllTheChecks()
    {
        CheckTargetFound();
        CheckTargetInRange();
        CheckTooClose();
        MeterLevelsCheck();
    }


    //Func to be called on updates to check the levels of ammo, fuel and health. Flicking between bools as necessary.
    void MeterLevelsCheck()
    {
        //checking current health against critical health level; if below then bool critHealth set to true, else it's false.
        if (health <= critHealthLevel){
            critHealth = true;
        } else {
            critHealth = false;
        }
        //checking whether fuel at critical level.
        if (fuel <= critFuelLevel) {
            critFuel = true;
        } else {
            critFuel = false;
        }
        //checking whether ammo at critical level.
        if (ammo <= critAmmoLevel) {
            critAmmo = true;
        } else {
            critAmmo = false;
        }
    }


    public override void AIOnCollisionEnter(Collision collision) { }
}
