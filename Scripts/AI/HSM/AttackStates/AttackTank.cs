using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackTank : HSMBaseState
{
    public HSMTank tank;

    public AttackTank(HSMTank tank)
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
        //check that tank still in range, if not kick back to attack main state to deal with.
        if(tank.tankTargetInRange == false) {
            return typeof(AttackMain);
        } else {
            //tank is still in range, so check no critical levels
            if (tank.anyCritLevel == true) {
                //if it's just the fuel levels that are critical.
                if (tank.critFuel == true) { 
                    return typeof(AttackTankBalanced);      //then attack balanced, so not to waste fuel.
                }
                return typeof(AttackMain);      //otherwise, health or ammo critical, kick to main state to transition out of here.
            } else {
                //no critical levels, fuel and health check to determine type of attack.
                if(tank.goodFuel == true || tank.okHealth == true) {
                    return typeof(AttackAndMove);
                } else {
                    return typeof(AttackTankBalanced);
                }
            }
        }
    }


}
