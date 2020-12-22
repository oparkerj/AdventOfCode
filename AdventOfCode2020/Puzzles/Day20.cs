using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventToolkit;
using AdventToolkit.Data;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles
{
    public class Day20 : Puzzle
    {
        public const int Size = 10;
        public Dictionary<int, Grid<bool>> Tiles = new();
        
        public Day20()
        {
            ReadInput();
            Part = 2;
        }

        public void ReadInput()
        {
            foreach (var group in Groups)
            {
                var id = int.Parse(group[0][5..^1]);
                var width = group[0].Length;
                var height = group.Length - 1;
                var grid = new Grid<bool>();
                for (var i = 0; i < width; i++)
                {
                    for (var j = 0; j < height; j++)
                    {
                        grid[i, -j] = group[j + 1][i] == '#';
                    }
                }
                Tiles[id] = grid;
            }
        }

        public static Side Opposite(Side s)
        {
            return s switch
            {
                Side.Top => Side.Bottom,
                Side.Right => Side.Left,
                Side.Bottom => Side.Top,
                Side.Left => Side.Right,
                _ => throw new Exception()
            };
        }

        public IEnumerable<Side> Sides()
        {
            yield return Side.Top;
            yield return Side.Right;
            yield return Side.Bottom;
            yield return Side.Left;
        }

        public class Tile
        {
            public Grid<bool> Data;
            public readonly List<RelativeSide> Sides = new();

            public Tile(Grid<bool> data) => Data = data;

            public void Transform(ITransformer<bool, Grid<bool>> transformer) => transformer.ApplyTo(Data);

            public void AlignTo(Tile tile, Side side)
            {
                var relevantSide = Opposite(side);
                var tileSide = tile.Data.GetSide(side).AsString();
                foreach (var _ in ..4)
                {
                    Transform(SimpleGridTransformer<bool>.RotateRight);
                    foreach (var rs in Sides) rs.RotateRight();
                    if (tileSide == Data.GetSide(relevantSide).AsString()) return;
                }
                Transform(SimpleGridTransformer<bool>.FlipH);
                foreach (var rs in Sides) rs.FlipH();
                foreach (var _ in ..4)
                {
                    Transform(SimpleGridTransformer<bool>.RotateRight);
                    foreach (var rs in Sides) rs.RotateRight();
                    if (tileSide == Data.GetSide(relevantSide).AsString()) return;
                }
                throw new Exception("No orientation aligns tiles");
            }
        }

        public class RelativeSide
        {
            public readonly int Id;
            public Side Side;

            public RelativeSide(int id, Side side)
            {
                Id = id;
                Side = side;
            }

            public void FlipH()
            {
                if (Side is Side.Left or Side.Right) RotateRight(2);
            }

            public void RotateRight(int amount = 1)
            {
                Side = (Side) ((int) (Side + amount) % 4);
            }
        }

        public Grid<int> AssembleTiles()
        {
            // Get list of which sides of tiles are touching
            var links = new List<(RelativeSide, RelativeSide)>();
            var sides = new Dictionary<string, RelativeSide>();
            foreach (var (id, grid) in Tiles)
            {
                foreach (var side in Sides())
                {
                    var border = grid.GetSide(side).AsString();
                    var reverse = border.Reversed();
                    if (sides.TryGetValue(border, out var s) || sides.TryGetValue(reverse, out s))
                    {
                        links.Add((s, new RelativeSide(id, side)));
                        sides.Remove(border);
                        sides.Remove(reverse);
                        continue;
                    }
                    var rs = new RelativeSide(id, side);
                    sides[border] = rs;
                    sides[reverse] = rs;
                }
            }
            
            // Add relative sides to tiles
            var tiles = new Dictionary<int, Tile>();
            foreach (var (id, grid) in Tiles)
            {
                var tile = new Tile(grid);
                tiles[id] = tile;
                foreach (var (a, b) in links)
                {
                    if (a.Id == id) tile.Sides.Add(a); 
                    if (b.Id == id) tile.Sides.Add(b); 
                }
            }
            // Orient tiles based on adjacent edges
            var places = new Grid<int> {[0, 0] = tiles.Keys.First()};
            while (links.Count > 0)
            {
                var linkable = links.Where(tuple => places.ContainsValue(tuple.Item1.Id) || places.ContainsValue(tuple.Item2.Id)).ToList();
                foreach (var link in linkable)
                {
                    links.Remove(link);
                    var (first, second) = link;
                    if (places.ContainsValue(first.Id) && places.ContainsValue(second.Id)) continue;
                    if (places.ContainsValue(second.Id)) (first, second) = (second, first);
                    var from = places.First(pair => pair.Value == first.Id).Key;
                    var to = from.Adjacent(true).ElementAt((int) first.Side);
                    places[to] = second.Id;
                    tiles[second.Id].AlignTo(tiles[first.Id], first.Side);
                }
            }
            return places;
        }
        
        public override void PartOne()
        {
            var tiles = AssembleTiles();
            var result = tiles.Bounds.Corners().Select(pos => tiles[pos]).LongProduct();
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var tiles = AssembleTiles();
            SimpleGridTransformer<int>.ToFirstQuadrant().ApplyTo(tiles);

            // Remove tile borders
            foreach (var tile in Tiles)
            {
                foreach (var side in Sides())
                {
                    foreach (var pos in tile.Value.GetSidePositions(side))
                    {
                        tile.Value.Remove(pos);
                    }
                }
                tile.Value.ResetBounds();
                SimpleGridTransformer<bool>.ToFirstQuadrant().ApplyTo(tile.Value);
            }

            // Combine tiles into single grid
            var tileSize = Tiles.Values.First().Bounds;
            var tileWidth = tileSize.Width;
            var tileHeight = tileSize.Height;
            var width = tileSize.Width * tiles.Bounds.Width;
            var height = tileSize.Height * tiles.Bounds.Height;
            var full = new Grid<char>();
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var filled = Tiles[tiles[i / tileWidth, j / tileHeight]][i % tileWidth, j % tileHeight];
                    full[i, j] = filled ? '1' : '0';
                }
            }

            // Find sea monsters
            var pattern = new Regex($"1(?=.{{{width - 19}}}1.{{4}}11.{{4}}11.{{4}}111.{{{width - 19}}}1.{{2}}1.{{2}}1.{{2}}1.{{2}}1.{{2}}1)", RegexOptions.Compiled);
            var count = pattern.ToString().Count(c => c == '1');
            var success = full.TryAllOrientations(grid =>
            {
                var s = grid.ToArray().All(true).Str();
                return pattern.IsMatch(s);
            });
            if (!success) throw new Exception("No sea monsters found.");
            var str = full.ToArray().All(true).Str();
            var monsters = pattern.Matches(str).Count;
            var result = str.Count(c => c == '1') - monsters * count;
            WriteLn(result);
        }
    }
}