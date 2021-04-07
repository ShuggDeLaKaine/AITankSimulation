using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackState : GroundState
{
    public FSMTank tank;


    public AttackState(FSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.attackState = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.attackState = false;
        return null;
    }

    public override Type UpdateState()
    {
        //no tank in range, no tank found, no base found...
        if(tank.targetFound == false && tank.targetInRange == false && tank.baseFound == false)
        {
            //... and low level of ammo OR fuel OR health = explore consumables.
            if(tank.critAmmo == true || tank.critFuel == true || tank.critHealth == true)
            {
                return typeof(ExploreConsumable);
            }
            //...no low levels = explore. 
            else
            {
                return typeof(ExploreState);
            }
        }
        //tank found, tank not in range, good health AND ammo = pursue tank.
        else if(tank.targetInRange == false && tank.targetFound == true &&
            (tank.critAmmo == false && tank.critHealth == false))
        {
            return typeof(PursueState);
        } 
        //tank not in range, tank not found, base found, good ammo = pursue base.
        else if(tank.targetInRange == false && tank.targetFound == false 
            && tank.baseFound == true && tank.critAmmo == false)
        {
            return typeof(PursueBase);
        }
        //low health OR ammo = flee.
        else if(tank.critAmmo == true || tank.critHealth == true)
        {
            if (tank.critHealth == true)
            {
                Debug.Log("FLEE because HEALTH critical");
            }
            else if (tank.critAmmo == true)
            {
                Debug.Log("FLEE because AMMO critical");
            }
            return typeof(FleeState);
        }
        //otherwise keep attacking the target tank.
        else
        {
            tank.AttackTarget();
            return null;
        }
    }
}
