using DakaniLabs.CardLane.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRandomAI {

    private System.Random random = new System.Random();

    public Card pickCard(IList<Card> cards)
    {
        return cards[random.Next(cards.Count)];
    }

    public int pickLane()
    {
        return random.Next(3);
    }
}
