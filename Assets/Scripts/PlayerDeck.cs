using DakaniLabs.CardLane.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages and maintains a deck of cards
/// </summary>
public class PlayerDeck
{

    private List<Card> cards = new List<Card>();

    /// <summary>
    /// List of cards that comprises the deck
    /// </summary>
    public IList<Card> Cards
    {
        get
        {
            return cards.AsReadOnly();
        }
    }

    /// <summary>
    /// Queue of the current deck
    /// </summary>
    private Queue<Card> inDeck = new Queue<Card>();

    /// <summary>
    /// List of cards that are currently in the deck
    /// </summary>
    /// <returns></returns>
    public IList<Card> Deck
    {
        get
        {
            return inDeck.ToArray();
        }
    }

    /// <summary>
    /// Random instance
    /// </summary>
    private static System.Random random = new System.Random();

    /// <summary>
    /// Constructor
    /// </summary>
    public PlayerDeck()
    {
    }

    /// <summary>
    /// Adds a card to the deck
    /// </summary>
    /// <param name="card"></param>
    public void Add(Card card)
    {
        cards.Add(card);
        inDeck.Enqueue(card);
        Shuffle();
    }

    /// <summary>
    /// Adds cards to the deck
    /// </summary>
    /// <param name="collection">Collection of cards</param>
    public void AddRange(ICollection<Card> collection)
    {
        cards.AddRange(collection);

        List<Card> sum = new List<Card>();
        sum.AddRange(collection);
        sum.AddRange(inDeck.ToArray());
        ShuffleWith(sum);
    }

    /// <summary>
    /// Remove a single card from the deck.
    /// Will reshuffle if the deck currently contained it.
    /// </summary>
    /// <param name="card">Card to remove</param>
    public void Remove(Card card)
    {
        cards.Remove(card);

        if (inDeck.Contains(card))
        {
            List<Card> remaining = new List<Card>();
            remaining.AddRange(inDeck.ToArray());
            remaining.Remove(card);
            ShuffleWith(remaining);
        }
    }

    /// <summary>
    /// Remove cards from the deck.
	/// Will reshuffle if the deck contained any of the cards.
    /// </summary>
    /// <param name="collection">Cards to remove</param>
    public void RemoveRange(ICollection<Card> collection)
    {
        foreach (Card card in collection)
        {
            cards.Remove(card);
        }

        List<Card> remaining = new List<Card>();
        foreach (Card card in inDeck)
        {
            if (!collection.Contains(card))
            {
                remaining.Add(card);
            }
        }

        if (remaining.Count != inDeck.Count)
        {
            ShuffleWith(remaining);
        }
    }

    /// <summary>
    /// Shuffle the current deck
    /// </summary>
    public void Shuffle()
    {
        ShuffleWith(inDeck.ToArray());
    }

    /// <summary>
    /// Shuffle all the cards
    /// </summary>
    public void ShuffleAll()
    {
        ShuffleWith(cards);
    }

    /// <summary>
    /// Replace the current deck and shuffle using the given collection
    /// </summary>
    /// <param name="collection">Collection of cards to shuffle as the deck</param>
    private void ShuffleWith(ICollection<Card> collection)
    {
        List<Card> temp = new List<Card>();
        temp.AddRange(collection);
        inDeck.Clear();

        while (temp.Count > 0)
        {
            int index = random.Next(temp.Count);
            inDeck.Enqueue(temp[index]);
            temp.RemoveAt(index);
        }
    }

    public void Clear()
    {
        cards.Clear();
        inDeck.Clear();
    }

    /// <summary>
    /// Draw a card from the deck
    /// </summary>
    /// <returns>The card that was drawn</returns>
    public Card Draw()
    {
        Card card = null;
        if(inDeck.Count > 0)
        {
            card = inDeck.Dequeue();
        }
        return card;
    }
}
