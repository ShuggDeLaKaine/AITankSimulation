using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class HSMBaseState
{
    public abstract Type EnterState();
    public abstract Type UpdateState();
    public abstract Type ExitState();
}
