using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FleeState : GroundState
{
    public FSMTank tank;


    public FleeState(FSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.fleeState = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.fleeState = false;
        return null;
    }

    public override Type UpdateState()
    {
        //tank not found, tank not in range...
        if(tank.targetFound == false && tank.targetInRange == false)
        {
            //...and low on ammo OR fuel OR health = explore consumable.
            if (tank.critAmmo == true || tank.critFuel == true || tank.critHealth == true)
            {
                return typeof(ExploreConsumable);
            }
            //... levels are fine = explore.
            else
            {
                return typeof(ExploreState);
            }
        }
        //tank found, has good health AND ammo...
        else if(tank.targetFound == true && (tank.critHealth == false && tank.critAmmo == false))
        {
            //...and tank is in range = attack tank.
            if(tank.targetInRange == true)
            {
                return typeof(AttackState);
            }
            //...and tank is not in range = pursue tank.
            else
            {
                return typeof(PursueState);
            }
        }
        //tank not found, tank not in range, ammo is good, base is found...
        else if(tank.targetFound == false && tank.targetInRange == false &&
            tank.critAmmo == false && tank.baseFound == true)
        {
            //...and base is in range = attack base.
            if (tank.baseInRange == true)
            {
                return typeof(AttackBases);
            }
            //...and base is not in range = pursue base.
            else
            {
                return typeof(PursueBase);
            }
        }
        //otherwise keep fleeing.
        else
        {
            tank.FleeTarget();
            return null;
        }
    }
}
