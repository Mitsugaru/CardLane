public class Card
{

    private Suit suit;

    public Suit Suit
    {
        get
        {
            return suit;
        }
    }

    private Rank rank;

    public Rank Rank
    {
        get
        {
            return rank;
        }
    }

    public Card(Rank rank, Suit suit)
    {
        this.suit = suit;
        this.rank = rank;
    }

    public override bool Equals(object obj)
    {
        if(obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Card c = (Card)obj;
        return suit == c.Suit && rank == c.Rank;
    }

    public override int GetHashCode()
    {
        return (int)suit * 31 + (int)rank * 31;
    }

    public override string ToString()
    {
        string name = rank.ToString();

        if (rank != Rank.JOKER)
        {
            name = string.Concat(name, " of ", suit.ToString());
        }

        return name;
    }
}
