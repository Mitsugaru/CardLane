using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckScript : MonoBehaviour
{

    public GameObject card;

    public int limit = 27;

    public AudioSource audioSource;

    public AudioClip[] drawSounds;

    private List<GameObject> spawnedCards = new List<GameObject>();

    private System.Random random = new System.Random();

    // Use this for initialization
    void Start()
    {
        //Generate card pool
        float height = 0f;
        Vector3 cardScale = new Vector3(25f, 25f, 25f);
        for (int i = 0; i < limit; i++)
        {
            GameObject spawn = GameObject.Instantiate(card);
            spawn.SetActive(false);
            spawn.transform.SetParent(gameObject.transform);
            spawn.transform.position = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
            spawn.transform.rotation = Quaternion.Euler(-90f, 90f, 0f);
            spawn.transform.localScale = cardScale;
            spawnedCards.Add(spawn);
            height += 0.01f;
        }
    }

    public void SpawnDeck()
    {
        // Deactivate cards and reposition them
        foreach (GameObject card in spawnedCards)
        {
            card.SetActive(true);
        }
    }

    public void DrawCard()
    {
        List<GameObject> cards = new List<GameObject>(spawnedCards);
        cards.Reverse();
        foreach (GameObject card in cards)
        {
            if (card.activeSelf)
            {
                card.SetActive(false);
                playSound();
                break;
            }
        }
    }

    private void playSound()
    {
        if(audioSource != null && drawSounds.Length > 0)
        {
            audioSource.PlayOneShot(drawSounds[random.Next(drawSounds.Length)]);
        }
    }

    // TODO method to change limit programmatically and adjust the pool of gameobjects

    // Update is called once per frame
    void Update()
    {

    }
}
