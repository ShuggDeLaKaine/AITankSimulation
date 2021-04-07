using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PursueTankFast : HSMBaseState
{
    public HSMTank tank;

    public PursueTankFast(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.pursueTankFast = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.pursueTankFast = false;
        return null;
    }

    public override Type UpdateState()
    {
        //if no tank then kick back to pursue main state.
        if (tank.tankTarget == false) {
            return typeof(PursueMain);
        } else if (tank.tankTargetInRange == true) {
            return typeof(PursueMain);
        } else {
            //if any critical levels, kick back to main state.
            if (tank.anyCritLevel == true) {
                return typeof(PursueMain);
            } else if (tank.okFuel == true) {
                //if fuel ok, move to balanced pursuit.
                return typeof(PursueTankBalanced);
            } else {
                //otherwise, crack on and pursue at a fast 1.5f speed.
                tank.PursueTargetHSM(tank.fastSpeed);
                return null;
            }
        }
    }


}
