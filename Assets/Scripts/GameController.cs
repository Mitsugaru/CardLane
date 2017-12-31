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

    private bool drawPhase = false;

    private bool laneSelectionPhase = false;

    private bool mouseDown = false;

    private float selectionTime = 0f;

    private Lane selectedLane;

    private bool revealPhase = false;

    private float waitTime = 0f;

    private bool fillPhase = false;

    private bool endState = false;

    private bool resolveCards = false;

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

        // Start setup portion of game
        if (waitForInitialLaneFill)
        {
            if (checkPlayerLanesFilled() && checkOpponentLanesFilled() && checkHandSetup())
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
                            animateCard(selectedLane.PlayerCard);
                            animateCard(selectedLane.OpponentCard);
                            laneSelectionPhase = false;
                            revealPhase = true;
                        }
                    }
                }

                if (revealPhase)
                {
                    // after animation resolve
                    if (!isCardPlaying(selectedLane.PlayerCard)
                        && !isCardPlaying(selectedLane.OpponentCard))
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
                        else
                        {
                            //TODO handle stockpile maneuver here
                            //TODO move cards and reset local rotation (it'd be cool to do a rotate animation)
                        }

                        //TODO it'd be cool to have a fade animation
                        // remove cards from lane slots
                        GameObject.Destroy(selectedLane.removeCard(Lane.Slot.PLAYER));
                        GameObject.Destroy(selectedLane.removeCard(Lane.Slot.OPPONENT));
                        //TODO remove cards from stockpile

                        revealPhase = false;
                        fillPhase = true;
                        waitTime = 0f;
                    }
                }

                if (fillPhase)
                {
                    // wait for player lane to be filled
                    if ((checkPlayerLanesFilled() && checkOpponentLanesFilled())
                        || (playerHand.Count == 0 && opponentHand.Count == 0))
                    {
                        fillPhase = false;
                        playerTurn = false;
                        drawPhase = true;
                        selectedLane = null;
                    }
                    else if (!checkOpponentLanesFilled())
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
                        animateCard(selectedLane.PlayerCard);
                        animateCard(selectedLane.OpponentCard);
                        laneSelectionPhase = false;
                        revealPhase = true;
                    }
                }

                if (revealPhase)
                {
                    // after animation resolve
                    if (!isCardPlaying(selectedLane.PlayerCard)
                        && !isCardPlaying(selectedLane.OpponentCard))
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
                        else
                        {
                            //TODO handle stockpile maneuver here
                            //TODO move cards and reset local rotation (it'd be cool to do a rotate animation)
                        }

                        //TODO it'd be cool to have a fade animation
                        // remove cards from lane slots
                        GameObject.Destroy(selectedLane.removeCard(Lane.Slot.PLAYER));
                        GameObject.Destroy(selectedLane.removeCard(Lane.Slot.OPPONENT));
                        //TODO remove cards from stockpile

                        revealPhase = false;
                        fillPhase = true;
                        waitTime = 0f;
                    }
                }

                if (fillPhase)
                {
                    // wait for player lane to be filled
                    if ((checkPlayerLanesFilled() && checkOpponentLanesFilled())
                        || (playerHand.Count == 0 && opponentHand.Count == 0))
                    {
                        fillPhase = false;
                        playerTurn = true;
                        drawPhase = true;
                        selectedLane = null;
                    }
                    else if (!checkOpponentLanesFilled())
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
                //TODO reveal and resolve the remaining cards
                // on the table
                if (laneSelectionPhase)
                {
                    foreach (Lane lane in arenaManager.getLanes())
                    {
                        if (lane.PlayerCard != null
                            && lane.OpponentCard != null)
                        {
                            selectedLane = lane;
                            animateCard(selectedLane.PlayerCard);
                            animateCard(selectedLane.OpponentCard);
                            laneSelectionPhase = false;
                            revealPhase = true;
                            break;
                        }
                    }
                }

                if (revealPhase)
                {
                    // after animation resolve
                    if (!isCardPlaying(selectedLane.PlayerCard)
                        && !isCardPlaying(selectedLane.OpponentCard))
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
                        //TODO remove cards from stockpile

                        revealPhase = false;
                        laneSelectionPhase = true;
                        selectedLane = null;
                        waitTime = 0f;
                    }
                }

                if (selectedLane == null)
                {
                    //TODO true end, figure out winner
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

    public void NewGame()
    {
        //TODO detect if there is a current game already and ask if player wants to abandon current

        // Clear the arena
        arenaManager.Clear();
        playerHand.Clear();
        opponentHand.Clear();
        visualPlayerDeck.SpawnDeck();
        visualOpponentDeck.SpawnDeck();

        // Reset flags
        waitForInitialLaneFill = false;
        gameLoop = false;
        resolveCards = false;
        drawPhase = false;
        mouseDown = false;
        laneSelectionPhase = false;
        revealPhase = false;
        fillPhase = false;
        selectedLane = null;
        endState = false;
        selectionTime = 0f;
        waitTime = 0f;

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
        GameObject playerCardGameObject = cardManager.SpawnCard(playerCard);
        playerHand.AddCard(playerCardGameObject);
        visualPlayerDeck.DrawCard();
        yield return new WaitForSeconds(delay);
    }

    private IEnumerator opponentHandDraw(float delay)
    {
        Card opponentCard = opponentDeck.Draw();
        GameObject opponentCardGameObject = cardManager.SpawnCard(opponentCard);
        opponentHand.AddCard(opponentCardGameObject);
        visualOpponentDeck.DrawCard();
        yield return new WaitForSeconds(delay);
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
            if (checkOpponentLanesFilled() && opponentHand.Count < 5)
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

    private void animateCard(GameObject card)
    {
        Animation animation = card.GetComponent<Animation>();
        if (animation != null)
        {
            animation.Play("CardFlip");
        }
        AudioSource audio = card.GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.Play();
        }
    }

    private bool isCardPlaying(GameObject card)
    {
        bool playing = false;

        Animation animation = card.GetComponent<Animation>();
        if (animation != null)
        {
            playing = animation.IsPlaying("CardFlip");
        }
        AudioSource audio = card.GetComponent<AudioSource>();
        if (audio != null && !playing)
        {
            playing = audio.isPlaying;
        }

        return playing;
    }
}
