using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private PlayerDeck playerDeck;

    private PlayerDeck opponentDeck;

    private bool playerFirst = false;

    private bool addJokers = true;

	// Use this for initialization
	void Start () {
        playerDeck = new PlayerDeck();
        opponentDeck = new PlayerDeck();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NewGame()
    {
        //TODO detect if there is a current game already and ask if player wants to abandon current
        
        //TODO check if this is a rematch, if so, auto switch colors

        if(Random.value > 0.5f)
        {
            playerFirst = true;
        }

        //Generate card lists
        List<Card> playerCards = new List<Card>();
        List<Card> opponentCards = new List<Card>();

        if (playerFirst)
        {
            populateCardLists(playerCards, opponentCards);
        }
        else
        {
            populateCardLists(opponentCards, playerCards);
        }

        // check if joker rule
        if (addJokers)
        {
            playerCards.Add(new Card(Rank.JOKER, Suit.NONE));
            opponentCards.Add(new Card(Rank.JOKER, Suit.NONE));
        }

        playerDeck.AddRange(playerCards);
        opponentDeck.AddRange(opponentCards);

        //TODO Start setup portion of game
        //TODO once setup is complete, start game loop
    }

    private IList<Card> generateCardsForSuit(Suit suit)
    {
        IList<Card> cards = new List<Card>();

        foreach(Rank rank in Rank.Values)
        {
            if(rank != Rank.JOKER)
            {
                cards.Add(new Card(rank, suit));
            }
        }

        return cards;
    }

    private void populateCardLists(List<Card> redList, List<Card> blackList)
    {
        redList.AddRange(generateCardsForSuit(Suit.DIAMONDS));
        redList.AddRange(generateCardsForSuit(Suit.HEARTS));
        blackList.AddRange(generateCardsForSuit(Suit.CLUBS));
        blackList.AddRange(generateCardsForSuit(Suit.SPADES));
    }
}
