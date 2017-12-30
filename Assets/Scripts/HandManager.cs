using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{

    //TODO inject later
    public CardManager cardManager;

    private Quaternion handAngle;

    private float scalingFactor = 10f;

    private List<GameObject> cards;

    private float moveLapse = 1f;

    private float selectLapse = 1f;

    private GameObject selectedCard;

    //TODO manage the rotation of cards based on number in hand
    //TODO lerp / animate

    // Use this for initialization
    void Start()
    {
        cards = new List<GameObject>();
        handAngle = Quaternion.Euler(-60f + transform.rotation.eulerAngles.x, 90f + transform.rotation.eulerAngles.y, 0f + transform.rotation.eulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchSupported)
        {
            handleTouch();
        }
        else
        {
            handleMouseAndKeyboard();
        }

        //TODO manage card movement
        if (cards.Count > 0)
        {
            if (moveLapse < 1f)
            {
                //TODO foreach card, move them according to size of current hand
                moveLapse += 0.1f;
            }
            if (selectLapse < 1f)
            {
                //TODO foreach card that isn't selected, ensure that their transform is lerped back to "normal"
            }
        }
    }

    public void AddCard(GameObject card)
    {
        moveLapse = 0f;

        card.transform.parent = gameObject.transform;
        card.transform.position = gameObject.transform.position;
        card.transform.localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor);
        card.transform.rotation = handAngle;
        card.layer = LayerMask.NameToLayer("Card");
        card.AddComponent<BoxCollider>();
    }

    public void RemoveCard(GameObject card)
    {
        if (cards.Remove(card))
        {
            moveLapse = 0f;
        }
    }

    public void Clear()
    {
        moveLapse = 1f;
        selectLapse = 1f;
        cards.Clear();
        selectedCard = null;
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        transform.DetachChildren();
    }

    public GameObject GetSelectedCard()
    {
        return selectedCard;
    }

    private void handleTouch()
    {
        bool hitHand = false;
        // Look for all fingers
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            GameObject card = detectHandCardHit(Camera.main.ScreenPointToRay(touch.position));
            if (card != null)
            {
                selectCard(card);
                hitHand = true;
            }
        }

        if (!hitHand)
        {
            //TODO if hand card was already selected, check if a lane was selected
            //TODO handle hand deselection here
        }
    }

    private void handleMouseAndKeyboard()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject card = detectHandCardHit(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (card != null)
            {
                selectCard(card);
            }
            else
            {
                //TODO check if a lane was selected
                //TODO otherwise, deselect hand card
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

    private void selectCard(GameObject card)
    {
        //TODO select the card
        string cardName = card.name;
        CardScript script = card.GetComponent<CardScript>();
        if (script != null)
        {
            cardName = script.Card.ToString();

            selectedCard = card;

            //TODO selectLapse = 0f;
        }
        Debug.Log("Clicked card: " + cardName);
    }
}
