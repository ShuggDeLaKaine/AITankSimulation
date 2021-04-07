using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackAndMove : HSMBaseState
{
    public HSMTank tank;

    public AttackAndMove(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.attackAndMove = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.attackAndMove = false;
        return null;
    }

    public override Type UpdateState()
    {
        //check if target in range, if not kick up to main attack state. 
        if (tank.tankTargetInRange == false) {
            return typeof(AttackMain);
        }
        //tank is in range, checking if any critical levels.
        else if (tank.anyCritLevel == true) { 
            if (tank.goodFuel != true) {    
                return typeof(AttackTankBalanced);      //not good level of fuel, keep attacking but stop moving and attacking.
            } else {
                return typeof(AttackMain);      //otherwise ammo or health is critical, so kick up to attack main state to decided next move.
            }
        } else {
            tank.AttackAndMove(tank.fastSpeed, 2.0f);   //otherwise all good, so keep attacking and moving
            return null;
        }
    }


}
