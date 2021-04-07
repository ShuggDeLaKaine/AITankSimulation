using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExploreMain : HSMBaseState
{
    public HSMTank tank;

    public ExploreMain(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        tank.exploreMain = true;
        return null;
    }

    public override Type ExitState()
    {
        tank.exploreMain = false;
        return null;
    }

    public override Type UpdateState()
    {
        if(tank.anyTargetInRange == true)
        {
            if (tank.critHealth == true || tank.critAmmo == true)
            {
                return typeof(FleeMain);            //target is in range and critical health or ammo, so flee main.
            }
            else
            {
                return typeof(AttackMain);          //got health and ammo, in range, attack.
            }      
        }
        else if((tank.anyTarget == true) && (tank.critHealth == false && tank.critAmmo == false))
        {
            return typeof(PursueMain);      //not in range, so pursue
        }
        else
        {
            //no targetFound, so staying in explore state; check which sub-state to go into here.
            if (tank.anyCritLevel == true)
            {
                return typeof(ExploreConsume);
            }
            else if (tank.goodFuel == true)
            {
                return typeof(ExploreFast);
            }
            else
            {
                return typeof(ExploreBalanced);
            }
        }

        /*
        //first check if targetFound
        if (tank.anyTarget == true) {
            //then the levels of health and ammo
            if (tank.critHealth == false && tank.critAmmo == false) {
                //check if the tank is in range (base target secondary to tank target).
                if(tank.tankTargetInRange == true) {
                    return typeof(AttackMain);      //in range, so attack main
                } else {
                    return typeof(PursueMain);      //not in range, so pursue main (if it's a base, pursue state will knock over into attack-attackbase).
                }
            } else {
                return typeof(FleeMain);            //target is in range and critical health or ammo, so flee main.
            }
        } else {
            //no targetFound, so staying in explore state; check which sub-state to go into here.
            if (tank.anyCritLevel == true) {
                return typeof(ExploreConsume);
            } else if (tank.goodFuel == true) {
                return typeof(ExploreFast);
            } else {
                return typeof(ExploreBalanced);
            }
        }
        */
    }


}
