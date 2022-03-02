using System.Text.RegularExpressions;
using AdventToolkit;

namespace AdventOfCode2016.Puzzles;

public class Day7 : Puzzle
{
    private Regex _abba = new(@"(.)(?!\1)(.)\2\1", RegexOptions.Compiled);
    private Regex _ssl = new(@"(.)(?!\1)(.)\1", RegexOptions.Compiled);

    public bool IsAbba(string s, bool brackets = true)
    {
        if (brackets)
        {
            var pos = 0;
            while ((pos = s.IndexOf('[', pos)) > -1)
            {
                var start = pos;
                pos = s.IndexOf(']', pos);
                if (IsAbba(s[(start + 1)..pos])) return false;
            }
        }
        s = Regex.Replace(s, @"\[.+?\]", "|||");
        return _abba.IsMatch(s);
    }

    public override void PartOne()
    {
        var count = Input.Count(s => IsAbba(s));
        WriteLn(count);
    }

    public bool IsSsl(string s)
    {
        var brackets = new List<string>();
        var pos = 0;
        while ((pos = s.IndexOf('[', pos)) > -1)
        {
            var start = pos;
            pos = s.IndexOf(']', pos);
            brackets.Add(s[(start + 1)..pos]);
        }
        s = Regex.Replace(s, @"\[.+?\]", "||");
        
        pos = 0;
        Match m;
        while ((m = _ssl.Match(s, pos)).Success)
        {
            pos++;
            var a = m.Groups[1];
            var b = m.Groups[2];
            if (brackets.Any(h => h.Contains($"{b}{a}{b}"))) return true;
        }
        return false;
    }

    public override void PartTwo()
    {
        var count = Input.Count(IsSsl);
        WriteLn(count);
    }
}