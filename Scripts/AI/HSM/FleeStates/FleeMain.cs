using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FleeMain : HSMBaseState
{
    public HSMTank tank;

    public FleeMain(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.fleeMain = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.fleeMain = false;
        return null;
    }

    public override Type UpdateState()
    {
        //first check if there is a tank target.
        if(tank.tankTargetInRange == false) {
            return typeof(ExploreMain);     //if not, switch to explore, either to find consumable or new target.
        } else {
            //there is a tank targetted.
            if(tank.critAmmo == false && tank.critHealth == false) {
                //no critical ammo and health.
                if(tank.tankTargetInRange == true) {
                    return typeof(AttackMain);      //got health and ammo, tank in range, then switch to attack state.
                } else {
                    return typeof(PursueMain);      //got health and ammo, tank found, switch to pursue state.
                }
            }
            //otherwise tank found, have critical ammo or health, keep fleeing.
            //pick which state to flee in.
            else {
                if (tank.goodFuel == true)
                {
                    return typeof(FleeFast);        //plenty of fuel, move to the fast flee state.
                }
                else
                {
                    return typeof(FleeBalanced);    //fuel is not good (ok or critical) so move balanced.
                }
            } 
        }
    }


}
