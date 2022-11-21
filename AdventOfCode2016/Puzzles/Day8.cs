using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2016.Puzzles;

public class Day8 : Puzzle
{
    private const int Width = 50;
    private const int Height = 6;
    
    private bool[,] _screen = new bool[Width, Height];
    private bool[] _tempRow = new bool[Width];
    private bool[] _tempCol = new bool[Height];

    private void Rect(int x, int y)
    {
        for (var i = 0; i < x; i++)
        {
            for (var j = 0; j < y; j++)
            {
                _screen[i, j] = true;
            }
        }
    }

    private void RotateRow(int index, int amount)
    {
        var length = _screen.GetLength(0);
        amount %= length;
        if (amount == 0) return;
        for (var i = 0; i < Width; i++)
        {
            _tempRow[(i + amount) % length] = _screen[i, index];
        }
        for (var i = 0; i < Width; i++)
        {
            _screen[i, index] = _tempRow[i];
        }
    }

    private void RotateColumn(int index, int amount)
    {
        var length = _screen.GetLength(1);
        amount %= length;
        if (amount == 0) return;
        for (var i = 0; i < Height; i++)
        {
            _tempCol[(i + amount) % length] = _screen[index, i];
        }
        for (var i = 0; i < Height; i++)
        {
            _screen[index, i] = _tempCol[i];
        }
    }

    private void RunInput()
    {
        var matcher = new StringMatcher();
        matcher.AddRegex<int, int>(@"rect (\d+)x(\d+)", Rect);
        matcher.AddRegex<int, int>(@"rotate row y=(\d+) by (\d+)", RotateRow);
        matcher.AddRegex<int, int>(@"rotate column x=(\d+) by (\d+)", RotateColumn);
        
        foreach (var line in Input)
        {
            matcher.Handle(line);
        }
    }
    
    public override void PartOne()
    {
        RunInput();
        var count = _screen.Cast<bool>().Count(true);
        WriteLn(count);
    }

    public override void PartTwo()
    {
        RunInput();
        WriteLn(_screen.Stringify(b => b ? '#' : ' '));
    }
}