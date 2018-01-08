using DakaniLabs.CardLane.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPhase : GamePhase
{

    public CardManager CardManager { get; set; }

    public HandManager Hand { get; set; }

    public PlayerDeck Deck { get; set; }

    public DeckScript VisualDeck { get; set; }

    public bool PlayerTurn { get; set; }

    public override void execute()
    {
        if (!hasCompleted())
        {
            Card playerCard = Deck.Draw();
            if (playerCard != null)
            {
                GameObject playerCardGameObject = CardManager.SpawnCard(playerCard);
                if (playerCardGameObject != null)
                {
                    Hand.AddCard(playerCardGameObject);
                    VisualDeck.DrawCard();
                }
            }
        }
    }

    public override GamePhase getNextPhase()
    {
        GamePhase next = null;
        if(PlayerTurn)
        {
            next = new LaneSelectionPhase();
        }
        else
        {
            next = new AILaneSelectionPhase();
        }
        return next;
    }

    public override bool hasCompleted()
    {
        return Hand.Count >= 5 || Deck.Deck.Count == 0;
    }
}
