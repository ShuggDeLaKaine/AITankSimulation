using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PursueMain : HSMBaseState
{
    public HSMTank tank;

    public PursueMain(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.pursueMain = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.pursueMain = false;
        return null;
    }

    public override Type UpdateState()
    {
        if(tank.anyTarget == false || tank.anyCritLevel == true) {
            return typeof(ExploreMain);
        }  else if (tank.anyTarget == true && (tank.critAmmo == true || tank.critHealth == true)) {
            return typeof(FleeMain);
        } else if (tank.anyTargetInRange == true && (tank.critAmmo == false || tank.critHealth == false)) {
            return typeof(AttackMain);
        } else {
            //check whether it's a tank or base that has been seen, send them to the relevant one.
            //if both are detected, send to tank as that takes precedent.
            if (tank.anyTargetInRange == false && ((tank.tankTarget == true) || (tank.tankTarget == true && tank.baseTarget == true))) {
                return typeof(PursueSubTank);
            } else {
                return typeof(PursueSubBase);
            }
        }
    }


}
