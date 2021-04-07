using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackTankBalanced : HSMBaseState
{
    public HSMTank tank;

    public AttackTankBalanced(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.attackTankBalanced = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.attackTankBalanced = false;
        return null;
    }

    public override Type UpdateState()
    {
        //check if target in range, if not kick up to main attack state. 
        if (tank.tankTargetInRange == false) {
            return typeof(AttackMain);
        } else if (tank.anyCritLevel == true) { 
            //tank is in range, checking if any critical levels.
            if (tank.critFuel == false) {
                return typeof(AttackAndMove);   //if fuel isn't critical, get moving and attacking.
            } else {
                return typeof(AttackMain);      //otherwise ammo or health critical, so kick up to attack main state to decided.
            }
        } else {
            tank.AttackTarget();   //otherwise all good, so keep attacking but no moving.
            return null;
        }
    }
}
