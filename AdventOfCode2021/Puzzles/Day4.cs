using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2021.Puzzles
{
    public class Day4 : Puzzle
    {
        public Day4()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var nums = AllGroups[0][0].Csv().Ints().ToList();
            var boards = AllGroups.Skip(1).Select(data => new Board(data)).ToList();
            
            foreach (var num in nums)
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
            var nums = AllGroups[0][0].Csv().Ints().ToList();
            var boards = AllGroups.Skip(1).Select(data => new Board(data)).ToList();
            
            var lastScore = 0;
            
            foreach (var num in nums)
            {
                for (var i = 0; i < boards.Count; i++)
                {
                    var board = boards[i];
                    board.Mark(num);
                    if (board.CheckWon())
                    {
                        boards.RemoveConcurrent(b => b == board, ref i);
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
                Grid = new FixedGrid<int>(5, 5);
                for (var y = 0; y < 5; y++)
                {
                    foreach (var (x, v) in data[y].Spaced().Ints().Index())
                    {
                        Grid[x, y] = v;
                    }
                }
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
                if (Grid.Diagonal(true).All(v => v < 0)) return true;
                if (Grid.Diagonal(false).All(v => v < 0)) return true;
                return false;
            }
        }
    }
}