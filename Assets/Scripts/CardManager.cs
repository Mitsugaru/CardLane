using DakaniLabs.CardLane.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CardManager : MonoBehaviour
{

    public AnimationClip cardFlipAnimation;

    public AudioClip cardFlipAudio;

    public AudioMixerGroup mixerGroup;

    public CardIDPair[] ids;

    private bool readyFlag = false;

    public bool Ready
    {
        get
        {
            return readyFlag;
        }
    }

    private Dictionary<PlayingCard, GameObject> cards = new Dictionary<PlayingCard, GameObject>();

    private System.Random random = new System.Random();

    // Use this for initialization
    void Start()
    {
        foreach (Suit suit in Suit.Values)
        {
            foreach (Rank rank in Rank.Values)
            {
                PlayingCard card = new PlayingCard(rank, suit);

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
        readyFlag = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject SpawnCard(PlayingCard card)
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

        List<PlayingCard> keys = new List<PlayingCard>(cards.Keys);
        PlayingCard chosen = keys[random.Next(keys.Count)];
        GameObject goRef = null;
        if (cards.TryGetValue(chosen, out goRef))
        {
            spawned = GameObject.Instantiate(goRef);

            configureCardComponents(spawned, chosen);
        }

        return spawned;
    }

    private void configureCardComponents(GameObject go, PlayingCard card)
    {
        CardScript script = go.AddComponent<CardScript>();
        script.Card = card;

        Animation animation = go.AddComponent<Animation>();
        animation.AddClip(cardFlipAnimation, "CardFlip");
        animation.playAutomatically = false;

        AudioSource audio = go.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.clip = cardFlipAudio;
        audio.outputAudioMixerGroup = mixerGroup;
    }
}
