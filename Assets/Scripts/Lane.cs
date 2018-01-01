using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lane : MonoBehaviour
{
    public Transform PlayerSlot;

    public Transform OpponentSlot;

    public Transform playerStockpile;

    public Transform opponentStockpile;

    public Text playerLabel;

    public Text opponentLabel;

    public int playerPoints = 0;

    public int opponentPoints = 0;

    private int currentPlayerPoints = -1;

    private int currentOpponentPoints = -1;

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

    void Update()
    {
        if (currentPlayerPoints != playerPoints && playerLabel != null)
        {
            currentPlayerPoints = playerPoints;
            if (currentPlayerPoints != 0)
            {
                playerLabel.text = currentPlayerPoints.ToString();
            }
            else
            {
                playerLabel.text = "";
            }
        }
        if (currentOpponentPoints != opponentPoints && opponentLabel != null)
        {
            currentOpponentPoints = opponentPoints;
            if (currentOpponentPoints != 0)
            {
                opponentLabel.text = currentOpponentPoints.ToString();
            }
            else
            {
                opponentLabel.text = "";
            }
        }
    }

    public bool setCard(GameObject card, Slot slot)
    {
        bool set = false;

        switch (slot)
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
                if (opponentCard == null)
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

    public void Clear()
    {
        playerCard = null;
        opponentCard = null;

        playerPoints = 0;
        opponentPoints = 0;
        currentPlayerPoints = -1;
        currentOpponentPoints = -1;
    }

    public enum Slot
    {
        PLAYER,
        OPPONENT
    }
}
