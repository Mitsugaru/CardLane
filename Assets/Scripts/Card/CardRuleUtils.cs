namespace DakaniLabs.CardLane.Card
{
    /// <summary>
    /// Utility class to handle card rule / resolution logic
    /// </summary>
    public class CardRuleUtils
    {

        /// <summary>
        /// Resolves the two given cards using the game rules.
        /// Returns an int result that is similar to compare logic.
        /// Where 0 is equal, and <= -1 and >= 1 are respective to the
        /// first card compared against the second.
        /// </summary>
        /// <param name="firstCard">Base card to compare to</param>
        /// <param name="secondCard">Card being compared against</param>
        /// <returns>Comparison result</returns>
        public static int Resolve(Card firstCard, Card secondCard)
        {
            int result = 0;

            if (firstCard.Rank != secondCard.Rank)
            {
                if (firstCard.Rank.Equals(Rank.JOKER) || secondCard.Rank.Equals(Rank.JOKER))
                {
                    result = 0;
                }
                else if (firstCard.Rank.Equals(Rank.ACE))
                {
                    result = HandleAceLogic(secondCard);
                }
                else if (secondCard.Rank.Equals(Rank.ACE))
                {
                    result = -HandleAceLogic(firstCard);
                }
                else if (IsRoyal(firstCard))
                {
                    result = HandleRoyalLogic(secondCard);
                }
                else if (IsRoyal(secondCard))
                {
                    result = -HandleRoyalLogic(firstCard);
                }
                else
                {
                    result = firstCard.Rank - secondCard.Rank;
                }

            }

            return result;
        }

        /// <summary>
        /// Handles the special case rule for Aces against the target card.
        /// Assumes the source card is an Ace.
        /// </summary>
        /// <param name="targetCard">Card to compare to</param>
        /// <returns>Comparison result</returns>
        private static int HandleAceLogic(Card targetCard)
        {
            int result = 0;

            if (!targetCard.Rank.Equals(Rank.ACE))
            {
                if (IsRoyal(targetCard))
                {
                    result = 1;
                }
                else
                {
                    result = -1;
                }
            }

            return result;
        }

        /// <summary>
        /// Handle the rules for royals against the target card.
        /// Assumes the source card is a royal.
        /// </summary>
        /// <param name="targetCard">Card to compare to</param>
        /// <returns>Comparison result</returns>
        private static int HandleRoyalLogic(Card targetCard)
        {
            int result = 0;

            if (targetCard.Rank.Equals(Rank.ACE))
            {
                result = -1;
            }
            else if (!IsRoyal(targetCard))
            {
                result = 1;
            }

            return result;
        }

        /// <summary>
        /// Determine if the given card is a royal card
        /// </summary>
        /// <param name="card">Card to validate</param>
        /// <returns>True if the Card Rank is a royal card, else false</returns>
        public static bool IsRoyal(Card card)
        {
            return IsRoyal(card.Rank);
        }

        public static bool IsRoyal(Rank rank)
        {
            return rank.Equals(Rank.KING)
                || rank.Equals(Rank.QUEEN)
                || rank.Equals(Rank.JACK);
        }
    }
}