using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PursueBaseBalanced : HSMBaseState
{
    public HSMTank tank;

    public PursueBaseBalanced(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.pursueBaseBalanced = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.pursueBaseBalanced = false;
        return null;
    }

    public override Type UpdateState()
    {
        //if no base target or a tank target appears, back to main state.
        if(tank.baseTarget == false || tank.tankTarget == true) {
            return typeof(PursueMain);
        } else if (tank.baseTargetInRange == true) {
            return typeof(PursueMain);
        } else {
            //critical levels check, if true, kick back to main state
            if(tank.anyCritLevel == true) {
                return typeof(PursueMain);
            } else if(tank.okFuel == true) {
                //fuel only ok, so save fuel and take cautious speed pursuit.
                return typeof(PursueBaseCautious);
            } else {
                //otherwise, crack on making way to base at standard speed(1.0f).
                tank.MoveToBaseHSM(tank.balancedSpeed);
                return null;
            }
        }
    }


}
