using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day2 : Puzzle<int>
{
    public const int Rock = 0;
    public const int Paper = 1;
    public const int Scissors = 2;

    public const int Lose = 0;
    public const int Draw = 3;
    public const int Win = 6;
    
    public const char NeedLose = 'X';
    public const char NeedDraw = 'Y';
    public const char NeedWin = 'Z';

    public int Score(char other, char you)
    {
        var yourThrow = you - 'X';
        var otherThrow = other - 'A';
        
        var outcome = (yourThrow, otherThrow) switch
        {
            (Rock, Rock) => Draw,
            (Rock, Paper) => Lose,
            (Rock, Scissors) => Win,
            
            (Paper, Rock) => Win,
            (Paper, Paper) => Draw,
            (Paper, Scissors) => Lose,
            
            (Scissors, Rock) => Lose,
            (Scissors, Paper) => Win,
            (Scissors, Scissors) => Draw,
        };

        return outcome + (yourThrow + 1);
    }
    
    public override int PartOne()
    {
        return Input.Select(s => s.SingleSplit(' '))
            .Select(tuple => Score(tuple.Left[0], tuple.Right[0]))
            .Sum();
    }
    
    public int Score2(char other, char need)
    {
        var otherThrow = other - 'A';
        
        var yourThrow = (otherThrow, need) switch
        {
            (Rock, NeedLose) => Scissors,
            (Rock, NeedDraw) => Rock,
            (Rock, NeedWin) => Paper,
            
            (Paper, NeedLose) => Rock,
            (Paper, NeedDraw) => Paper,
            (Paper, NeedWin) => Scissors,
            
            (Scissors, NeedLose) => Paper,
            (Scissors, NeedDraw) => Scissors,
            (Scissors, NeedWin) => Rock,
        };

        var outcome = (need - 'X') * 3;
        return outcome + (yourThrow + 1);
    }

    public override int PartTwo()
    {
        return Input.Select(s => s.SingleSplit(' '))
            .Select(tuple => Score2(tuple.Left[0], tuple.Right[0]))
            .Sum();
    }
}