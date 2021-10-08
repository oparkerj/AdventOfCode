using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2018.Puzzles
{
    public class Day10 : Puzzle
    {
        public IEnumerable<Star> Stars()
        {
            return Input.Extract<Star>(@"position=<(?<Position>.+?)> velocity=<(?<Velocity>.+?)>");
        }

        public override void PartOne()
        {
            var stars = Stars().ToList();
            var rect = Rect.Bound(stars.Select(star => star.Position));
            var size = rect.LongArea;
            var next = size;
            var time = 0;

            while (next <= size)
            {
                size = next;
                foreach (var star in stars)
                {
                    star.Move();
                }
                time++;
                rect.Rebound(stars.Select(star => star.Position));
                next = rect.LongArea;
            }

            // As soon as the bounding box starts getting larger again,
            // go back one step to get the result.
            time--;
            stars.ForEach(star => star.Reverse());

            var result = stars.Select(star => star.Position).ToGrid().ToArray(false).Stringify(b => b ? '#' : ' ');
            WriteLn(result);
            WriteLn(time);
        }

        public class Star
        {
            public Pos Position { get; set; }
            public Pos Velocity { get; set; }

            public void Move() => Position += Velocity;

            public void Reverse() => Position -= Velocity;
        }
    }
}