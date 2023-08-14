using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2016.Puzzles;

public class Day21 : Puzzle
{
    public StringMatcher Matcher;
    public char[] Current;

    public Day21()
    {
        Matcher = new StringMatcher();
        Matcher.AddPrefix<int, int>("swap p", @"swap position (\d+) with position (\d+)", SwapPosition);
        Matcher.AddPrefix<char, char>("swap l", @"swap letter (.) with letter (.)", SwapLetter);
        Matcher.AddRegex<string, int>(@"rotate (left|right) (\d+) steps?", Rotate);
        Matcher.AddPrefix<char>("rotate b", @"rotate based on position of letter (.)", RotateFromLetter);
        Matcher.AddPrefix<int, int>("reverse", @"reverse positions (\d+) through (\d+)", Reverse);
        Matcher.AddPrefix<int, int>("move", @"move position (\d+) to position (\d+)", Move);
        
        Current = "abcdefgh".ToCharArray();
    }

    public void SwapPosition(int x, int y)
    {
        Current.Swap(x, y);
    }

    public void SwapLetter(char x, char y)
    {
        var a = Array.IndexOf(Current, x);
        var b = Array.IndexOf(Current, y);
        Current.Swap(a, b);
    }

    public void Rotate(string dir, int steps)
    {
        if (Part == 2) dir = dir == "left" ? "right" : "left";
        Current.RotateTo(steps * (dir == "left" ? 1 : -1));
    }

    public void RotateFromLetter(char c)
    {
        var i = Array.IndexOf(Current, c);
        int steps;
        if (Part == 2)
        {
            steps = i % 2 == 1 ? i / 2 + 1 : (i + 6) % 8 / 2 - 2;
        }
        else steps = -(i + 1 + (i >= 4 ? 1 : 0));
        Current.RotateTo(steps);
    }

    public void Reverse(int x, int y)
    {
        Current.AsSpan(x..++y).Reverse();
    }

    public void Move(int x, int y)
    {
        if (x == y) return;
        if (Part == 2) (x, y) = (y, x);
        var c = Current[x];
        if (x < y) Array.Copy(Current, x + 1, Current, x, y - x);
        else Array.Copy(Current, y, Current, y + 1, x - y);
        Current[y] = c;
    }

    public override void PartOne()
    {
        foreach (var s in Input)
        {
            Matcher.Handle(s);
        }
        WriteLn(new string(Current));
    }

    public override void PartTwo()
    {
        Current = "fbgdceah".ToCharArray();
        Input.AsSpan().Reverse();
        PartOne();
    }
}