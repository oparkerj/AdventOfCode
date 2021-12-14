using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventToolkit.Collections;
using MoreLinq;

namespace AdventOfCode2021.Puzzles;

public class Day14 : Puzzle
{
    public Day14()
    {
        Part = 2;
    }

    public BigInteger Compute(int times)
    {
        var counts = AllGroups[0][0].Window(2).StrEach().Frequencies().SelectValue(i => (BigInteger) i).ToDictionary();
        var map = AllGroups[1].ReadKeys("->");
        
        for (var i = 0; i < times; i++)
        {
            var next = new DefaultDict<string, BigInteger>();
            foreach (var (pair, count) in counts)
            {
                var add = map[pair];
                next[pair[0] + add] += count;
                next[add + pair[1]] += count;
            }
            counts = next;
        }
        
        var freq = new DefaultDict<char, BigInteger>();
        freq[AllGroups[0][0][^1]]++; // Account for possible off-by-one
        foreach (var (pair, count) in counts)
        {
            freq[pair[0]] += count;
        }
        return freq.Values.Max() - freq.Values.Min();
    }

    public override void PartOne()
    {
        WriteLn(Compute(10));
    }

    public override void PartTwo()
    {
        WriteLn(Compute(40));
    }

    public void OriginalPartOne()
    {
        var template = AllGroups[0][0].ToLinkedList();
        var map = AllGroups[1].ReadKeys("->");

        for (var i = 0; i < 10; i++)
        {
            var insert = new Dictionary<LinkedListNode<char>, char>();

            foreach (var pair in template.Nodes().Window(2))
            {
                var key = pair.Select(node => node.Value).Str();
                if (map.TryGetValue(key, out var s))
                {
                    insert[pair[0]] = s[0];
                }
            }
            
            foreach (var (node, c) in insert)
            {
                template.AddAfter(node, c);
            }
        }

        var freq = template.Str().Frequencies().ToDictionary();
        WriteLn(freq.Values.Max() - freq.Values.Min());
    }
}