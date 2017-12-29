using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckScript : MonoBehaviour
{

    public GameObject card;

    public int limit = 26;

    private List<GameObject> spawnedCards = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        //Generate card pool
        float height = 0f;
        Vector3 cardScale = new Vector3(25f, 25f, 25f);
        for (int i = 0; i < limit; i++)
        {
            GameObject spawn = GameObject.Instantiate(card);
            spawn.transform.SetParent(gameObject.transform);
            spawn.transform.position = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
            spawn.transform.rotation = Quaternion.Euler(-90f, 90f, 0f);
            spawn.transform.localScale = cardScale;
            spawnedCards.Add(spawn);
            height += 0.01f;
        }
    }

    public void spawnDeck()
    {
        // Deactivate cards and reposition them
        foreach (GameObject card in spawnedCards)
        {
            card.SetActive(true);
        }
    }

    public void drawCard()
    {
        List<GameObject> cards = new List<GameObject>(spawnedCards);
        cards.Reverse();
        foreach (GameObject card in cards)
        {
            if (card.activeSelf)
            {
                card.SetActive(false);
                break;
            }
        }
    }

    // TODO method to change limit programmatically and adjust the pool of gameobjects

    // Update is called once per frame
    void Update()
    {

    }
}
