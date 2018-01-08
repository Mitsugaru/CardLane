using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DakaniLabs.CardLane.Phase
{
    /// <summary>
    /// Empty placeholder phase
    /// </summary>
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
}