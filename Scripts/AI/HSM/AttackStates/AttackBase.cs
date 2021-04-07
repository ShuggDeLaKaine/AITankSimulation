using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackBase : HSMBaseState
{
    public HSMTank tank;

    public AttackBase(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.attackBase = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.attackBase = false;
        return null;
    }

    public override Type UpdateState()
    {
        //if no base in range, or a tank is found or in range, then kick back to attack main state to transition.
        if (tank.currentBase != null)
        {
            if (tank.baseTargetInRange == false || (tank.tankTargetInRange == true || tank.tankTarget == true))
            {
                return typeof(AttackMain);
            }
            else if (tank.currentBase == null)
            {
                return typeof(AttackMain);
            }
            else
            {
                //otherwise base in range, no tank near; check if any critical levels.
                if (tank.anyCritLevel == true)
                {
                    //if it's just fuel that is critical, keep attacking (only 1 shot to destroy base...).
                    if (tank.critFuel == true)
                    {
                        tank.AttackBase();
                        return null;
                    }
                    else
                    {
                        return typeof(AttackMain);      //otherwise ammo or health is critical, so kick up to attack main state to transition.
                    }
                }
                //otherwise no criticals, no tank, base in range so attack it.
                tank.AttackBase();
                return null;
            }
        } else
        {
            return typeof(AttackMain);
        }
    }


}
