using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExploreConsumable : GroundState
{
    public FSMTank tank;


    public ExploreConsumable(FSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.exploreConsumables = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.exploreConsumables = false;
        return null;
    }

    public override Type UpdateState()
    {
        //tank found OR in range, low health OR low ammo = flee
        if((tank.targetFound == true || tank.targetInRange == true) && 
            (tank.critHealth == false || tank.critAmmo == false))
        {
            return typeof(FleeState);
        }
        //tank found, good health AND ammo...
        else if (tank.targetFound == true && 
            (tank.critHealth == false && tank.critAmmo == false))
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
        //base in range, no tank found, good health and ammo = attack base.
        //no PursueBase in this state as may be low on fuel and don't want to waste on travel.
        else if ((tank.targetFound == false && tank.targetInRange == false) && 
            tank.baseInRange == true && tank.critAmmo == false)
        {
            return typeof(AttackBases);
        }
        //tank not found, base not found, good health, ammo and fuel = explore.
        else if ((tank.targetFound = false && tank.targetInRange == false) && tank.baseFound == false 
            && (tank.critHealth == false && tank.critFuel == false && tank.critAmmo == false))
        {
            return typeof(ExploreState);
        }
        //else carry on looking for a consumable.
        else
        {
            tank.MoveToConsumable();
            return null;
        }  
    }
}
