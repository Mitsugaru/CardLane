﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DakaniLabs.CardLane.Card
{
    public class CardUtils
    {
        public static IList<Card> generateCardsForSuit(Suit suit)
        {
            IList<Card> cards = new List<Card>();

            foreach (Rank rank in Rank.Values)
            {
                if (rank != Rank.JOKER)
                {
                    cards.Add(new Card(rank, suit));
                }
            }

            return cards;
        }

        public static void populateCardLists(List<Card> redList, List<Card> blackList)
        {
            redList.AddRange(generateCardsForSuit(Suit.DIAMONDS));
            redList.AddRange(generateCardsForSuit(Suit.HEARTS));
            blackList.AddRange(generateCardsForSuit(Suit.CLUBS));
            blackList.AddRange(generateCardsForSuit(Suit.SPADES));

            adjustCardList(redList);
            adjustCardList(blackList);
        }

        private static void adjustCardList(List<Card> list)
        {
            List<Card> toReplace = new List<Card>();
            foreach (Card card in list)
            {
                if (card.Rank == Rank.TEN)
                {
                    toReplace.Add(card);
                }
            }
            foreach (Card replace in toReplace)
            {
                Suit suit = replace.Suit;
                list.Remove(replace);
                list.Add(new Card(Rank.JACK, suit));
            }
        }

        public static void animateCard(GameObject card)
        {
            Animation animation = card.GetComponent<Animation>();
            if (animation != null)
            {
                animation.Play("CardFlip");
            }
            AudioSource audio = card.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Play();
            }
        }

        public static bool isCardPlaying(GameObject card)
        {
            bool playing = false;

            Animation animation = card.GetComponent<Animation>();
            if (animation != null)
            {
                playing = animation.IsPlaying("CardFlip");
            }
            AudioSource audio = card.GetComponent<AudioSource>();
            if (audio != null && !playing)
            {
                playing = audio.isPlaying;
            }

            return playing;
        }
    }
}