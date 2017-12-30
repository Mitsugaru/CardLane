﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{

    //TODO inject later
    public CardManager cardManager;

    public HighlightManager highlightManager;

    public bool enableInput = true;

    [Range(-0.1f, -0.5f)]
    public float selectionDistance = -0.3f;

    private Quaternion handAngle;

    private float scalingFactor = 10f;

    private List<GameObject> cards;

    private float moveLapse = 1f;

    private float selectLapse = 1f;

    private GameObject selectedCard;

    private readonly IComparer<GameObject> cardCompare = new CardComparer();

    // Use this for initialization
    void Start()
    {
        cards = new List<GameObject>();
        handAngle = Quaternion.Euler(-60f + transform.rotation.eulerAngles.x, 90f + transform.rotation.eulerAngles.y, 0f + transform.rotation.eulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (enableInput && Input.touchSupported)
        {
            handleTouch();
        }
        else if(enableInput)
        {
            handleMouseAndKeyboard();
        }

        // manage card movement
        if (cards.Count > 0)
        {
            if (moveLapse < 1f)
            {
                // foreach card, move them according to size of current hand
                Vector3 targetPosition = new Vector3(0f, 0f, 0f);
                bool flip = false;
                int magnitude = 1;
                float distance = 0.8f;
                //TODO it'd be cool if the order of the cards was correct based on positioning
                for(int i = 0; i < cards.Count; i++)
                {
                    Vector3 currentPosition = cards[i].transform.localPosition;

                    //Also take into account selection
                    if (cards[i].Equals(selectedCard))
                    {
                        targetPosition.x = selectionDistance;
                    }
                    else
                    {
                        targetPosition.x = 0f;
                    }

                    cards[i].transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, moveLapse);

                    if(flip)
                    {
                        targetPosition.z = magnitude * distance;
                        flip = false;
                        magnitude++;
                    }
                    else
                    {
                        targetPosition.z = -magnitude * distance;
                        flip = true;
                    }
                }

                moveLapse += 0.1f;
            }
            if (selectLapse < 1f)
            {
                // foreach card that isn't selected, ensure that their transform is lerped back to "normal"
                for(int i = 0; i < cards.Count; i++)
                {
                    Vector3 currentPosition = cards[i].transform.localPosition;
                    Vector3 targetPosition = cards[i].transform.localPosition;
                    if (cards[i].Equals(selectedCard))
                    {
                        targetPosition.x = selectionDistance;
                    }
                    else
                    {
                        targetPosition.x = 0f;
                    }
                    cards[i].transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, selectLapse);
                }

                selectLapse += 0.1f;
            }
        }
    }

    public void AddCard(GameObject card)
    {
        moveLapse = 0f;
        selectLapse = 0f;

        card.transform.parent = gameObject.transform;
        card.transform.position = gameObject.transform.position;
        card.transform.localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor);
        card.transform.rotation = handAngle;
        card.layer = LayerMask.NameToLayer("Card");
        card.AddComponent<BoxCollider>();

        cards.Add(card);

        cards.Sort(cardCompare);
    }

    public void RemoveCard(GameObject card)
    {
        if (cards.Remove(card))
        {
            cards.Sort(cardCompare);
            moveLapse = 0f;
        }
    }

    public void Clear()
    {
        highlightManager.select(null);
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

    public void ClearSelection()
    {
        selectedCard = null;
        highlightManager.select(null);
        selectLapse = 0f;
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
        GameObject card = detectHandCardHit(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (Input.GetMouseButtonDown(0))
        {
            if (card != null)
            {
                selectCard(card);
            }
            else
            {
                //TODO check if a lane was selected
                //TODO otherwise, deselect hand card
                selectLapse = 0f;
                selectedCard = null;
                highlightManager.select(null);
            }
        }
        else if (card != null && selectedCard == null)
        {
            // add a hover effect on what would be the current selection
            highlightManager.select(card.transform);
        }
        else if(selectedCard == null)
        {
            // remove hover effect
            highlightManager.select(null);
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
        CardScript script = card.GetComponent<CardScript>();
        if (script != null)
        {
            selectedCard = card;
            highlightManager.select(card.transform);
            selectLapse = 0f;
        }
    }
}
