using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2018.Puzzles
{
    public class Day13 : Puzzle
    {
        public readonly Grid<char> Map;
        public readonly List<Cart> Carts;
        public readonly IComparer<Cart> Comparer = Pos.ReadingOrder.SelectFrom<Pos, Cart>(cart => cart.Position);

        public Day13()
        {
            Map = Input.ToGrid();
            Carts = Map.WhereValue(IsCart)
                .Select(pair => new Cart
                {
                    Position = pair.Key,
                    Direction = GetDir(pair.Value)
                }).ToList();
            foreach (var cart in Carts)
            {
                if (cart.Direction.Dot(Pos.Up) == 0) Map[cart.Position] = '-';
                else Map[cart.Position] = '|';
            }
            Part = 2;
        }

        public Pos GetDir(char c)
        {
            return c switch
            {
                '<' => Pos.Left,
                '>' => Pos.Right,
                '^' => Pos.Up,
                'v' => Pos.Down,
                _ => default
            };
        }

        public static bool IsCart(char c)
        {
            return c is '<' or '>' or '^' or 'v';
        }

        // Step carts once, returns true if there was a collision, along with the position.
        public bool Step(out Pos pos)
        {
            Carts.Sort(Comparer);
            pos = default;
            var b = false;
            for (var i = 0; i < Carts.Count; i++)
            {
                var cart = Carts[i];
                var hit = cart.Advance(Map, Carts);
                if (hit && !b)
                {
                    b = true;
                    pos = cart.Position;
                }
                if (hit) Carts.RemoveConcurrent(c => c.Position == cart.Position, ref i);
            }
            return b;
        }

        public override void PartOne()
        {
            while (true)
            {
                if (Step(out var p))
                {
                    WriteLn(p.Invert());
                    return;
                }
            }
        }

        public override void PartTwo()
        {
            while (Carts.Count > 1)
            {
                Step(out _);
            }
            WriteLn(Carts[0].Position.Invert());
        }

        public class Cart
        {
            public Pos Position;
            public Pos Direction;
            public int Turn = -1;

            public bool Advance(Grid<char> map, List<Cart> carts)
            {
                Position += Direction;
                if (carts.Without(this).Any(cart => cart.Position == Position)) return true;
                var c = map[Position];
                if (c == '\\') Direction = -Direction.Flip();
                else if (c == '/') Direction = Direction.Flip();
                else if (c == '+')
                {
                    Direction = Direction.Turn(Turn);
                    if (++Turn == 2) Turn = -1;
                }
                return false;
            }
        }
    }
}