using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FindHealth : HSMBaseState
{
    public HSMTank tank;

    public FindHealth(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.findingHealth = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.findingHealth = false;
        return null;
    }

    public override Type UpdateState()
    {
        if (tank.critHealth == false) {
            return typeof(ExploreMain);
        }
        else if (tank.healthFound == true)
        {
            tank.MoveToConsumableHSM(tank.healthDict, tank.healthCon, tank.balancedSpeed);
            return null;
        }
        else if (tank.critFuel == true && tank.fuelFound == true)
        {
            return typeof(FindFuel);
        } else
        {
            tank.ExploreForTargets(tank.fastSpeed, 10.0f);
            return null;
        }
    }
}
