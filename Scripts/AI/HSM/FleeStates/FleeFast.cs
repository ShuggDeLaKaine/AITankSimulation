using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FleeFast : HSMBaseState
{
    public HSMTank tank;

    public FleeFast(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.fleeFast = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.fleeFast = false;
        return null;
    }

    public override Type UpdateState()
    {
        //if no tank target OR if ammo and health not critical.
        if(tank.tankTargetInRange == false) {
            return typeof(FleeMain);        //then kick back to main flee state to decide on next transition.
        } else if (tank.critAmmo == false && tank.critHealth == false)
        {
            return typeof(FleeMain);
        }
        else {
            //otherwise there is a tank target or ammo or health is critical, so...
            //first check the fuel level
            if(tank.critFuel == true) {
                return typeof(FleeBalanced);        //fuel critical, save some, change to moving at standard speed.
            } else {
                //otherwise, have more than critical fuel so move fast
                //speed = 1.5f (standard: 1.0f), wait = 6.5f (standard: 10.0f)
                tank.FleeTargetHSM(tank.fastSpeed, 6.5f);
                return null;
            }
        }
    }
}
