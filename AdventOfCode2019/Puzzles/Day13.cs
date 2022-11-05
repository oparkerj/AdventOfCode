using System;
using AdventOfCode2019.IntCode;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Extensions;

namespace AdventOfCode2019.Puzzles;

public class Day13 : Puzzle
{
    public Grid<char> Display = new();
    public int Score = 0;

    public Day13()
    {
        Part = 2;
    }

    public char Tile(int id)
    {
        return id switch
        {
            0 => ' ',
            1 => '#',
            2 => '@',
            3 => '_',
            4 => 'O',
            _ => ' ',
        };
    }
        
    public override void PartOne()
    {
        var c = Computer.From(InputLine);
        c.Output = new OutputSequence()
            .ThenMultipleInts(3, ints => Display[ints[0], -ints[1]] = Tile(ints[2]))
            .Line;
        c.Execute();
        WriteLn(Display.CountValues(Tile(2)));
    }

    public override void PartTwo()
    {
        var c = Computer.From(InputLine);
        c.Output = new OutputSequence()
            .ThenMultipleInts(3, ints =>
            {
                if (ints[0] == -1 && ints[1] == 0) Score = ints[2];
                else Display[ints[0], -ints[1]] = Tile(ints[2]);
            })
            .Line;
        c[0] = 2;
        c.Input = () => Math.Sign(Display.Find(Tile(4)).X - Display.Find(Tile(3)).X);
        c.Execute();
        WriteLn(Score);
    }
}