using System.Collections.Generic;

namespace DakaniLabs.CardLane.Card
{
    /// <summary>
    /// Card suit / type
    /// </summary>
    public class Suit
    {
        public static readonly Suit NONE = new Suit("", 0);
        public static readonly Suit CLUBS = new Suit("Clubs", 1);
        public static readonly Suit DIAMONDS = new Suit("Diamonds", 2);
        public static readonly Suit HEARTS = new Suit("Hearts", 3);
        public static readonly Suit SPADES = new Suit("Spades", 4);

        public static IEnumerable<Suit> Values
        {
            get
            {
                yield return NONE;
                yield return CLUBS;
                yield return DIAMONDS;
                yield return HEARTS;
                yield return SPADES;
            }
        }

        private readonly string name;
        private readonly int index;

        Suit(string name, int index)
        {
            this.name = name;
            this.index = index;
        }

        public override string ToString()
        {
            return name;
        }

        public static implicit operator int(Suit a)
        {
            return a.index;
        }
    }
}