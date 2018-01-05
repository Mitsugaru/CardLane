using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPhase : GamePhase
{

    public ArenaManager ArenaManager { get; set; }

    public UIManager InterfaceManager { get; set; }

    private RevealPhase revealPhase;

    private Lane selectedLane;

    private bool laneSelection = true;

    private bool reveal = false;

    private bool completed = false;

    public override void execute()
    {
        if (laneSelection)
        {
            foreach (Lane lane in ArenaManager.getLanes())
            {
                if (lane.PlayerCard != null
                    && lane.OpponentCard != null)
                {
                    selectedLane = lane;
                    CardUtils.animateCard(selectedLane.PlayerCard);
                    CardUtils.animateCard(selectedLane.OpponentCard);
                    laneSelection = false;
                    reveal = true;
                    revealPhase = new RevealPhase();
                    revealPhase.ArenaManager = ArenaManager;
                    revealPhase.SelectedLane = selectedLane;
                    revealPhase.StockpileRule = false;
                    break;
                }
            }
        }

        if (reveal)
        {
            revealPhase.execute();

            if (revealPhase.hasCompleted())
            {
                laneSelection = true;
                reveal = false;
            }
        }
        else
        {
            //true end, figure out winner
            int playerLanes = 0;
            int opponentLanes = 0;

            foreach (Lane lane in ArenaManager.getLanes())
            {
                //TODO highlight the lane to the winner
                if (lane.playerPoints > lane.opponentPoints)
                {
                    playerLanes++;
                }
                else if (lane.playerPoints < lane.opponentPoints)
                {
                    opponentLanes++;
                }
            }

            if (playerLanes > opponentLanes)
            {
                InterfaceManager.gameResultLabel.text = "You Win";
                InterfaceManager.gameResultLabel.color = Color.green;
            }
            else if (opponentLanes > playerLanes)
            {
                InterfaceManager.gameResultLabel.text = "You Lose";
                InterfaceManager.gameResultLabel.color = Color.red;
            }
            else
            {
                InterfaceManager.gameResultLabel.text = "Draw";
                InterfaceManager.gameResultLabel.color = Color.black;
            }

            completed = true;
        }
    }

    public override GamePhase getNextPhase()
    {
        return new EmptyPhase();
    }

    public override bool hasCompleted()
    {
        return completed;
    }
}
