using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PursueState : GroundState
{
    public FSMTank tank;


    public PursueState(FSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.pursueState = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.pursueState = false;
        return null;
    }

    public override Type UpdateState()
    {
        //tank not found, base not found = explore
        if (tank.targetFound == false && tank.baseFound == false)
        {
            return typeof(ExploreState);
        }
        //tank not found, base found = pursue base
        else if (tank.targetFound == false && tank.baseFound == true)
        {
            return typeof(PursueBase);
        }
        //tank in range, good health AND ammo = attack tank.
        else if (tank.targetInRange && (tank.critHealth == false && tank.critAmmo == false))
        {
            return typeof(AttackState);
        }
        //tank found, low health OR ammo = flee. 
        else if (tank.targetFound == true && 
            (tank.critHealth == true || tank.critAmmo == true))
        {
            return typeof(FleeState);
        }
        //otherwise keep pursuing the tank.
        else
        {
            tank.PursueTarget();
            return null;
        }
    }

}
