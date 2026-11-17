using System;
using System.Collections.Generic;
using System.Linq;
using Suit___Slash;

public class BossAI
{
    public List<CardState> SelectBestHand(List<CardState> handCards)
    {
        if (handCards == null || handCards.Count == 0)
            return new List<CardState>();

        ScoredHand bestHand = null;

        int maxCardsToPick = Math.Min(5, handCards.Count);

        for (int k = 1; k <= maxCardsToPick; k++)
        {
            var combinations = GetCombinations(handCards, k).ToList();

            foreach (var combo in combinations)
            {
                var scored = EvaluateHand(combo);

                if (bestHand == null || scored.TotalDamageScore > bestHand.TotalDamageScore)
                {
                    bestHand = scored;
                }
            }
        }

        return bestHand?.Cards.ToList() ?? new List<CardState>();
    }

    private IEnumerable<List<T>> GetCombinations<T>(IEnumerable<T> list, int length)
    {
        if (length == 0)
        {
            yield return new List<T>();
            yield break;
        }

        int index = 0;
        foreach (var element in list)
        {
            var remainder = list.Skip(index + 1);
            foreach (var combination in GetCombinations(remainder, length - 1))
            {
                var comboList = new List<T> { element };
                comboList.AddRange(combination);
                yield return comboList;
            }
            index++;
        }
    }

    private ScoredHand EvaluateHand(List<CardState> cards)
    {
        var sortedCards = cards.OrderByDescending(c => c.Value).ToList();

        HandRank rank = HandRank.HighCard;
        int baseScore = 0;

        if (IsStraightFlush(sortedCards))
        {
            rank = HandRank.StraightFlush;
            baseScore = 9000;
        }
        else if (IsFourOfAKind(sortedCards))
        {
            rank = HandRank.FourOfAKind;
            baseScore = 8000;
        }
        else if (IsFullHouse(sortedCards))
        {
            rank = HandRank.FullHouse;
            baseScore = 7000;
        }
        else if (IsFlush(sortedCards))
        {
            rank = HandRank.Flush;
            baseScore = 6000;
        }
        else if (IsStraight(sortedCards))
        {
            rank = HandRank.Straight;
            baseScore = 5000;
        }
        else if (IsThreeOfAKind(sortedCards))
        {
            rank = HandRank.ThreeOfAKind;
            baseScore = 4000;
        }
        else if (IsTwoPair(sortedCards))
        {
            rank = HandRank.TwoPair;
            baseScore = 3000;
        }
        else if (IsPair(sortedCards))
        {
            rank = HandRank.Pair;
            baseScore = 2000;
        }
        else
        {
            rank = HandRank.HighCard;
            baseScore = 1000;
        }

        int tieBreakerScore = sortedCards.Sum(c => c.Value);
        int totalDamage = baseScore + tieBreakerScore;

        return new ScoredHand
        {
            Cards = sortedCards,
            Rank = rank,
            TotalDamageScore = totalDamage
        };
    }

    private bool IsFlush(List<CardState> cards)
    {
        if (cards.Count < 5) return false;
        return cards.All(c => c.suit == cards[0].suit);
    }

    private bool IsStraight(List<CardState> cards)
    {
        if (cards.Count < 5) return false;
        for (int i = 0; i < cards.Count - 1; i++)
        {
            if (cards[i].Value - cards[i + 1].Value != 1)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsStraightFlush(List<CardState> cards)
    {
        return IsStraight(cards) && IsFlush(cards);
    }

    private bool IsFourOfAKind(List<CardState> cards)
    {
        if (cards.Count < 4) return false;
        var groups = cards.GroupBy(c => c.Value);
        return groups.Any(g => g.Count() == 4);
    }

    private bool IsFullHouse(List<CardState> cards)
    {
        if (cards.Count < 5) return false;
        var groups = cards.GroupBy(c => c.Value).ToList();
        bool hasThree = groups.Any(g => g.Count() == 3);
        bool hasPair = groups.Any(g => g.Count() == 2);
        return hasThree && hasPair;
    }

    private bool IsThreeOfAKind(List<CardState> cards)
    {
        if (cards.Count < 3) return false;
        var groups = cards.GroupBy(c => c.Value);
        return groups.Any(g => g.Count() == 3) && !IsFullHouse(cards);
    }

    private bool IsTwoPair(List<CardState> cards)
    {
        if (cards.Count < 4) return false;
        var groups = cards.GroupBy(c => c.Value);
        return groups.Count(g => g.Count() == 2) >= 2;
    }

    private bool IsPair(List<CardState> cards)
    {
        if (cards.Count < 2) return false;
        var groups = cards.GroupBy(c => c.Value);
        return groups.Any(g => g.Count() == 2) && !IsTwoPair(cards) && !IsFullHouse(cards);
    }
}