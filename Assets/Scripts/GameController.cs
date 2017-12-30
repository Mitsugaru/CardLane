using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public CardManager cardManager;

    public ArenaManager arenaManager;

    public DeckScript visualPlayerDeck;

    public DeckScript visualOpponentDeck;

    public HandManager playerHand;

    public HandManager opponentHand;

    private PlayerDeck playerDeck;

    private PlayerDeck opponentDeck;

    private bool playerFirst = false;

    private bool addJokers = true;

    private bool first = false;

    // Use this for initialization
    void Start()
    {
        playerDeck = new PlayerDeck();
        opponentDeck = new PlayerDeck();
    }

    // Update is called once per frame
    void Update()
    {
        if(cardManager.Ready && !first)
        {
            NewGame();
            first = true;
        }
    }

    public void NewGame()
    {
        //TODO detect if there is a current game already and ask if player wants to abandon current

        // Clear the arena
        arenaManager.Clear();
        playerHand.Clear();
        opponentHand.Clear();
        visualPlayerDeck.SpawnDeck();
        visualOpponentDeck.SpawnDeck();

        //TODO check if this is a rematch, if so, auto switch colors

        if (Random.value > 0.5f)
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
        playerDeck.ShuffleAll();
        opponentDeck.AddRange(opponentCards);
        opponentDeck.ShuffleAll();

        // Draw initial hands as a fast coroutine for animation to occur
        StartCoroutine(initialHandDraw());

        //TODO Start setup portion of game

        //TODO once setup is complete, start game loop
    }

    private IEnumerator initialHandDraw()
    {
        for (int i = 0; i < 5; i++)
        {
            Card playerCard = playerDeck.Draw();
            GameObject playerCardGameObject = cardManager.SpawnCard(playerCard);
            playerHand.AddCard(playerCardGameObject);
            visualPlayerDeck.DrawCard();
            yield return new WaitForSeconds(0.4f);

            Card opponentCard = opponentDeck.Draw();
            GameObject opponentCardGameObject = cardManager.SpawnCard(opponentCard);
            opponentHand.AddCard(opponentCardGameObject);
            visualOpponentDeck.DrawCard();
            yield return new WaitForSeconds(0.4f);
        }
    }

    public void playerDraw()
    {
        Card card = playerDeck.Draw();
        if (card != null)
        {
            visualPlayerDeck.DrawCard();
        }
        else
        {
            //TODO need to check if the player hands still have cards to play
            // otherwise, we need to reveal and resolve the remaining cards
            // on the table
        }
    }

    private IList<Card> generateCardsForSuit(Suit suit)
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

    private void populateCardLists(List<Card> redList, List<Card> blackList)
    {
        redList.AddRange(generateCardsForSuit(Suit.DIAMONDS));
        redList.AddRange(generateCardsForSuit(Suit.HEARTS));
        blackList.AddRange(generateCardsForSuit(Suit.CLUBS));
        blackList.AddRange(generateCardsForSuit(Suit.SPADES));
    }
}
