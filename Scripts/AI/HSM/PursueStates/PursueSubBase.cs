using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PursueSubBase : HSMBaseState
{
    public HSMTank tank;

    public PursueSubBase(HSMTank tank)
    {
        this.tank = tank;
    }

    public override Type EnterState()
    {
        return null;
    }

    public override Type ExitState()
    {

        return null;
    }

    public override Type UpdateState()
    {
        //check if base still found and that a tank has NOT been found (tanks take priority).
        if(tank.baseTarget == false || tank.tankTarget == true) {
            //kick to main pursue state to make decision on next move.
            return typeof(PursueState);
        } else {
            //still got a base target and no tank target, so...
            //check if any critical levels, if so kick up to main pursue state
            if(tank.anyCritLevel == true) {
                return typeof(PursueMain);
            } else {
                //no critical levels, so fuel level check to decide next move.
                //either a balanced speed or a low speed. 
                //no need to rush and burn excess fuel as that base ain't going nowhere.
                if(tank.goodFuel == true) {
                    return typeof(PursueBaseBalanced);
                } else {
                    return typeof(PursueBaseCautious);
                }
            }
        }
    }


}
