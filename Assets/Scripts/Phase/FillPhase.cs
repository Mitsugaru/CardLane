using DakaniLabs.CardLane.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillPhase : GamePhase
{

    public ArenaManager ArenaManager { get; set; }

    public HandManager PlayerHand { get; set; }

    public HandManager OpponentHand { get; set; }

    public PlayerDeck OpponentDeck { get; set; }

    public SimpleRandomAI AI { get; set; }

    public override void execute()
    {
        if (!ArenaManager.checkOpponentLanesFilled())
        {
            // have AI fill lane
            foreach (Lane lane in ArenaManager.getLanes())
            {
                if (lane.OpponentCard == null)
                {
                    refreshOpponentLane(lane);
                }
            }
        }
    }

    public override GamePhase getNextPhase()
    {
        GamePhase next = null;
        if (PlayerHand.Count == 0 && OpponentHand.Count == 0)
        {
            next = new EndPhase();
        }
        else
        {
            next = new DrawPhase();
        }
        return next;
    }

    public override bool hasCompleted()
    {
        return (ArenaManager.checkPlayerLanesFilled() && ArenaManager.checkOpponentLanesFilled())
                        || (PlayerHand.Count == 0 && OpponentHand.Count == 0);
    }

    private void refreshOpponentLane(Lane lane)
    {
        if (lane.OpponentCard == null)
        {
            Card card = AI.pickCard(OpponentHand.GetCards());
            OpponentHand.selectCard(card);
            GameObject cardGO = OpponentHand.GetSelectedCard();
            if (lane.setCard(cardGO, Lane.Slot.OPPONENT))
            {
                ArenaManager.placeCard(cardGO, lane.OpponentSlot);
                OpponentHand.RemoveCard(cardGO);
            }
        }
    }
}
