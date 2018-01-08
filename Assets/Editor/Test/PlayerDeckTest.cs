using DakaniLabs.CardLane.Card;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

[TestFixture]
public class PlayerDeckTest
{

    private PlayerDeck deck;

    private IList<PlayingCard> spades;

    [SetUp]
    public void Init()
    {
        deck = new PlayerDeck();
        spades = GenerateCardsForSuit(Suit.SPADES);
    }

    [Test]
    public void TestAddCards()
    {
        deck.AddRange(spades);

        Assert.AreEqual(13, deck.Cards.Count);
    }

    [Test]
    public void TestShuffleAll()
    {
        deck.AddRange(spades);
        deck.Shuffle();

        bool sameOrder = true;

        for (int i = 0; i < spades.Count; i++)
        {
            if (!spades[i].Equals(deck.Deck[i]))
            {
                sameOrder = false;
                break;
            }
        }

        Assert.False(sameOrder);
    }

    [Test]
    public void TestDraw()
    {
        deck.AddRange(spades);

        PlayingCard card = deck.Draw();

        Assert.True(spades.Contains(card));
        Assert.False(deck.Deck.Contains(card));
    }

    public void TestShuffleAfterDraw()
    {
        deck.AddRange(spades);

        deck.Draw();

        List<PlayingCard> snapshot = new List<PlayingCard>();
        snapshot.AddRange(deck.Deck);

        deck.Shuffle();

    }

    private IList<PlayingCard> GenerateCardsForSuit(Suit suit)
    {
        List<PlayingCard> cards = new List<PlayingCard>();

        foreach (Rank rank in Rank.Values)
        {
            if (rank != Rank.JOKER)
            {
                cards.Add(new PlayingCard(rank, suit));
            }
        }

        return cards;
    }
}
