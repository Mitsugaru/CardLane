using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DakaniLabs.CardLane.Card
{
    public class CardComparer : IComparer<GameObject>
    {

        public int Compare(GameObject x, GameObject y)
        {
            int result = 0;
            CardScript xScript = x.GetComponent<CardScript>();
            CardScript yScript = y.GetComponent<CardScript>();

            if (xScript != null && yScript != null)
            {
                result = xScript.Card.Rank - yScript.Card.Rank;
                if (result == 0)
                {
                    result = xScript.Card.Suit - yScript.Card.Suit;
                }
            }
            else
            {
                if (!x.Equals(y))
                {
                    result = x.name.CompareTo(y.name);
                }
            }

            return result;
        }

    }
}