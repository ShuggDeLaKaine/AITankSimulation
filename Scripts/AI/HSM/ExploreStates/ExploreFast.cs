using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExploreFast : HSMBaseState
{
    public HSMTank tank;

    public ExploreFast(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.exploreFast = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.exploreFast = false;

        return null;
    }

    public override Type UpdateState()
    {
        //first check if targetFound
        if (tank.anyTarget == true) {
            return typeof(ExploreMain);         //kick back up to ExploreMain to decide on next state move.
        } else {
            //no targetFound, so staying in explore state; check which sub-state to go into here.
            if (tank.anyCritLevel == true) {
                return typeof(ExploreConsume);
            } else if (tank.okFuel == true) {
                return typeof(ExploreBalanced);
            } else {
                //params, 1.5f is for speed (up from 1.0f), 6.5f is for wait (down from 10.0f)
                tank.ExploreForTargets(tank.fastSpeed, 6.5f);
                return null;
            }
        }
    }
}
