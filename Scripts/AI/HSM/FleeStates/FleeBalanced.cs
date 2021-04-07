using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FleeBalanced : HSMBaseState
{
    public HSMTank tank;

    public FleeBalanced(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.fleeBalanced = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.fleeBalanced = false;
        return null;
    }

    public override Type UpdateState()
    {
        //if no tank target OR if ammo and health not critical.
        if (tank.tankTargetInRange == false)
        {
            return typeof(FleeMain);        //then kick back to main flee state to decide on next transition.
        }
        else if (tank.critAmmo == false && tank.critHealth == false)
        {
            return typeof(FleeMain);
        } else {
            //otherwise there is a tank target or ammo or health is critical, so...
            //first check the fuel level
            if (tank.critFuel != true) {
                return typeof(FleeFast);    //fuel not critical, burn some to get away, change to moving at fast speed.
            } else {
                //otherwise, have more than critical fuel so move fast
                //speed = 1.0f (standard: 1.0f), wait = 10.0f (standard: 10.0f)
                tank.FleeTargetHSM(tank.balancedSpeed, 10.0f);
                return null;
            }
        }
    }


}
