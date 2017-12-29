using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{

    public CardIDPair[] ids;

    private Dictionary<Card, GameObject> cards = new Dictionary<Card, GameObject>();

    private System.Random random = new System.Random();

    // Use this for initialization
    void Start()
    {
        foreach (Suit suit in Suit.Values)
        {
            foreach (Rank rank in Rank.Values)
            {
                Card card = new Card(rank, suit);

                foreach (CardIDPair id in ids)
                {
                    if (card.ToString().Equals(id.cardId))
                    {
                        cards.Add(card, id.gameObject);
                        break;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject SpawnCard(Card card)
    {
        GameObject spawned = null;

        GameObject cardGORef;
        if (cards.TryGetValue(card, out cardGORef))
        {
            spawned = GameObject.Instantiate(cardGORef);
        }
        return spawned;
    }

    public GameObject SpawnRandom()
    {
        GameObject spawned = null;

        List<Card> keys = new List<Card>(cards.Keys);
        GameObject goRef = null;
        if (cards.TryGetValue(keys[random.Next(keys.Count)], out goRef))
        {
            spawned = GameObject.Instantiate(goRef);
        }

        return spawned;
    }
}
