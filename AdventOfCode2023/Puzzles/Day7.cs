using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day7 : Puzzle<int>
{
    public readonly IComparer<char> BetterCard;
    public readonly IComparer<string> BetterHand;
    
    public readonly int[] FullHouse = {2, 3};
    public readonly int[] TwoPair = {1, 2, 2};

    public Day7()
    {
        BetterCard = Comparer<char>.Create(CompareCard);
        BetterHand = Comparer<string>.Create(CompareHand);
    }

    public int CompareCard(char left, char right)
    {
        const string cards = "23456789TJQKA";
        const string cards2 = "J23456789TQKA";
        var use = Part == 2 ? cards2 : cards;
        return use.IndexOf(left).CompareTo(use.IndexOf(right));
    }
    
    public int TieBreaker(string left, string right)
    {
        for (var i = 0; i < 5; i++)
        {
            var compare = CompareCard(left[i], right[i]);
            if (compare != 0) return compare;
        }
        return 0;
    }
    
    public int CompareHand(string left, string right)
    {
        var leftCards = left.Frequencies().ToDictionary();
        var rightCards = right.Frequencies().ToDictionary();

        if (Part == 2)
        {
            ModifyHand(leftCards);
            ModifyHand(rightCards);
            
            void ModifyHand(IDictionary<char, int> dict)
            {
                if (!dict.TryGetValue('J', out var jokers)) return;
                
                // Five Js become the best possible hand
                if (dict.Count == 1 && dict.First().Key == 'J')
                {
                    dict.Remove('J');
                    dict['K'] = 5;
                    return;
                }
                
                // Whatever best card appears the most, increase it by the number of Js.
                var most = dict.Where(p => p.Key != 'J').Values().Max();
                var what = dict.Where(p => p.Value == most).MaxBy(p => p.Key, BetterCard).Key;
                dict[what] += jokers;
                dict.Remove('J');
            }
        }
        
        

        var leftFive = leftCards.ContainsValue(5);
        var rightFive = rightCards.ContainsValue(5);
        if (Comparison(leftFive, rightFive) is var five and not 0) return five;
        
        var leftFour = leftCards.ContainsValue(4);
        var rightFour = rightCards.ContainsValue(4);
        if (Comparison(leftFour, rightFour) is var four and not 0) return four;
        
        var leftFull = leftCards.Values.Order().SequenceEqual(FullHouse);
        var rightFull = rightCards.Values.Order().SequenceEqual(FullHouse);
        if (Comparison(leftFull, rightFull) is var full and not 0) return full;
        
        var leftThree = leftCards.ContainsValue(3);
        var rightThree = rightCards.ContainsValue(3);
        if (Comparison(leftThree, rightThree) is var three and not 0) return three;

        var leftPair = leftCards.Values.Order().SequenceEqual(TwoPair);
        var rightPair = rightCards.Values.Order().SequenceEqual(TwoPair);
        if (Comparison(leftPair, rightPair) is var pair and not 0) return pair;
        
        var leftTwo = leftCards.ContainsValue(2);
        var rightTwo = rightCards.ContainsValue(2);
        if (Comparison(leftTwo, rightTwo) is var two and not 0) return two;

        // High card
        return TieBreaker(left, right);

        int Comparison(bool leftResult, bool rightResult)
        {
            if (leftResult && rightResult) return TieBreaker(left, right);
            if (leftResult) return 1;
            if (rightResult) return -1;
            return 0;
        }
    }
    
    public override int PartOne()
    {
        var hands = Input.Select(s => s.SingleSplit(' '))
            .OrderBy(tuple => tuple.Left, BetterHand)
            .Indexed();
        return hands.Select(pair => (pair.Key + 1) * pair.Value.Right.AsInt()).Sum();
    }
}