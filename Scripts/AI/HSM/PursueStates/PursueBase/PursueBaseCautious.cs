using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PursueBaseCautious : HSMBaseState
{
    public HSMTank tank;

    public PursueBaseCautious(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.pursueBaseCautious = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.pursueBaseCautious = false;
        return null;
    }

    public override Type UpdateState()
    {
        //if no base target or a tank target appears, back to main state.
        if (tank.baseTarget == false || tank.tankTarget == true) {
            return typeof(PursueMain);
        } else if (tank.baseTargetInRange == true) {
            return typeof(PursueMain);
        } else {
            //critical levels check, if true, kick back to main state
            if (tank.anyCritLevel == true) {
                return typeof(PursueMain);
            } else if (tank.goodFuel == true) {
                //fuel good, so save fuel and take cautious speed pursuit.
                return typeof(PursueBaseBalanced);
            } else {
                //otherwise, crack on making way to base at slow speed(0.75f).
                tank.MoveToBaseHSM(0.75f);
                return null;
            }
        }
    }


}
