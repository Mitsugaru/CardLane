using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{

    //TODO inject later
    public CardManager cardManager;

    private GameObject heldCard;

    //TODO manage the rotation of cards based on number in hand
    //TODO lerp / animate

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        while (heldCard == null)
        {
            heldCard = cardManager.SpawnRandom();

            if (heldCard != null)
            {
                float scalingFactor = 10f;
                heldCard.transform.parent = gameObject.transform;
                heldCard.transform.position = gameObject.transform.position;
                heldCard.transform.localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor);
                heldCard.transform.rotation = Quaternion.Euler(-60f, 90f, 0f);
            }
        }
    }
}
