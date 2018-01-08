using DakaniLabs.CardLane.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public UIManager uiManager;

    public CardManager cardManager;

    public ArenaManager arenaManager;

    public DeckScript visualPlayerDeck;

    public DeckScript visualOpponentDeck;

    public HandManager playerHand;

    public HandManager opponentHand;

    private PlayerDeck playerDeck;

    private PlayerDeck opponentDeck;

    private SimpleRandomAI ai;

    private float delay = 0.2f;

    private bool playerFirst = false;

    private bool addJokers = true;

    private bool stockpileRule = true;

    private bool waitForInitialLaneFill = false;

    private bool gameLoop = false;

    private bool playerTurn = false;

    private Lane selectedLane;

    private GamePhase currentPhase = new DrawPhase();

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
            delay = 0.3f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Start setup portion of game
        if (waitForInitialLaneFill)
        {
            if (arenaManager.checkPlayerLanesFilled() && arenaManager.checkOpponentLanesFilled() && checkHandSetup())
            {
                waitForInitialLaneFill = false;
                gameLoop = true;
            }
            else
            {
                handlePlayerSetup();
                handleOpponentSetup();
            }
        }

        // once setup is complete, start game loop
        if (gameLoop)
        {
            if (!currentPhase.Setup)
            {
                if (currentPhase is DrawPhase)
                {
                    selectedLane = null;
                    uiManager.hideHelp();
                    ((DrawPhase)currentPhase).CardManager = cardManager;
                    ((DrawPhase)currentPhase).PlayerTurn = playerTurn;
                    if (playerTurn)
                    {
                        ((DrawPhase)currentPhase).Hand = playerHand;
                        ((DrawPhase)currentPhase).Deck = playerDeck;
                        ((DrawPhase)currentPhase).VisualDeck = visualPlayerDeck;
                    }
                    else
                    {
                        ((DrawPhase)currentPhase).Hand = opponentHand;
                        ((DrawPhase)currentPhase).Deck = opponentDeck;
                        ((DrawPhase)currentPhase).VisualDeck = visualOpponentDeck;
                    }
                }
                else if (currentPhase is AILaneSelectionPhase)
                {
                    ((AILaneSelectionPhase)currentPhase).ArenaManager = arenaManager;
                    ((AILaneSelectionPhase)currentPhase).AI = ai;
                }
                else if(currentPhase is LaneSelectionPhase && !uiManager.tooltipHelp.activeInHierarchy)
                {
                    uiManager.showHelp(HelpDisplay.SELECT_LANE);
                }
                else if (currentPhase is RevealPhase)
                {
                    uiManager.hideHelp();
                    ((RevealPhase)currentPhase).ArenaManager = arenaManager;
                    ((RevealPhase)currentPhase).SelectedLane = selectedLane;
                    ((RevealPhase)currentPhase).StockpileRule = stockpileRule;
                }
                else if (currentPhase is StockpileDrawPhase)
                {
                    ((StockpileDrawPhase)currentPhase).CardManager = cardManager;
                    ((StockpileDrawPhase)currentPhase).PlayerHand = playerHand;
                    ((StockpileDrawPhase)currentPhase).PlayerDeck = playerDeck;
                    ((StockpileDrawPhase)currentPhase).PlayerVisualDeck = visualPlayerDeck;
                    ((StockpileDrawPhase)currentPhase).OpponentHand = opponentHand;
                    ((StockpileDrawPhase)currentPhase).OpponentDeck = opponentDeck;
                    ((StockpileDrawPhase)currentPhase).OpponentVisualDeck = visualOpponentDeck;
                }
                else if (currentPhase is FillPhase)
                {
                    uiManager.showHelp(HelpDisplay.FILL_LANE);
                    ((FillPhase)currentPhase).ArenaManager = arenaManager;
                    ((FillPhase)currentPhase).PlayerHand = playerHand;
                    ((FillPhase)currentPhase).OpponentHand = opponentHand;
                    ((FillPhase)currentPhase).OpponentDeck = opponentDeck;
                    ((FillPhase)currentPhase).AI = ai;
                    playerTurn = !playerTurn;
                }
                else if (currentPhase is EndPhase)
                {
                    uiManager.hideHelp();
                    ((EndPhase)currentPhase).ArenaManager = arenaManager;
                    ((EndPhase)currentPhase).InterfaceManager = uiManager;
                }
                else if (currentPhase is EmptyPhase)
                {
                    gameLoop = false;
                }

                currentPhase.Setup = true;
            }

            currentPhase.execute();

            if (currentPhase.hasCompleted())
            {
                if (currentPhase is LaneSelectionPhase)
                {
                    selectedLane = ((LaneSelectionPhase)currentPhase).SelectedLane;
                }
                currentPhase = currentPhase.getNextPhase();
            }
        }
    }

    public void EndGame()
    {
        // Clear the arena
        arenaManager.Clear();
        playerHand.Clear();
        opponentHand.Clear();
        playerDeck.Clear();
        opponentDeck.Clear();

        currentPhase = new DrawPhase();
        waitForInitialLaneFill = false;
        gameLoop = false;
        selectedLane = null;
        uiManager.gameResultLabel.text = "";
        uiManager.gameResultLabel.color = Color.black;

        //TODO for any coroutines, make sure to interrupt / stop them
        //Can get into an odd state with extra initial hand cards being drawn
    }

    public void NewGame()
    {
        //TODO detect if there is a current game already and ask if player wants to abandon current

        // Reset everything
        EndGame();
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

        uiManager.showHelp(HelpDisplay.SETUP);
    }

    private IEnumerator initialHandDraw()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return StartCoroutine(playerHandDraw(delay));

            yield return StartCoroutine(opponentHandDraw(delay));
        }

        // Flag that hands have been drawn and we can play cards
        waitForInitialLaneFill = true;
    }

    private IEnumerator playerHandDraw(float delay)
    {
        Card playerCard = playerDeck.Draw();
        if (playerCard != null)
        {
            GameObject playerCardGameObject = cardManager.SpawnCard(playerCard);
            if (playerCardGameObject != null)
            {
                playerHand.AddCard(playerCardGameObject);
                visualPlayerDeck.DrawCard();
            }
        }
        yield return new WaitForSeconds(delay);
    }

    private IEnumerator opponentHandDraw(float delay)
    {
        Card opponentCard = opponentDeck.Draw();
        if (opponentCard != null)
        {
            GameObject opponentCardGameObject = cardManager.SpawnCard(opponentCard);
            if (opponentCardGameObject != null)
            {
                opponentHand.AddCard(opponentCardGameObject);
                visualOpponentDeck.DrawCard();
            }
        }
        yield return new WaitForSeconds(delay);
    }

    private void handlePlayerSetup()
    {
        if (arenaManager.checkPlayerLanesFilled() && playerHand.Count < 5)
        {
            StartCoroutine(playerHandDraw(delay * 2));
        }
    }

    private void handleOpponentSetup()
    {
        Lane targetLane = aiPickLane();
        if (targetLane != null && targetLane.OpponentCard == null)
        {
            refreshOpponentLane(targetLane);
        }
        else
        {
            if (arenaManager.checkOpponentLanesFilled() && opponentHand.Count < 5)
            {
                StartCoroutine(opponentHandDraw(delay * 2));
            }
        }
    }

    private Lane aiPickLane()
    {
        Lane targetLane = null;
        int lane = ai.pickLane();
        switch (lane)
        {
            case 0:
                targetLane = arenaManager.LaneA;
                break;
            case 1:
                targetLane = arenaManager.LaneB;
                break;
            case 2:
                targetLane = arenaManager.LaneC;
                break;
            default:
                break;
        }
        return targetLane;
    }

    private void refreshOpponentLane(Lane lane)
    {
        if (lane.OpponentCard == null)
        {
            Card card = ai.pickCard(opponentHand.GetCards());
            opponentHand.selectCard(card);
            GameObject cardGO = opponentHand.GetSelectedCard();
            if (lane.setCard(cardGO, Lane.Slot.OPPONENT))
            {
                arenaManager.placeCard(cardGO, lane.OpponentSlot);
                opponentHand.RemoveCard(cardGO);
            }
        }
    }

    private bool checkHandSetup()
    {
        return playerHand.Count == 5 && opponentHand.Count == 5;
    }
}
