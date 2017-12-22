using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour {

    public CardIDPair[] ids;

    private Dictionary<Card, GameObject> cards = new Dictionary<Card, GameObject>();

	// Use this for initialization
	void Start () {
		foreach(Suit suit in Suit.Values)
        {
            foreach(Rank rank in Rank.Values)
            {
                Card card = new Card(rank, suit);

                foreach(CardIDPair id in ids)
                {
                    if (card.ToString().Equals(id.cardId))
                    {
                        cards.Add(card, id.gameObject);
                        Debug.Log("Loaded '" + card.ToString() + "' using " + id.gameObject.name);
                        break;
                    }
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
