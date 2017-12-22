using System.Collections.Generic;
/// <summary>
/// Card rank / numeral
/// </summary>
public class Rank
{
    public static readonly Rank JOKER = new Rank("Joker", 0);
    public static readonly Rank ACE = new Rank("Ace", 1);
    public static readonly Rank TWO = new Rank("Two", 2);
    public static readonly Rank THREE = new Rank("Three", 3);
    public static readonly Rank FOUR = new Rank("Four", 4);
    public static readonly Rank FIVE = new Rank("Five", 5);
    public static readonly Rank SIX = new Rank("Six", 6);
    public static readonly Rank SEVEN = new Rank("Seven", 7);
    public static readonly Rank EIGHT = new Rank("Eight", 8);
    public static readonly Rank NINE = new Rank("Nine", 9);
    public static readonly Rank TEN = new Rank("Ten", 10);
    public static readonly Rank JACK = new Rank("Jack", 11);
    public static readonly Rank QUEEN = new Rank("Queen", 12);
    public static readonly Rank KING = new Rank("King", 13);

    public static IEnumerable<Rank> Values
    {
        get
        {
            yield return JOKER;
            yield return ACE;
            yield return TWO;
            yield return THREE;
            yield return FOUR;
            yield return FIVE;
            yield return SIX;
            yield return SEVEN;
            yield return EIGHT;
            yield return NINE;
            yield return TEN;
            yield return JACK;
            yield return QUEEN;
            yield return KING;
        }
    }

    private readonly string name;
    private readonly int value;

    public int Value
    {
        get
        {
            return value;
        }
    }

    Rank(string name, int value)
    {
        this.name = name;
        this.value = value;
    }

    public override string ToString()
    {
        return name;
    }

    public static implicit operator int(Rank a)
    {
        return a.value;
    }
}
