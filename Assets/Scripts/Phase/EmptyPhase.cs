using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyPhase : GamePhase
{
    public override void execute()
    {
    }

    public override GamePhase getNextPhase()
    {
        return this;
    }

    public override bool hasCompleted()
    {
        return false;
    }
}
