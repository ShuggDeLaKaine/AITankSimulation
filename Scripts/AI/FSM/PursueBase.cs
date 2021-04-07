using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PursueBase : GroundState
{
    public FSMTank tank;


    public PursueBase(FSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.pursueBase = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.pursueBase = false;
        return null;
    }

    public override Type UpdateState()
    {
        //base in range, has ammo, no tank found or in range = attack base.
        if(tank.baseInRange == true && tank.critFuel == false &&
            tank.targetFound == false && tank.targetInRange == false)
        {
            return typeof(AttackBases);
        }
        
        else if (tank.baseFound == false && tank.targetFound == false)
        {
            //base not found, tank not found = explore
            if (tank.critHealth == true || tank.critFuel == true || tank.critAmmo == true)
            {
                return typeof(ExploreConsumable);
            }
            //base not found, tank not found, health OR ammo OR fuel low = explore consumable.
            else
            {
                return typeof(ExploreState);
            }
        }

        else if(tank.targetFound == true && tank.critHealth == false && tank.critAmmo == false)
        {
            //tank found, tank not in range, health AND ammo good = pursue tank.
            if (tank.targetInRange == false)
            {
                return typeof(PursueState);
            }
            //tank found, tank in range, health AND ammo good = attack tank.
            else /*if (tank.targetInRange == true)*/
            {
                return typeof(AttackState);
            }
        }
        //tank found OR in range, low health OR ammo = flee.
        else if ((tank.targetFound == true || tank.targetInRange == true) 
            && (tank.critHealth == true || tank.critAmmo == true))
        {
            return typeof(FleeState);
        }
        //other keep moving towards the base.
        else
        {
            tank.MoveToBase();
            return null;
        }
    }
}
