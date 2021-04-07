using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExploreBalanced : HSMBaseState
{
    public HSMTank tank;

    public ExploreBalanced(HSMTank tank)
    {
        this.tank = tank;
    }
    
    public override Type EnterState()
    {
        tank.exploreBalanced = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.exploreBalanced = false;
        return null;
    }

    public override Type UpdateState()
    {
        //first check if targetFound.
        if (tank.anyTarget == true) {
            return typeof(ExploreMain);     //kick back up to ExploreMain to decide on next state move.
        } else {
            //no targetFound, so staying in explore state; check which sub-state to go into here.
            if (tank.anyCritLevel == true) {
                return typeof(ExploreConsume);
            } else if (tank.goodFuel == true) {
                return typeof(ExploreFast);
            } else {
                //params, 1.0f is for speed (balanced), 10.0f is for wait (standard).
                tank.ExploreForTargets(tank.balancedSpeed, 10.0f);
                return null;
            }
        }
    }


}


