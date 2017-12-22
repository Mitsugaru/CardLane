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
        float height = 0.02f;
        Vector3 cardScale = new Vector3(1f, 1f, 1f);
        height += 0.02f;
        for (int i = 0; i < limit; i++)
        {
            GameObject spawn = GameObject.Instantiate(card);
            spawn.transform.SetParent(gameObject.transform);
            spawn.transform.position = gameObject.transform.position;
            spawn.transform.rotation = gameObject.transform.rotation;
            spawn.transform.localScale = cardScale;
            spawnedCards.Add(spawn);
        }
    }

    public void spawnDeck()
    {
        // Deactivate cards and reposition them
        foreach (GameObject card in spawnedCards)
        {
            card.SetActive(true);
        }

        spawnedCards.Reverse();
    }

    public void drawCard()
    {
        foreach (GameObject card in spawnedCards)
        {
            if (card.activeSelf)
            {
                card.SetActive(false);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
