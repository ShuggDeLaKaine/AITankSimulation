using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackMain : HSMBaseState
{
    public HSMTank tank;

    public AttackMain(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.attackMain = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.attackMain = false;
        return null;
    }

    public override Type UpdateState()
    {
        //if no longer any target in range, then...
        if (tank.anyTargetInRange == false) {
            //if still a target found
            if (tank.anyTarget == true) {
                //check if ammo or health is critical
                if (tank.critAmmo == true || tank.critHealth == true) {
                    return typeof(FleeMain);    //if so, get out of there
                } else { 
                    return typeof(PursueMain);  //otherwise have health and ammo so pursue target, switch to pursue state.
                }
            } else {       
                return typeof(ExploreMain);     //otherwise no target found, so switch to explore main state.
            }
        } else {
            //so anyTargetInRange is true, staying in attack state
            //so need to check whether target is a tank or base; if there is both, then tank takes precedence.
            if ((tank.tankTargetInRange == true) || (tank.tankTargetInRange == true && tank.baseTargetInRange == true))
            {
                return typeof(AttackTank);
            }
            else
            {
                return typeof(AttackBase);
            }
        }
    }


}
