using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2015.Puzzles;

public class Day11 : Puzzle<string>
{
    public string Next(string s)
    {
        const int li = 'i' - 'a';
        const int lo = 'o' - 'a';
        const int ll = 'l' - 'a';
        const int lz = 'z' - 'a';
        
        var a = s.Select(c => c - 'a').ToArray(s.Length);
        do
        {
            Increment(a.Length - 1);
        } while (!Valid());
        return a.Select(i => (char) (i + 'a')).Str();

        bool Valid()
        {
            if (a.Any(i => i is li or lo or ll)) return false;
            // Check for an increasing sequence of at least 3 letters
            var increase = a.Pairwise((l, r) => r - l == 1).Pairwise((b, b1) => b && b1).Any(b => b);
            if (!increase) return false;
            var first = -1;
            for (var i = 1; i < a.Length; i++)
            {
                if (a[i - 1] != a[i]) continue;
                if (first == -1) first = a[i++];
                else if (a[i++] == first) continue;
                else return true;
            }
            return false;
        }

        void Increment(int index)
        {
            if (index < 0) return;
            if (a[index] == lz)
            {
                a[index] = 0;
                Increment(index - 1);
            }
            else if (++a[index] is li or lo or ll)
            {
                a[index]++;
                if (index < s.Length - 1)
                {
                    a.AsSpan((index + 1)..).Fill(0);
                }
            }
        }
    }
    
    public override string PartOne()
    {
        return Next(InputLine);
    }

    public override string PartTwo()
    {
        return Next(Next(InputLine));
    }
}