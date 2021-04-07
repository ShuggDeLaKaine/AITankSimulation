using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FiringRetreat : HSMBaseState
{
    public HSMTank tank;
    public FleeMain main;

    public FiringRetreat(HSMTank tank)
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

        return null;
    }
}
