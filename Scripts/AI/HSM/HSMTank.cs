using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class HSMTank : AITank
{
    //list of dictionarys to hold the targets, bases, consumables and specific consumables; the game object of each as well as the distance.    
    public Dictionary<GameObject, float> targDict = new Dictionary<GameObject, float>();  
    public Dictionary<GameObject, float> baseDict = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> consDict = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> ammoDict = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> fuelDict = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> healthDict = new Dictionary<GameObject, float>();
    
    //game objects for tank and base.
    public GameObject currentTarget;
    public GameObject currentBase;
    //public GameObject currentConsumable;

    //game objects for each ammo, fuel and health.
    public GameObject ammoCon;
    public GameObject fuelCon;
    public GameObject healthCon;

    //creating a Dictionary to contain all the states.
    Dictionary<Type, HSMBaseState> states = new Dictionary<Type, HSMBaseState>();

    //bools and floats for critical levels of health, ammo or fuel.
    public bool critHealth;
    private float critHealthLevel;
    public bool goodHealth;
    private float goodHealthLevel;
    public bool okHealth;

    public bool critAmmo;
    private int critAmmoLevel;
    public bool goodAmmo;
    private float goodAmmoLevel;
    public bool okAmmo;

    public bool critFuel;
    private float critFuelLevel;
    public bool goodFuel;
    private float goodFuelLevel;
    public bool okFuel;

    public bool anyCritLevel;

    //floats to be used in the movement functions second part of params, instead of 1.0f 'magic number'
    public float fastSpeed = 1.0f;
    public float balancedSpeed = 0.8f;
    public float cautiousSpeed = 0.6f;
 
    //bools similar to above but for HSM_Tank specific. 
    public bool anyTarget;
    public bool anyTargetInRange;
    public bool tankTarget;
    public bool tankTargetInRange;
    public bool baseTarget;
    public bool baseTargetInRange;

    //bools for the consumables.
    public bool consumableFound;
    public bool ammoFound;
    public bool fuelFound;
    public bool healthFound;

    //floats for ranges and time counter. 
    public float inRange = 20.0f;
    public float tooClose = 5.0f;
    public bool stopRange;
    private float time = 0.0f;
    public bool tankIsFiring;

    //these just for the checker, to be shown in the inspector. 
    public bool exploreMain = false;
    public bool exploreFast = false;
    public bool exploreBalanced = false;
    public bool exploreConsumables = false;
    public bool exploreConsume = false;
    public bool findingAmmo = false;
    public bool findingFuel = false;
    public bool findingHealth = false;

    public bool pursueMain = false;
    public bool pursueTank = false;
    public bool pursueTankFast = false;
    public bool pursueTankBalanced = false;
    public bool pursueBase = false;
    public bool pursueBaseBalanced = false;
    public bool pursueBaseCautious = false;

    public bool attackMain = false;
    public bool attackTank = false;
    public bool attackAndMove = false;
    public bool attackTankBalanced = false;
    public bool attackBase = false;

    public bool fleeMain = false;
    public bool fleeFast = false;
    public bool fleeBalanced = false;
    public bool firingRetreat = false;
    //ABOVE BOOLS ARE JUST TO CHECK IN INSPECTOR THAT STATE TRANSITIONS ARE WORKING CORRECTLY

    public override void AITankStart()
    {
        InitHSM();          //calling func to initalise all states and get FSM script component.
        AllTheChecks();     //check whether target found, in range or too close.
        TargetsFoundUpdate();   //update the dictionaries containing lists of found targets, bases and consumerables.
    }

    public override void AITankUpdate()
    {
        AllTheChecks();         //check whether target found, in range or too close.
        TargetsFoundUpdate();   //update the dictionaries containing lists of found targets, bases and consumerables.
    }

    //function to initialise the states of the desired state machine and add them to the FSM script SetStates function.
    private void InitHSM()
    {
        //adding the states and adding to Dictionary 'states'
        //adding Explore main and sub states
        states.Add(typeof(ExploreMain), new ExploreMain(this));
        states.Add(typeof(ExploreFast), new ExploreFast(this));
        states.Add(typeof(ExploreBalanced), new ExploreBalanced(this));
        states.Add(typeof(ExploreConsume), new ExploreConsume(this));
        states.Add(typeof(FindAmmo), new FindAmmo(this));
        states.Add(typeof(FindFuel), new FindFuel(this));
        states.Add(typeof(FindHealth), new FindHealth(this));

        //adding Pursue main and sub states
        states.Add(typeof(PursueMain), new PursueMain(this));
        states.Add(typeof(PursueSubTank), new PursueSubTank(this));
        states.Add(typeof(PursueTankFast), new PursueTankFast(this));
        states.Add(typeof(PursueTankBalanced), new PursueTankBalanced(this));
        states.Add(typeof(PursueSubBase), new PursueSubBase(this));
        states.Add(typeof(PursueBaseBalanced), new PursueBaseBalanced(this));
        states.Add(typeof(PursueBaseCautious), new PursueBaseCautious(this));

        //adding Attack main and sub states
        states.Add(typeof(AttackMain), new AttackMain(this));
        states.Add(typeof(AttackTank), new AttackTank(this));
        states.Add(typeof(AttackTankBalanced), new AttackTankBalanced(this));
        states.Add(typeof(AttackAndMove), new AttackAndMove(this));
        states.Add(typeof(AttackBase), new AttackBase(this));

        //adding Flee main and sub states
        states.Add(typeof(FleeMain), new FleeMain(this));
        states.Add(typeof(FleeFast), new FleeFast(this));
        states.Add(typeof(FleeBalanced), new FleeBalanced(this));

        //get module component 'HSM' script and using it's SetStates function to add the above Dictionary 'states' to.
        GetComponent<HSM>().SetStates(states);
    }

    //function to setup the bools for the ammo, fuel and health meter levels.
    void InitMeters()
    {
        //setting critical health & ammo level to 20% of their maxs. Critical Ammo set to 0. 
        critHealthLevel = ((healthMax / 100) * 40);
        critFuelLevel = ((fuelMax / 100) * 40);
        critAmmoLevel = 1;

        //setting good levels for health, ammo and fuel; anything above 60% of max.
        goodHealthLevel = ((healthMax / 100) * 65);
        goodFuelLevel = ((healthMax / 100) * 65);
        goodAmmoLevel = 6;

        //calling function to intialise the bools set to each of these levels.
        MeterLevelsCheck();
    }

    //function for to explore, including speed and wait changes.
    public void ExploreForTargets(float speed, float wait)
    {
        //Move to random point at, parameter multiplier for tank speed, just keep to standard hence * 1.0f.
        FollowPathToRandomPoint(speed);
        //moving to target point; if there's no interaction, a countdown, then pick new point to move to and reset timer.
        time += Time.deltaTime;
        if (time > wait) {
            FindAnotherRandomPoint();
            time = 0.0f;
        }
    }

    //function to pursue the targeted tank.
    public void PursueTargetHSM(float speed)
    {
        if(currentTarget != null) { 
            if(tankTarget == true) { 
                if(stopRange == false) { 
                //FaceTurretToPointInWorld(currentTarget.transform.position);
                FollowPathToPointInWorld(currentTarget, speed);     //move to point occupied by current target at a speed multiplier of 1.0f.
                } else {
                    FaceTurretToPointInWorld(currentTarget.transform.position);
                    FollowPathToPointInWorld(currentTarget, 0.0f);
                }
            }
        }
        AllTheChecks();     //keep checking that other tank still found and whether they are in range yet.
    }

    //function to flee from a target.
    public void FleeTargetHSM(float speed, float wait)
    {
        ExploreForTargets(speed, wait);
        AllTheChecks();
    }

    //function to move to targeted base.
    public void MoveToBaseHSM(float speed)
    {
        if(currentBase != null) { 
            //check that there is no tank found or in range AND that a base has been found.
            if ((tankTarget == false && tankTargetInRange == false) && baseTarget == true) {
                //check whether base is in firing range.
                if (baseTargetInRange == false) {  
                    FollowPathToPointInWorld(currentBase, speed);       //move towards base.
                    FaceTurretToPointInWorld(currentBase.transform.position);
                }
            }
        }
        AllTheChecks();     //check the sensors for updates.
    }
    
    //same as above but for HSM Tank with choice of which consumable to move to
    public void MoveToConsumableHSM(Dictionary<GameObject, float> dicType, GameObject consumeType, float speed)
    {
        //check relevant dictionary, see if something is on it and first element isn't null.
        if (dicType.Count > 0 && dicType.First().Key != null) {
            FollowPathToPointInWorld(consumeType, speed);      //go to location of required type of consumable.
            if(this.transform.position == consumeType.transform.position) {
                if(consumeType == ammoCon) {
                    ammoFound = false;
                } else if (consumeType == fuelCon) {
                    fuelFound = false;
                } else if (consumeType == healthCon) {
                    healthFound = false;
                }
            }
        } else {
            //otherwise no consumable in the dictionary, so pick another random point and go there until a consumable is added to dictionary.
            ExploreForTargets(speed, 10.0f);
        }
    }

    //function to attack the current target.
    public void AttackTarget()
    {
        if (currentTarget != null) {
            FireAtPointInWorld(currentTarget);          //fire at the position that the current target is in.
        }
        AllTheChecks();         //keep checking that other tank still found and still in range yet.
    }

    //function for smarter attack, fire, move for 1.5 secs (reload is 2 secs) and repeat.
    public void AttackAndMove(float speed, float wait)
    {
        if(currentTarget != null)
        {
            if(tankIsFiring == false) {
                FireAtPointInWorld(currentTarget);  //fire at the current target.
            } else {
                FollowPathToRandomPoint(speed);
                //ExploreForTargets(speed, wait);
            }
        }
        AllTheChecks();
    }

    //function to attack the current base
    public void AttackBase()
    {
        if(baseDict.Count > 0) { 
            if (currentBase != null) {
                //check that there is no tank found or in range AND that a base has been found.
                if ((tankTarget == false && tankTargetInRange == false) && baseTarget == true) {
                    //check whether base is in firing range.
                    if (baseTargetInRange == true) { 
                        FireAtPointInWorld(currentBase);        //fire at base.
                        // clear the dictionary list so the destroyed bases is no longer targeted.
                        currentBase = null;
                        baseDict.Clear();
                    }
                }
            }
            AllTheChecks();         //check the sensors for updates.
        } 
    }

    //function to fill and update Dictionaries targets, consumables and bases.
    void TargetsFoundUpdate()
    {
        //Uses Dictionarys targetsFound, basesFound, consumablesFound (plus their Get functions) and FindVisibleTargets()
        targDict = GetTargetsFound;
        consDict = GetConsumablesFound;
        baseDict = GetBasesFound;
        ammoDict = GetAmmoFound;
        fuelDict = GetFuelFound;
        healthDict = GetHealthFound;
        tankIsFiring = IsFiring;
    }

    //get all these checks into one function to be called at start() and update().
    public void AllTheChecks()
    {
        CheckTargetFound();
        CheckConsumeTypesFound();
        CheckTargetInRange();
        CheckTooClose();
        MeterLevelsCheck();
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
                tankTarget = true;
            } else {
                tankTarget = false; 
            }
        } /*else {
            currentTarget = null;
        }*/

        //same as above but for target bases.
        if (baseDict.Count > 0 && baseDict.First().Key != null) {
            //set currentBase to the first target in the dictionary bases.
            currentBase = baseDict.First().Key;
            //check that currentBase has a value, ie assigned to something.
            if (currentBase != null) {
                baseTarget = true;
            } else {
                baseTarget = false;
            }
        } else {
            currentBase = null;
        }

        //check to be used in HSM_Tank, checks whether a base or tank has been found.
        if (tankTarget == true || baseTarget == true) {
            anyTarget = true;
        } else {
            anyTarget = false;
        }
    }

    //function to check whether target is in range.
    public void CheckTargetInRange()
    {
        //do the following only if the currentTarget has a value, otherwise calls it in next bit and kicks an ERROR
        if (currentTarget != null) {
            //check vector between tank and other tank is less than fireRange
            if (Vector3.Distance(transform.position, currentTarget.transform.position) < inRange) {
                tankTargetInRange = true;
            } else {
                tankTargetInRange = false;
            }
        } else {
            tankTargetInRange = false;
        }

        //same as above but for bases.
        if (currentBase != null) {
            //check vector between tank and base is less than firing range
            if (Vector3.Distance(transform.position, currentBase.transform.position) < inRange) {
                baseTargetInRange = true;
            } else {
                baseTargetInRange = false;
            }
        } else {
            baseTargetInRange = false;
        }

        //check to be used in HSM_Tank, checks whether a base or tank has been found.
        if (tankTargetInRange == true || baseTargetInRange == true) {
            anyTargetInRange = true;
        } else {
            anyTargetInRange = false;
        }
    }

    public void CheckConsumeTypesFound()
    {
        //checking for ammo consumable finds and setting to relevant game object.
        if (ammoDict.Count > 0 && ammoDict.First().Key != null) {
            ammoCon = ammoDict.First().Key;
            if (ammoCon != null) {
                ammoFound = true;
            } else {
                ammoFound = false;
            }
        } else {
            ammoCon = null;
        }
        //same as above but for fuel
        if (fuelDict.Count > 0 && fuelDict.First().Key != null) {
            fuelCon = fuelDict.First().Key;
            if (fuelCon != null) {
                fuelFound = true;
            } else {
                fuelFound = false;
            }
        } else {
            fuelCon = null;
        }
        //same as above but for health
        if (healthDict.Count > 0 && healthDict.First().Key != null) {
            healthCon = healthDict.First().Key;
            if (healthCon != null) {
                healthFound = true;
            } else {
                healthFound = false;
            }
        } else {
            healthCon = null;
        }
    }

    //function to stop tank getting too close to other tank
    public void CheckTooClose()
    {
        //do the following only if the currentTarget has a value, otherwise calls it in next bit and kicks an ERROR
        if (currentTarget != null) {
            //checking whether the tank is too close to the other tank
            if (Vector3.Distance(transform.position, currentTarget.transform.position) < tooClose)
            {
                stopRange = true;
            }
            else
            {
                stopRange = false;
            }
        }
    }

    //Func to be called on updates to check the levels of ammo, fuel and health. Flicking between bools as necessary.
    void MeterLevelsCheck()
    {
        //checking current HEALTH against CRITICAL health level; if below then bool critHealth set to true, else it's false.
        if (health <= critHealthLevel) {
            critHealth = true;
        } else {
            critHealth = false;
        }
        //checking whether FUEL at CRITICAL level.
        if (fuel <= critFuelLevel) {
            critFuel = true;
        } else {
            critFuel = false;
        }
        //checking whether AMMO at CRITICAL level.
        if (ammo <= critAmmoLevel) {
            critAmmo = true;
        } else {
            critAmmo = false;
        }

        //checking whether HEALTH at GOOD level.
        if (health >= goodHealthLevel) {
            goodHealth = true;
        } else {
            goodHealth = false;
        }
        //checking whether FUEL at GOOD level.
        if (fuel >= goodFuelLevel) {
            goodFuel = true;
        } else {
            goodFuel = false;
        }
        //checking whether AMMO at GOOD level.
        if (ammo >= goodFuelLevel) {
            goodAmmo = true;
        } else {
            goodAmmo = false;
        }

        //checking whether HEALTH at OK level.
        if (!critHealth && !goodHealth) {
            okHealth = true;
        } else {
            okHealth = false;
        }
        //checking whether AMMO at OK level.
        if (!critAmmo && !goodAmmo) {
            okAmmo = true;
        } else {
            okAmmo = false;
        }
        //checking whether FUEL at OK level.
        if (!critFuel && !goodFuel) {
            okFuel = true;
        } else {
            okFuel = false;
        }

        //check if ammo, fuel or health are at a critical level.
        if (critAmmo == true || critFuel == true || critHealth == true){
            anyCritLevel = true;
        } else {
            anyCritLevel = false;
        }
    }

    public override void AIOnCollisionEnter(Collision collision) { }
}
