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

    private SimpleRandomAI ai;

    private float delay = 0.3f;

    private bool playerFirst = false;

    private bool addJokers = true;

    private bool first = false;

    private bool waitForInitialLaneFill = false;

    private bool gameLoop = false;

    private bool playerTurn = false;

    // Use this for initialization
    void Start()
    {
        playerDeck = new PlayerDeck();
        opponentDeck = new PlayerDeck();
        ai = new SimpleRandomAI();
        
        if (Application.platform != RuntimePlatform.WindowsPlayer
            || Application.platform != RuntimePlatform.WindowsEditor
            || Application.platform != RuntimePlatform.OSXPlayer
            || Application.platform != RuntimePlatform.OSXEditor)
        {
            delay = 0.4f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!first && cardManager.Ready)
        {
            NewGame();
            first = true;
        }

        //TODO Start setup portion of game
        if (waitForInitialLaneFill)
        {
            //TODO have AI play cards
            if (checkPlayerLanesFilled() && checkOpponentLanesFilled() && checkHandSetup())
            {
                waitForInitialLaneFill = false;
                gameLoop = true;
            }
            else
            {
                handlePlayerSetup();
                handleOpponentSetup();
            }
            //TODO once setup is complete, start game loop
        }

        if (gameLoop)
        {
            if (playerTurn)
            {
                while(playerHand.Count < 5)
                {
                    Card playerCard = playerDeck.Draw();
                    GameObject playerCardGameObject = cardManager.SpawnCard(playerCard);
                    playerHand.AddCard(playerCardGameObject);
                    visualPlayerDeck.DrawCard();
                }
                //TODO make a lane selection
                //TODO animate the cards
                //TODO after animation resolve
                //TODO remove cards from lane slots
                //TODO wait for player lane to be filled
                //TODO have AI fill lane
            }
            else
            {
                while(opponentHand.Count < 5)
                {
                    Card opponentCard = opponentDeck.Draw();
                    GameObject opponentCardGameObject = cardManager.SpawnCard(opponentCard);
                    opponentHand.AddCard(opponentCardGameObject);
                    visualOpponentDeck.DrawCard();
                }
                // Let AI decide
            }
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
            playerTurn = true;
        }
        else
        {
            playerFirst = false;
            playerTurn = false;
        }

        //Generate card lists
        List<Card> playerCards = new List<Card>();
        List<Card> opponentCards = new List<Card>();

        if (playerFirst)
        {
            CardUtils.populateCardLists(playerCards, opponentCards);
        }
        else
        {
            CardUtils.populateCardLists(opponentCards, playerCards);
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
    }

    private IEnumerator initialHandDraw()
    {
        for (int i = 0; i < 5; i++)
        {
            Card playerCard = playerDeck.Draw();
            GameObject playerCardGameObject = cardManager.SpawnCard(playerCard);
            playerHand.AddCard(playerCardGameObject);
            visualPlayerDeck.DrawCard();
            yield return new WaitForSeconds(delay);

            Card opponentCard = opponentDeck.Draw();
            GameObject opponentCardGameObject = cardManager.SpawnCard(opponentCard);
            opponentHand.AddCard(opponentCardGameObject);
            visualOpponentDeck.DrawCard();
            yield return new WaitForSeconds(delay);
        }

        // Flag that hands have been drawn and we can play cards
        waitForInitialLaneFill = true;
    }

    private bool checkPlayerLanesFilled()
    {
        return arenaManager.LaneA.PlayerCard != null
                && arenaManager.LaneB.PlayerCard != null
                && arenaManager.LaneC.PlayerCard != null;
    }

    private bool checkOpponentLanesFilled()
    {
        return arenaManager.LaneA.OpponentCard != null
                && arenaManager.LaneB.OpponentCard != null
                && arenaManager.LaneC.OpponentCard != null;
    }

    private void handlePlayerSetup()
    {
        if (checkPlayerLanesFilled() && playerHand.Count < 5)
        {
            Card playerCard = playerDeck.Draw();
            GameObject playerCardGameObject = cardManager.SpawnCard(playerCard);
            playerHand.AddCard(playerCardGameObject);
            visualPlayerDeck.DrawCard();
        }
    }

    private void handleOpponentSetup()
    {
        Lane targetLane = null;
        int lane = ai.pickLane();
        switch (lane)
        {
            case 0:
                if (arenaManager.LaneA.OpponentCard == null)
                {
                    targetLane = arenaManager.LaneA;
                }
                break;
            case 1:
                if (arenaManager.LaneB.OpponentCard == null)
                {
                    targetLane = arenaManager.LaneB;
                }
                break;
            case 2:
                if (arenaManager.LaneC.OpponentCard == null)
                {
                    targetLane = arenaManager.LaneC;
                }
                break;
            default:
                break;
        }

        if (targetLane != null)
        {
            Card card = ai.pickCard(opponentHand.GetCards());
            opponentHand.selectCard(card);
            GameObject cardGO = opponentHand.GetSelectedCard();
            if (targetLane.setCard(cardGO, Lane.Slot.OPPONENT))
            {
                arenaManager.placeCard(cardGO, targetLane.OpponentSlot);
                opponentHand.RemoveCard(cardGO);
            }
        }
        else
        {
            if (checkOpponentLanesFilled() && opponentHand.Count < 5)
            {
                Card opponentCard = opponentDeck.Draw();
                GameObject opponentCardGameObject = cardManager.SpawnCard(opponentCard);
                opponentHand.AddCard(opponentCardGameObject);
                visualOpponentDeck.DrawCard();
            }
        }
    }

    private bool checkHandSetup()
    {
        return playerHand.Count == 5 && opponentHand.Count == 5;
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
}
