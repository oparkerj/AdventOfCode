using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections.Space;

namespace AdventOfCode2021.Puzzles;

public class Day4 : Puzzle
{
    public Day4()
    {
        Part = 2;
    }

    public IEnumerable<int> Nums => AllGroups[0][0].Csv().Ints();

    public List<Board> Boards() => AllGroups.Skip(1).Select(data => new Board(data)).ToList();

    public override void PartOne()
    {
        var boards = Boards();
            
        foreach (var num in Nums)
        {
            foreach (var board in boards)
            {
                board.Mark(num);
                if (board.CheckWon())
                {
                    WriteLn(board.Score(num));
                    return;
                }
            }
        }
    }

    public override void PartTwo()
    {
        var boards = Boards();
        var lastScore = 0;
            
        foreach (var num in Nums)
        {
            for (var i = 0; i < boards.Count; i++)
            {
                var board = boards[i];
                board.Mark(num);
                if (board.CheckWon())
                {
                    boards.RemoveConcurrent(ref i);
                    lastScore = board.Score(num);
                }
            }
        }
            
        WriteLn(lastScore);
    }

    public class Board
    {
        public FixedGrid<int> Grid;

        public Board(string[] data)
        {
            Grid = data.Select(s => s.Spaced().Ints()).ToFixedGrid(5, 5);
        }

        public void Mark(int i)
        {
            if (!Grid.Find(i, out var pos)) return;
            Grid[pos] = -i;
        }

        public int Score(int last)
        {
            return Grid.Values.Where(v => v >= 0).Sum() * last;
        }

        public bool CheckWon()
        {
            for (var i = 0; i < 5; i++)
            {
                if (Grid.Row(i).All(v => v < 0)) return true;
            }
            for (var i = 0; i < 5; i++)
            {
                if (Grid.Col(i).All(v => v < 0)) return true;
            }
            return false;
        }
    }
}