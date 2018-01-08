using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DakaniLabs.CardLane.Phase
{
    public class AILaneSelectionPhase : LaneSelectionPhase
    {

        public ArenaManager ArenaManager { get; set; }

        public SimpleRandomAI AI { get; set; }

        public override void execute()
        {
            if (SelectedLane == null)
            {
                int lane = AI.pickLane();
                switch (lane)
                {
                    case 0:
                        SelectedLane = ArenaManager.LaneA;
                        break;
                    case 1:
                        SelectedLane = ArenaManager.LaneB;
                        break;
                    case 2:
                        SelectedLane = ArenaManager.LaneC;
                        break;
                    default:
                        break;
                }
            }

            checkReveal();
        }

    }
}