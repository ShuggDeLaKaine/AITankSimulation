using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExploreConsume : HSMBaseState
{
    public HSMTank tank;

    public ExploreConsume(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.exploreConsume = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.exploreConsume = false;
        return null;
    }

    public override Type UpdateState()
    {
        if (tank.anyCritLevel == true) {
            //ORDER: fuel check first (without fuel can't get any others), then health and finally ammo.
            if (tank.critFuel == true) {
                return typeof(FindFuel);
            } else if (tank.critHealth == true) {
                return typeof(FindHealth);
            } else {
                return typeof(FindAmmo);
            }
        } else if (tank.tankTargetInRange == true) {
            return typeof(ExploreMain);
        } else {
            //no critical levels, so kick back up explore main state to decide next move.
            return typeof(ExploreMain);
        }
    }


}
