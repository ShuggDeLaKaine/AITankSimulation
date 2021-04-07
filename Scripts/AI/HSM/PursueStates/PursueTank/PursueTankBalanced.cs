using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PursueTankBalanced : HSMBaseState
{
    public HSMTank tank;

    public PursueTankBalanced(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.pursueTankBalanced = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.pursueTankBalanced = false;
        return null;
    }

    public override Type UpdateState()
    {
        //if no tank then kick back to pursue main state.
        if(tank.tankTarget == false) {
            return typeof(PursueMain);
        } else if (tank.tankTargetInRange == true)
        {
            return typeof(PursueMain);
        }
        else {
            //if any critical levels, kick back to main state.
            if(tank.anyCritLevel == true){
                return typeof(PursueMain);
            } else if (tank.goodFuel == true) {
                //if fuel good, move to faster pursuit.
                return typeof(PursueTankFast);
            } else {
                //otherwise, crack on and pursue at a standard 1.0f speed.
                tank.PursueTargetHSM(tank.balancedSpeed);
                return null;
            }
        }
    }


}
