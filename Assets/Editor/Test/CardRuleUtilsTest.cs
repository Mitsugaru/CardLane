using NUnit.Framework;

[TestFixture]
public class CardRuleUtilsTest
{

    [Test]
    public void TestNumericRule()
    {
        foreach (Rank rank in Rank.Values)
        {
            if (isRankSpecial(rank))
            {
                continue;
            }

            Card source = new Card(rank, Suit.SPADES);

            foreach (Rank targetRank in Rank.Values)
            {
                if (!isRankSpecial(targetRank))
                {
                    Card target = new Card(targetRank, Suit.SPADES);

                    if (rank > targetRank)
                    {
                        Assert.GreaterOrEqual(CardRuleUtils.Resolve(source, target), 1);
                    }
                    else if (rank < targetRank)
                    {
                        Assert.LessOrEqual(CardRuleUtils.Resolve(source, target), -1);
                    }
                    else
                    {
                        Assert.AreEqual(0, CardRuleUtils.Resolve(source, target));
                    }
                }
            }
        }
    }

    [Test]
    public void TestAceRule()
    {
        foreach (Suit suit in Suit.Values)
        {
            Card ace = new Card(Rank.ACE, suit);

            foreach (Rank rank in Rank.Values)
            {
                if (rank.Equals(Rank.JOKER))
                {
                    continue;
                }

                Card target = new Card(rank, suit);

                if (rank.Equals(Rank.ACE))
                {
                    Assert.AreEqual(0, CardRuleUtils.Resolve(ace, target));
                }
                else if (CardRuleUtils.IsRoyal(target))
                {
                    Assert.GreaterOrEqual(CardRuleUtils.Resolve(ace, target), 1);
                }
                else
                {
                    Assert.LessOrEqual(CardRuleUtils.Resolve(ace, target), -1);
                }
            }
        }
    }

    [Test]
    public void TestRoyalRule()
    {
        foreach (Rank rank in Rank.Values)
        {
            if (!CardRuleUtils.IsRoyal(rank))
            {
                continue;
            }
            else
            {
                Card source = new Card(rank, Suit.SPADES);

                foreach (Rank targetRank in Rank.Values)
                {
                    Card target = new Card(targetRank, Suit.SPADES);

                    if (rank.Equals(Rank.JOKER) || CardRuleUtils.IsRoyal(targetRank))
                    {
                        Assert.AreEqual(0, CardRuleUtils.Resolve(source, target));
                    }
                    if (targetRank.Equals(Rank.ACE))
                    {
                        Assert.LessOrEqual(CardRuleUtils.Resolve(source, target), -1);
                    }
                }
            }
        }
    }

    [Test]
    public void TestJokerRule()
    {
        Card joker = new Card(Rank.JOKER, Suit.NONE);
        foreach (Suit suit in Suit.Values)
        {
            foreach (Rank rank in Rank.Values)
            {
                Card card = new Card(rank, suit);

                Assert.AreEqual(0, CardRuleUtils.Resolve(joker, card));
            }
        }
    }

    private bool isRankSpecial(Rank rank)
    {
        return rank.Equals(Rank.ACE)
            || rank.Equals(Rank.JOKER)
            || rank.Equals(Rank.KING)
            || rank.Equals(Rank.QUEEN)
            || rank.Equals(Rank.JACK);
    }
}
