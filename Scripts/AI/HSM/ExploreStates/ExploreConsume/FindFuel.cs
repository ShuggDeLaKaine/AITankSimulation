using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FindFuel : HSMBaseState
{
    public HSMTank tank;

    public FindFuel(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.findingFuel = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.findingFuel = false;
        return null;
    }

    public override Type UpdateState()
    {
        if (tank.critFuel == false) {
            return typeof(ExploreMain);
        }
        else if (tank.fuelFound == true)
        {
            tank.MoveToConsumableHSM(tank.fuelDict, tank.fuelCon, tank.balancedSpeed);
            return null;
        }
        else
        {
            tank.ExploreForTargets(tank.fastSpeed, 10.0f);
            return null;
        }
    }
}
