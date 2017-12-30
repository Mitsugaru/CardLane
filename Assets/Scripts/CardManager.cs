using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{

    public AnimationClip cardFlipAnimation;

    public AudioClip cardFlipAudio;

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

            configureCardComponents(spawned, card);
        }
        return spawned;
    }

    public GameObject SpawnRandom()
    {
        GameObject spawned = null;

        List<Card> keys = new List<Card>(cards.Keys);
        Card chosen = keys[random.Next(keys.Count)];
        GameObject goRef = null;
        if (cards.TryGetValue(chosen, out goRef))
        {
            spawned = GameObject.Instantiate(goRef);

            configureCardComponents(spawned, chosen);
        }

        return spawned;
    }

    private void configureCardComponents(GameObject go, Card card)
    {
        CardScript script = go.AddComponent<CardScript>();
        script.Card = card;

        Animation animation = go.AddComponent<Animation>();
        animation.clip = cardFlipAnimation;
        animation.playAutomatically = false;

        AudioSource audio = go.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.clip = cardFlipAudio;
    }
}
