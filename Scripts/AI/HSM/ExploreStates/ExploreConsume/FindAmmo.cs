using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FindAmmo : HSMBaseState
{
    public HSMTank tank;

    public FindAmmo(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.findingAmmo = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.findingAmmo = false;
        return null;
    }

    public override Type UpdateState()
    {
        if (tank.critAmmo == false)
        {
            return typeof(ExploreMain);
        }
        else if (tank.ammoFound == true)
        {
            tank.MoveToConsumableHSM(tank.ammoDict, tank.ammoCon, tank.balancedSpeed);
            return null;
        }
        else if (tank.critFuel == true && tank.fuelFound == true)
        {
            return typeof(FindFuel);
        }
        else if (tank.critHealth == true && tank.healthFound == true)
        {
            return typeof(FindHealth);
        }
        else
        {
            tank.ExploreForTargets(tank.fastSpeed, 10.0f);
            return null;
        }
    }
}
