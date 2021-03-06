﻿using DakaniLabs.CardLane.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DakaniLabs.CardLane.Phase
{
    public class LaneSelectionPhase : GamePhase
    {
        public Lane SelectedLane { get; set; }

        private float selectionTime = 0;

        private bool mouseDown = false;

        public override void execute()
        {
            // make a lane selection
            GameObject laneGo = null;
            bool hadInput = false;
            // Look for all fingers
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                Ray ray = Camera.main.ScreenPointToRay(touch.position);

                if (HitUtils.detectHitLane(ray, out laneGo))
                {
                    selectionTime += Time.deltaTime;
                    hadInput = true;
                }
            }

            if (!mouseDown && Input.touchCount == 0)
            {
                //Touch was released, stop counting
                selectionTime = 0f;
            }
            if (Input.GetMouseButtonUp(0))
            {
                selectionTime = 0f;
                mouseDown = false;
            }
            if (Input.GetMouseButtonDown(0))
            {
                mouseDown = true;
            }
            if (mouseDown && Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (HitUtils.detectHitLane(ray, out laneGo))
                {
                    selectionTime += Time.deltaTime;
                    hadInput = true;
                }
            }

            if (hadInput && selectionTime >= 1f)
            {
                selectionTime = 0f;
                mouseDown = false;

                SelectedLane = laneGo.GetComponent<Lane>();
                checkReveal();
            }
        }

        public override GamePhase getNextPhase()
        {
            return new RevealPhase();
        }

        public override bool hasCompleted()
        {
            return SelectedLane != null
                    && SelectedLane.PlayerCard != null
                    && SelectedLane.OpponentCard != null;
        }

        protected void checkReveal()
        {
            if (hasCompleted())
            {
                // animate the cards
                CardUtils.animateCard(SelectedLane.PlayerCard);
                CardUtils.animateCard(SelectedLane.OpponentCard);
            }
        }
    }
}