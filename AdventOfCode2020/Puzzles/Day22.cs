using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles;

public class Day22 : Puzzle
{
    public LinkedList<int> P1 = new();
    public LinkedList<int> P2 = new();

    public Day22()
    {
        ReadInput();
        Part = 2;
    }

    public void ReadInput()
    {
        var groups = Groups.ToArray();
        foreach (var card in groups[0].Skip(1).Ints())
        {
            P1.AddLast(card);
        }
        foreach (var card in groups[1].Skip(1).Ints())
        {
            P2.AddLast(card);
        }
    }
        
    public int Draw(LinkedList<int> deck)
    {
        var c = deck.First.Value;
        deck.RemoveFirst();
        return c;
    }

    public void Round()
    {
        var a = Draw(P1);
        var b = Draw(P2);
        if (a > b)
        {
            P1.AddLast(a);
            P1.AddLast(b);
        }
        else
        {
            P2.AddLast(b);
            P2.AddLast(a);
        }
    }

    public override void PartOne()
    {
        while (true)
        {
            Round();
            if (P1.Count == 0 || P2.Count == 0) break;
        }
        int result;
        if (P1.Count != 0)
        {
            result = P1.Select((card, i) => (P1.Count - i) * card).Sum();
        }
        else
        {
            result = P2.Select((card, i) => (P2.Count - i) * card).Sum();
        }
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var combat = new RecursiveCombat(P1, P2);
        var winner = combat.Play();
        int result;
        if (winner == 1)
        {
            result = combat.P1.Select((card, i) => (combat.P1.Count - i) * card).Sum();
        }
        else
        {
            result = combat.P2.Select((card, i) => (combat.P2.Count - i) * card).Sum();
        }
        WriteLn(result);
    }

    public class RecursiveCombat
    {
        public HashSet<string> States = new();
            
        public LinkedList<int> P1;
        public LinkedList<int> P2;

        public RecursiveCombat(IEnumerable<int> p1, IEnumerable<int> p2)
        {
            P1 = new LinkedList<int>(p1);
            P2 = new LinkedList<int>(p2);
        }

        public int Draw(LinkedList<int> deck)
        {
            var c = deck.First.Value;
            deck.RemoveFirst();
            return c;
        }

        public string GetState()
        {
            return string.Concat(P1) + ',' + string.Concat(P2);
        }

        public int Round()
        {
            var state = GetState();
            if (States.Contains(state)) return 1;
            var a = Draw(P1);
            var b = Draw(P2);
            int roundWinner;
            if (P1.Count >= a && P2.Count >= b)
            {
                roundWinner = new RecursiveCombat(P1.Take(a), P2.Take(b)).Play();
            }
            else if (a > b) roundWinner = 1;
            else roundWinner = 2;
            if (roundWinner == 1)
            {
                P1.AddLast(a);
                P1.AddLast(b);
            }
            else
            {
                P2.AddLast(b);
                P2.AddLast(a);
            }
            States.Add(state);
            if (P1.Count == 0) return 2;
            if (P2.Count == 0) return 1;
            return 0;
        }

        public int Play()
        {
            int winner;
            while (true)
            {
                if ((winner = Round()) > 0) break;
            }
            return winner;
        }
    }
}