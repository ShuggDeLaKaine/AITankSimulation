using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackBases : GroundState
{
    public FSMTank tank;


    public AttackBases(FSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.attackBases = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.attackBases = false;
        return null;
    }

    public override Type UpdateState()
    {
        //no base found AND no base in range, no tank found AND no tank in range...
        if(tank.baseInRange == false && tank.baseFound == false &&
            tank.targetInRange == false && tank.targetFound == false)
        {
            //...with low health OR ammo OR fuel = explore consumable.
            if (tank.critAmmo == true || tank.critFuel == true || tank.critHealth == true)
            {
                return typeof(ExploreConsumable);
            }
            //...with fine health, ammo and fuel = explore.
            else
            {
                return typeof(ExploreState);
            }
        } 
        //good health AND ammo, tank found..
        else if((tank.critHealth == false && tank.critAmmo == false) && tank.targetFound == true)
        {
            //... and tank in range = attack tank.
            if (tank.targetInRange == true)
            {
                return typeof(AttackState);
            }
            //... tank not in range = pursue tank.
            else
            {
                return typeof(PursueState);
            }
        }
        //health OR ammo is low, tank found OR in range = flee.
        else if((tank.critHealth == true || tank.critAmmo == true) && 
            (tank.targetFound == true || tank.targetInRange == true))
        {
            return typeof(FleeState);
        } /*else if (tank.currentBase == null)
        {
            return typeof(ExploreState);
        }*/
        //othewise keep attacking the base.
        else
        {
            tank.AttackBase();
            return null;
        }
    }
}
