using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Transform PlayerSlot;

    public Transform OpponentSlot;

    public Transform playerStockpile;

    public Transform opponentStockpile;

    private GameObject playerCard;

    public GameObject PlayerCard
    {
        get
        {
            return playerCard;
        }
    }

    private GameObject opponentCard;

    public GameObject OpponentCard
    {
        get
        {
            return opponentCard;
        }
    }

    public bool setCard(GameObject card, Slot slot)
    {
        bool set = false;

        switch(slot)
        {
            case Slot.PLAYER:
                if (playerCard == null)
                {
                    playerCard = card;
                    card.transform.parent = PlayerSlot;
                    set = true;
                }
                break;
            case Slot.OPPONENT:
                if(opponentCard == null)
                {
                    opponentCard = card;
                    card.transform.parent = OpponentSlot;
                    set = true;
                }
                break;
            default:
                break;
        }

        return set;
    }

    public GameObject removeCard(Slot slot)
    {
        GameObject card = null;

        switch (slot)
        {
            case Slot.PLAYER:
                card = playerCard;
                playerCard = null;
                break;
            case Slot.OPPONENT:
                card = opponentCard;
                opponentCard = null;
                break;
            default:
                break;
        }

        return card;
    }

    public enum Slot
    {
        PLAYER,
        OPPONENT
    }
}
