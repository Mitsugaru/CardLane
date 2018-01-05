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

    private bool drawPhase = false;

    private bool laneSelectionPhase = false;

    private bool mouseDown = false;

    private float selectionTime = 0f;

    private Lane selectedLane;

    private bool revealPhase = false;

    private float waitTime = 0f;

    private bool stockpileDrawPhase = false;

    private bool fillPhase = false;

    private bool endState = false;

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
                drawPhase = true;
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
            if (playerTurn && !endState)
            {
                if (drawPhase)
                {
                    if (playerHand.Count < 5 && playerDeck.Deck.Count > 0)
                    {
                        StartCoroutine(playerHandDraw(delay * 2));
                    }
                    else
                    {
                        drawPhase = false;
                        laneSelectionPhase = true;
                    }
                }

                if (laneSelectionPhase)
                {
                    // make a lane selection
                    GameObject laneGo = null;
                    bool hadInput = false;
                    if (Input.touchSupported)
                    {
                        // Look for all fingers
                        for (int i = 0; i < Input.touchCount; i++)
                        {
                            Touch touch = Input.GetTouch(i);
                            Ray ray = Camera.main.ScreenPointToRay(touch.position);

                            if (HitUtils.detectHitLane(ray, out laneGo))
                            {
                                selectionTime += Time.deltaTime;
                                hadInput = true;
                            }
                        }

                        if (Input.touchCount == 0)
                        {
                            //Touch was released, stop counting
                            selectionTime = 0f;
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            selectionTime = 0f;
                            mouseDown = false;
                        }
                        if (Input.GetMouseButtonDown(0))
                        {
                            mouseDown = true;
                        }
                        if (mouseDown && Input.GetMouseButton(0))
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                            if (HitUtils.detectHitLane(ray, out laneGo))
                            {
                                selectionTime += Time.deltaTime;
                                hadInput = true;
                            }
                        }
                    }

                    if (hadInput && selectionTime >= 1f)
                    {
                        selectionTime = 0f;
                        mouseDown = false;

                        selectedLane = laneGo.GetComponent<Lane>();
                        if (selectedLane != null
                            && selectedLane.PlayerCard != null
                            && selectedLane.OpponentCard != null)
                        {
                            // animate the cards
                            CardUtils.animateCard(selectedLane.PlayerCard);
                            CardUtils.animateCard(selectedLane.OpponentCard);
                            laneSelectionPhase = false;
                            revealPhase = true;
                        }
                    }
                }

                if (revealPhase)
                {
                    // after animation resolve
                    if (!CardUtils.isCardPlaying(selectedLane.PlayerCard)
                        && !CardUtils.isCardPlaying(selectedLane.OpponentCard))
                    {
                        waitTime += Time.deltaTime;
                    }

                    if (waitTime >= 1.5f)
                    {
                        bool destroy = true;
                        // resolve
                        CardScript playerCardScript = selectedLane.PlayerCard.GetComponent<CardScript>();
                        CardScript opponentCardScript = selectedLane.OpponentCard.GetComponent<CardScript>();
                        int cardResolve = CardRuleUtils.Resolve(playerCardScript.Card, opponentCardScript.Card);

                        if (cardResolve > 0)
                        {
                            selectedLane.playerPoints += selectedLane.playerStockpile.childCount + 1;
                        }
                        else if (cardResolve < 0)
                        {
                            selectedLane.opponentPoints += selectedLane.opponentStockpile.childCount + 1;
                        }
                        else if (stockpileRule)
                        {
                            // handle stockpile maneuver here
                            stockpileDrawPhase = true;
                            destroy = false;
                            arenaManager.placeStockpile(selectedLane.removeCard(Lane.Slot.PLAYER), selectedLane.playerStockpile);
                            arenaManager.placeStockpile(selectedLane.removeCard(Lane.Slot.OPPONENT), selectedLane.opponentStockpile);
                        }

                        //TODO it'd be cool to have a fade animation
                        // remove cards from lane slots
                        if (destroy)
                        {
                            GameObject.Destroy(selectedLane.removeCard(Lane.Slot.PLAYER));
                            GameObject.Destroy(selectedLane.removeCard(Lane.Slot.OPPONENT));
                            foreach (Transform child in selectedLane.playerStockpile)
                            {
                                GameObject.Destroy(child.gameObject);
                            }
                            foreach (Transform child in selectedLane.opponentStockpile)
                            {
                                GameObject.Destroy(child.gameObject);
                            }
                            fillPhase = true;
                        }

                        revealPhase = false;
                        waitTime = 0f;
                    }
                }

                if (stockpileDrawPhase)
                {
                    bool playerReady = false;
                    bool opponentReady = false;
                    if (playerHand.Count < 5 && playerDeck.Deck.Count > 0)
                    {
                        StartCoroutine(playerHandDraw(delay * 2));
                    }
                    else
                    {
                        playerReady = true;
                    }
                    if (opponentHand.Count < 5 && opponentDeck.Deck.Count > 0)
                    {
                        StartCoroutine(opponentHandDraw(delay * 2));
                    }
                    else
                    {
                        opponentReady = true;
                    }

                    if (playerReady && opponentReady)
                    {
                        stockpileDrawPhase = false;
                        fillPhase = true;
                    }
                }

                if (fillPhase)
                {
                    // wait for player lane to be filled
                    if ((arenaManager.checkPlayerLanesFilled() && arenaManager.checkOpponentLanesFilled())
                        || (playerHand.Count == 0 && opponentHand.Count == 0))
                    {
                        fillPhase = false;
                        playerTurn = false;
                        drawPhase = true;
                        selectedLane = null;
                    }
                    else if (!arenaManager.checkOpponentLanesFilled())
                    {
                        // have AI fill lane
                        foreach (Lane lane in arenaManager.getLanes())
                        {
                            if (lane.OpponentCard == null)
                            {
                                refreshOpponentLane(lane);
                            }
                        }
                    }
                }
            }
            else if (!endState)
            {
                //AI opponent turn
                if (drawPhase)
                {
                    if (opponentHand.Count < 5 && opponentDeck.Deck.Count > 0)
                    {
                        StartCoroutine(opponentHandDraw(delay * 2));
                    }
                    else
                    {
                        drawPhase = false;
                        laneSelectionPhase = true;
                    }
                }

                if (laneSelectionPhase)
                {
                    selectedLane = aiPickLane();
                    if (selectedLane != null
                        && selectedLane.PlayerCard != null
                        && selectedLane.OpponentCard != null)
                    {
                        CardUtils.animateCard(selectedLane.PlayerCard);
                        CardUtils.animateCard(selectedLane.OpponentCard);
                        laneSelectionPhase = false;
                        revealPhase = true;
                    }
                }

                if (revealPhase)
                {
                    // after animation resolve
                    if (!CardUtils.isCardPlaying(selectedLane.PlayerCard)
                        && !CardUtils.isCardPlaying(selectedLane.OpponentCard))
                    {
                        waitTime += Time.deltaTime;
                    }

                    if (waitTime >= 1.5f)
                    {
                        bool destroy = true;
                        // resolve
                        CardScript playerCardScript = selectedLane.PlayerCard.GetComponent<CardScript>();
                        CardScript opponentCardScript = selectedLane.OpponentCard.GetComponent<CardScript>();
                        int cardResolve = CardRuleUtils.Resolve(playerCardScript.Card, opponentCardScript.Card);

                        if (cardResolve > 0)
                        {
                            selectedLane.playerPoints += selectedLane.playerStockpile.childCount + 1;
                        }
                        else if (cardResolve < 0)
                        {
                            selectedLane.opponentPoints += selectedLane.opponentStockpile.childCount + 1;
                        }
                        else if (stockpileRule)
                        {
                            destroy = false;
                            stockpileDrawPhase = true;
                            arenaManager.placeStockpile(selectedLane.removeCard(Lane.Slot.PLAYER), selectedLane.playerStockpile);
                            arenaManager.placeStockpile(selectedLane.removeCard(Lane.Slot.OPPONENT), selectedLane.opponentStockpile);
                        }

                        //TODO it'd be cool to have a fade animation
                        // remove cards from lane slots
                        if (destroy)
                        {
                            GameObject.Destroy(selectedLane.removeCard(Lane.Slot.PLAYER));
                            GameObject.Destroy(selectedLane.removeCard(Lane.Slot.OPPONENT));
                            foreach (Transform child in selectedLane.playerStockpile)
                            {
                                GameObject.Destroy(child.gameObject);
                            }
                            foreach (Transform child in selectedLane.opponentStockpile)
                            {
                                GameObject.Destroy(child.gameObject);
                            }

                            fillPhase = true;
                        }

                        revealPhase = false;
                        waitTime = 0f;
                    }
                }

                if (stockpileDrawPhase)
                {
                    bool playerReady = false;
                    bool opponentReady = false;
                    if (playerHand.Count < 5 && playerDeck.Deck.Count > 0)
                    {
                        StartCoroutine(playerHandDraw(delay * 2));
                    }
                    else
                    {
                        playerReady = true;
                    }
                    if (opponentHand.Count < 5 && opponentDeck.Deck.Count > 0)
                    {
                        StartCoroutine(opponentHandDraw(delay * 2));
                    }
                    else
                    {
                        opponentReady = true;
                    }

                    if (playerReady && opponentReady)
                    {
                        stockpileDrawPhase = false;
                        fillPhase = true;
                    }
                }

                if (fillPhase)
                {
                    // wait for player lane to be filled
                    if ((arenaManager.checkPlayerLanesFilled() && arenaManager.checkOpponentLanesFilled())
                        || (playerHand.Count == 0 && opponentHand.Count == 0))
                    {
                        fillPhase = false;
                        playerTurn = true;
                        drawPhase = true;
                        selectedLane = null;
                    }
                    else if (!arenaManager.checkOpponentLanesFilled())
                    {
                        // have AI fill lane
                        foreach (Lane lane in arenaManager.getLanes())
                        {
                            if (lane.OpponentCard == null)
                            {
                                refreshOpponentLane(lane);
                            }
                        }
                    }
                }
            }
            else
            {
                // reveal and resolve the remaining cards
                // on the table
                if (laneSelectionPhase)
                {
                    foreach (Lane lane in arenaManager.getLanes())
                    {
                        if (lane.PlayerCard != null
                            && lane.OpponentCard != null)
                        {
                            selectedLane = lane;
                            CardUtils.animateCard(selectedLane.PlayerCard);
                            CardUtils.animateCard(selectedLane.OpponentCard);
                            laneSelectionPhase = false;
                            revealPhase = true;
                            break;
                        }
                    }
                }

                if (revealPhase)
                {
                    // after animation resolve
                    if (!CardUtils.isCardPlaying(selectedLane.PlayerCard)
                        && !CardUtils.isCardPlaying(selectedLane.OpponentCard))
                    {
                        waitTime += Time.deltaTime;
                    }

                    if (waitTime >= 1.5f)
                    {
                        // resolve
                        CardScript playerCardScript = selectedLane.PlayerCard.GetComponent<CardScript>();
                        CardScript opponentCardScript = selectedLane.OpponentCard.GetComponent<CardScript>();
                        int cardResolve = CardRuleUtils.Resolve(playerCardScript.Card, opponentCardScript.Card);

                        if (cardResolve > 0)
                        {
                            selectedLane.playerPoints++;
                        }
                        else if (cardResolve < 0)
                        {
                            selectedLane.opponentPoints++;
                        }
                        // Discard draws because its the end state

                        //TODO it'd be cool to have a fade animation
                        // remove cards from lane slots
                        GameObject.Destroy(selectedLane.removeCard(Lane.Slot.PLAYER));
                        GameObject.Destroy(selectedLane.removeCard(Lane.Slot.OPPONENT));
                        foreach (Transform child in selectedLane.playerStockpile)
                        {
                            GameObject.Destroy(child.gameObject);
                        }
                        foreach (Transform child in selectedLane.opponentStockpile)
                        {
                            GameObject.Destroy(child.gameObject);
                        }

                        revealPhase = false;
                        laneSelectionPhase = true;
                        selectedLane = null;
                        waitTime = 0f;
                    }
                }
                else
                {
                    //true end, figure out winner
                    int playerLanes = 0;
                    int opponentLanes = 0;

                    foreach (Lane lane in arenaManager.getLanes())
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
                        uiManager.gameResultLabel.text = "You Win";
                        uiManager.gameResultLabel.color = Color.green;
                    }
                    else if (opponentLanes > playerLanes)
                    {
                        uiManager.gameResultLabel.text = "You Lose";
                        uiManager.gameResultLabel.color = Color.red;
                    }
                    else
                    {
                        uiManager.gameResultLabel.text = "Draw";
                        uiManager.gameResultLabel.color = Color.black;
                    }

                    gameLoop = false;
                }
            }

            // check if the player hands still have cards to play
            if (!endState && playerHand.Count == 0 && opponentHand.Count == 0)
            {
                endState = true;
                laneSelectionPhase = true;
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

        waitForInitialLaneFill = false;
        gameLoop = false;
        drawPhase = false;
        mouseDown = false;
        laneSelectionPhase = false;
        stockpileDrawPhase = false;
        revealPhase = false;
        fillPhase = false;
        selectedLane = null;
        endState = false;
        selectionTime = 0f;
        waitTime = 0f;
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
