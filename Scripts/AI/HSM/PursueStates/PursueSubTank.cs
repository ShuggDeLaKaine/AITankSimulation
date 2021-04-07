using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PursueSubTank : HSMBaseState
{
    public HSMTank tank;

    public PursueSubTank(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        return null;
    }

    public override Type ExitState()
    {

        return null;
    }

    public override Type UpdateState()
    {
        //check if tank found, if false then kick back to main state to deal with.
        if (tank.tankTarget == false) {
            return typeof(PursueMain);
        } else {
            //still got a tank target, so...
            //check if any criticals and whether should still be pursuing.
            if(tank.anyCritLevel == true) {
                return typeof(PursueMain);      //have a critical, kick back to Pursue main to decide which transition to make.
            } else {
                //no critical level, therefore fuel check to determine speed of pursuit.
                if(tank.goodFuel == true) {
                    return typeof(PursueTankFast);
                } else {
                    return typeof(PursueTankBalanced);
                }
            }
        }
    }


}
