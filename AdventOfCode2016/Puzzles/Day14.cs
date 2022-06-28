using System.Security.Cryptography;
using System.Text.RegularExpressions;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;

namespace AdventOfCode2016.Puzzles;

public class Day14 : Puzzle
{
    public IEnumerable<string> Hashes()
    {
        var salt = InputLine;
        var index = 0;
        using var md5 = MD5.Create();
        
        while (true)
        {
            yield return (salt + index).Hash(md5, false);
            index++;
        }
    }

    public IEnumerable<string> StretchedHashes()
    {
        using var md5 = MD5.Create();
        foreach (var hash in Hashes())
        {
            yield return hash.Repeat(s => s.Hash(md5, false), 2016);
        }
    }

    private Deque<(int, string)> Clean(Deque<(int, string)> queue, int i)
    {
        var cutoff = i - 1000;
        if (cutoff < 0) return queue;
        while (queue.TryPeekFirst(out var val) && val.Item1 < cutoff)
        { 
            queue.RemoveFirst();
        }
        return queue;
    }

    public IEnumerable<(int index, string hash)> KeyStream(Func<IEnumerable<string>> generator)
    {
        var seen = new Dictionary<char, Deque<(int, string)>>();
        var regex3 = new Regex(@"(.)\1\1", RegexOptions.Compiled);
        var regex5 = new Regex(@"(.)\1\1\1\1", RegexOptions.Compiled);

        using var stream = generator().Indexed().GetEnumerator();

        while (true)
        {
            stream.MoveNext();
            var (i, hash) = stream.Current;

            var m5 = regex5.Match(hash);
            if (m5.Success)
            {
                var deque = Clean(seen.GetOrNew(m5.Groups[0].ValueSpan[0]), i);
                while (deque.Count > 0)
                {
                    yield return deque.RemoveFirst();
                }
            }
            
            var m3 = regex3.Match(hash);
            if (m3.Success)
            {
                Clean(seen.GetOrNew(m3.Groups[1].ValueSpan[0]), i).AddLast((i, hash));
            }
        }
    }

    public override void PartOne()
    {
        var key = KeyStream(Hashes).ElementAt(63).index;
        WriteLn(key);
    }

    public override void PartTwo()
    {
        var key = KeyStream(StretchedHashes).ElementAt(63).index;
        WriteLn(key);
    }
}