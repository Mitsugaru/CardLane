using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockpileDrawPhase : GamePhase
{

    public CardManager CardManager
    {
        get
        {
            return playerDraw.CardManager;
        }

        set
        {
            playerDraw.CardManager = value;
            opponentDraw.CardManager = value;
        }
    }

    public HandManager PlayerHand
    {
        get
        {
            return playerDraw.Hand;
        }

        set
        {
            playerDraw.Hand = value;
        }
    }

    public PlayerDeck PlayerDeck
    {
        get
        {
            return playerDraw.Deck;
        }

        set
        {
            playerDraw.Deck = value;
        }
    }

    public DeckScript PlayerVisualDeck
    {
        get
        {
            return playerDraw.VisualDeck;
        }

        set
        {
            playerDraw.VisualDeck = value;
        }
    }

    public HandManager OpponentHand
    {
        get
        {
            return opponentDraw.Hand;
        }

        set
        {
            opponentDraw.Hand = value;
        }
    }

    public PlayerDeck OpponentDeck
    {
        get
        {
            return opponentDraw.Deck;
        }

        set
        {
            opponentDraw.Deck = value;
        }
    }

    public DeckScript OpponentVisualDeck
    {
        get
        {
            return opponentDraw.VisualDeck;
        }

        set
        {
            opponentDraw.VisualDeck = value;
        }
    }

    private DrawPhase playerDraw = new DrawPhase();

    private DrawPhase opponentDraw = new DrawPhase();

    public override void execute()
    {
        if (!playerDraw.hasCompleted())
        {
            playerDraw.execute();
        }
        if (!opponentDraw.hasCompleted())
        {
            opponentDraw.execute();
        }
    }

    public override GamePhase getNextPhase()
    {
        return new FillPhase();
    }

    public override bool hasCompleted()
    {
        return playerDraw.hasCompleted() && opponentDraw.hasCompleted();
    }
}
