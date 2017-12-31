using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUtils {
    public static IList<Card> generateCardsForSuit(Suit suit)
    {
        IList<Card> cards = new List<Card>();

        foreach (Rank rank in Rank.Values)
        {
            if (rank != Rank.JOKER)
            {
                cards.Add(new Card(rank, suit));
            }
        }

        return cards;
    }

    public static void populateCardLists(List<Card> redList, List<Card> blackList)
    {
        redList.AddRange(generateCardsForSuit(Suit.DIAMONDS));
        redList.AddRange(generateCardsForSuit(Suit.HEARTS));
        blackList.AddRange(generateCardsForSuit(Suit.CLUBS));
        blackList.AddRange(generateCardsForSuit(Suit.SPADES));
    }
}
