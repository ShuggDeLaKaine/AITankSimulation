using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExploreState : GroundState
{
    public FSMTank tank;


    public ExploreState(FSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.exploreState = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.exploreState = false;
        return null;
    }

    public override Type UpdateState()
    {
        //tank found, has health, has ammo...
        if (tank.targetFound == true && tank.critHealth == false && tank.critAmmo == false)
        {
            //...and tank is in range = attack tank.
            if (tank.targetInRange == true)
            {
                return typeof(AttackState);
            }
            //...else tank is not in range = persue tank.
            else
            {
                return typeof(PursueState);
            }
        }
        //no tank found, no tank in range, base found, has ammo...
        else if (tank.targetFound == false && tank.targetInRange == false &&
            tank.baseFound == true && tank.critAmmo == false)
        {
            //...and base is in range = attack base.
            if (tank.baseInRange == true)
            {
                return typeof(AttackBases);
            }
            //...else base is not in range = persue base.
            else
            {
                return typeof(PursueBase);
            }
        }
        //tank found OR tank in range, low health OR no ammo = flee.
        else if ((tank.targetFound == true || tank.targetInRange == true) &&
            (tank.critHealth == true || tank.critAmmo == true))
        {
            return typeof(FleeState);
        }
        //no tank found, low on health or fuel or ammo = find health, fuel or ammo.
        else if (tank.targetFound == false &&
            (tank.critHealth == true || tank.critAmmo == true || tank.critFuel == true))
        {
            return typeof(ExploreConsumable);
        }
        //else carry on exploring.
        else
        {
            tank.ExploreForTargets();
            return null;
        }
    }
}
