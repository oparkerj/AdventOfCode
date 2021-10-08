using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Solvers;
using MoreLinq;

namespace AdventOfCode2019.Puzzles
{
    public class Day24 : Puzzle
    {
        public const char Bug = '#';

        public Day24()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            static int GetState(GameOfLife<AdventToolkit.Common.Pos> game)
            {
                return game.WhereValue(true).Keys().Aggregate(0, (current, pos) => current | 1 << (pos.X + pos.Y * 5));
            }
            
            var seen = new HashSet<int>();
            var game = new GameOfLife<AdventToolkit.Common.Pos>();
            game.WithNeighborFunction(pos => pos.Adjacent())
                .WithLivingDeadRules(i => i != 1, i => i is 1 or 2);
            Input.ToGrid(false).ForEach(pair => game[pair.Key] = pair.Value == Bug);

            while (true)
            {
                seen.Add(GetState(game));
                game.Step();
                var state = GetState(game);
                if (seen.Contains(state))
                {
                    WriteLn(state);
                    return;
                }
            }
        }

        public override void PartTwo()
        {
            static Side GetSide(AdventToolkit.Common.Pos dir)
            {
                return dir switch
                {
                    (1, 0) => Side.Left,
                    (-1, 0) => Side.Right,
                    (0, 1) => Side.Bottom,
                    (0, -1) => Side.Top,
                    _ => throw new Exception("Invalid direction.")
                };
            }

            var map = Input.ToGrid();
            var area = map.Bounds;
            var mid = area.MidPos;
            var game = new GameOfLife<(AdventToolkit.Common.Pos Pos, int Level)>();
            map.ForEach(pair => game[(pair.Key, 0)] = pair.Value == Bug);
            game.WithLivingDeadRules(i => i != 1, i => i is 1 or 2);
            game.WithKeepDead(false);
            game.WithExpansion();

            game.WithNeighborFunction(Neighbors);
            IEnumerable<(AdventToolkit.Common.Pos Pos, int Level)> Neighbors((AdventToolkit.Common.Pos Pos, int Level) cell)
            {
                foreach (var pos in cell.Pos.Adjacent())
                {
                    if (pos == mid)
                    {
                        foreach (var inner in area.GetSidePositions(GetSide(mid - cell.Pos)))
                        {
                            yield return (inner, cell.Level + 1);
                        }
                        continue;
                    }
                    if (pos.X == -1) yield return (new Pos(1, -2), cell.Level - 1);
                    else if (pos.X == 5) yield return (new Pos(3, -2), cell.Level - 1);
                    else if (pos.Y == 1) yield return (new Pos(2, -1), cell.Level - 1);
                    else if (pos.Y == -5) yield return (new Pos(2, -3), cell.Level - 1);
                    else yield return (pos, cell.Level);
                }
            }
            
            game.Step(200);
            WriteLn(game.CountActive());
        }
    }
}