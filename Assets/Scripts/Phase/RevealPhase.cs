using DakaniLabs.CardLane.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DakaniLabs.CardLane.Phase
{
    public class RevealPhase : GamePhase
    {

        public ArenaManager ArenaManager { get; set; }

        public Lane SelectedLane { get; set; }

        public bool StockpileRule { get; set; }

        private float waitTime = 0f;

        private int resolution = 0;

        public override void execute()
        {
            // after animation resolve
            if (!CardUtils.isCardPlaying(SelectedLane.PlayerCard)
                && !CardUtils.isCardPlaying(SelectedLane.OpponentCard))
            {
                waitTime += Time.deltaTime;
            }

            if (waitTime >= 1.5f)
            {
                bool destroy = true;
                // resolve
                CardScript playerCardScript = SelectedLane.PlayerCard.GetComponent<CardScript>();
                CardScript opponentCardScript = SelectedLane.OpponentCard.GetComponent<CardScript>();
                resolution = CardRuleUtils.Resolve(playerCardScript.Card, opponentCardScript.Card);

                if (resolution > 0)
                {
                    SelectedLane.playerPoints += SelectedLane.playerStockpile.childCount + 1;
                }
                else if (resolution < 0)
                {
                    SelectedLane.opponentPoints += SelectedLane.opponentStockpile.childCount + 1;
                }
                else if (StockpileRule)
                {
                    // handle stockpile maneuver here
                    destroy = false;
                    ArenaManager.placeStockpile(SelectedLane.removeCard(Lane.Slot.PLAYER), SelectedLane.playerStockpile);
                    ArenaManager.placeStockpile(SelectedLane.removeCard(Lane.Slot.OPPONENT), SelectedLane.opponentStockpile);
                }

                //TODO it'd be cool to have a fade animation
                // remove cards from lane slots
                if (destroy)
                {
                    GameObject.Destroy(SelectedLane.removeCard(Lane.Slot.PLAYER));
                    GameObject.Destroy(SelectedLane.removeCard(Lane.Slot.OPPONENT));
                    foreach (Transform child in SelectedLane.playerStockpile)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    foreach (Transform child in SelectedLane.opponentStockpile)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }
            }
        }

        public override GamePhase getNextPhase()
        {
            GamePhase next = null;
            if (StockpileRule && resolution == 0)
            {
                //TODO stockpile draw phase
                next = new StockpileDrawPhase();
            }
            else
            {
                //TODO fill
                next = new FillPhase();
            }

            return next;
        }

        public override bool hasCompleted()
        {
            return waitTime >= 1.5f;
        }
    }
}