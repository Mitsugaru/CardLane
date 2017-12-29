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
                heldCard.layer = LayerMask.NameToLayer("Card");
                heldCard.AddComponent<BoxCollider>();
            }
        }

        if (Input.touchSupported)
        {
            handleTouch();
        }
        else
        {
            handleMouseAndKeyboard();
        }
    }

    private void handleTouch()
    {
        // Look for all fingers
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
        }
    }

    private void handleMouseAndKeyboard()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject card = detectHandCardHit(Camera.main.ScreenPointToRay(Input.mousePosition));
            if(card != null)
            {
                string cardName = card.name;
                CardScript script = card.GetComponent<CardScript>();
                if (script != null)
                {
                    cardName = script.Card.ToString();
                }
                Debug.Log("Clicked card: " + cardName);
            }
        }
    }

    private GameObject detectHandCardHit(Ray ray)
    {
        GameObject target = null;

        RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue, LayerMask.GetMask("Card"));
        for (int i = 0; i < hits.Length; i++)
        {
            //Check that the card touched has parent transform of hand
            if (hits[i].transform.parent.Equals(gameObject.transform))
            {
                target = hits[i].transform.gameObject;
                break;
            }
        }

        return target;
    }
}
