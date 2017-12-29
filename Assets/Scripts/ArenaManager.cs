using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{

    //TODO inject this later
    public CardManager cardManager;

    public Lane deckArea;

    public Lane LaneA;

    public Lane LaneB;

    public Lane LaneC;

    private GameObject playerDeck;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        while (playerDeck == null)
        {
            playerDeck = cardManager.SpawnCard(new Card(Rank.JOKER, Suit.NONE));
            if (playerDeck != null)
            {
                placeCard(playerDeck, deckArea.PlayerSlot);
            }

            GameObject opponentDeck = cardManager.SpawnCard(new Card(Rank.JOKER, Suit.NONE));
            if (opponentDeck != null)
            {
                placeCard(opponentDeck, deckArea.OpponentSlot);
            }

            List<Transform> placements = new List<Transform>();
            placements.Add(LaneA.PlayerSlot);
            placements.Add(LaneA.OpponentSlot);
            placements.Add(LaneB.PlayerSlot);
            placements.Add(LaneB.OpponentSlot);
            placements.Add(LaneC.PlayerSlot);
            placements.Add(LaneC.OpponentSlot);

            foreach (Transform placement in placements)
            {
                GameObject card = cardManager.SpawnRandom();

                if (card != null)
                {
                    placeCard(card, placement);
                }
            }
        }
    }

    private void placeCard(GameObject card, Transform parent)
    {
        float scalingFactor = 25f;
        card.transform.parent = parent;
        card.transform.position = parent.position;
        card.transform.localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor);
        card.transform.rotation = Quaternion.Euler(-90f, 90f, 0f);
    }
}
