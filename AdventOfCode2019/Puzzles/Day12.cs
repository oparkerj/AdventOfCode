using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Solvers;
using RegExtract;

namespace AdventOfCode2019.Puzzles
{
    public class Day12 : Puzzle
    {
        public Day12()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var moons = Input.Extract<Pos3D>(@"<x=(.+), y=(.+), z=(.+)>")
                .Select(pos => new Moon {Position = pos});
            var sim = new Simulation<Moon>(moons)
                .WithUpdate(Simulation<Moon>.Aggregate((a, b) => b.Position.Sub(a.Position).Normalize(), (moon, gravity) => moon.Velocity += gravity))
                .WithApply(moon => moon.Position += moon.Velocity);
            1000.Times(sim.Step);
            WriteLn(sim.Objects.Select(moon => moon.TotalEnergy).Sum());
        }

        public override void PartTwo()
        {
            var moons = Input.Extract<Pos3D>(@"<x=(.+), y=(.+), z=(.+)>")
                .Select(pos => new Moon {Position = pos});
            var sim = new Simulation<Moon>(moons)
                .WithUpdate(Simulation<Moon>.Aggregate((a, b) => b.Position.Sub(a.Position).Normalize(), (moon, gravity) => moon.Velocity += gravity))
                .WithApply(moon => moon.Position += moon.Velocity);

            static IEnumerable<string> GetComponents(Simulation<Moon> s)
            {
                yield return string.Concat(s.Objects.Select(m => $"{m.Position.X}{m.Velocity.X}"));
                yield return string.Concat(s.Objects.Select(m => $"{m.Position.Y}{m.Velocity.Y}"));
                yield return string.Concat(s.Objects.Select(m => $"{m.Position.Z}{m.Velocity.Z}"));
            }
            var cycle = Algorithms.CommonCycle(sim, GetComponents, s => s.Step());
            WriteLn(cycle);
        }

        public class Moon
        {
            public Pos3D Position;
            public Pos3D Velocity;

            public int PotentialEnergy => Position.MDist(Pos3D.Origin);

            public int KineticEnergy => Velocity.MDist(Pos3D.Origin);

            public int TotalEnergy => PotentialEnergy * KineticEnergy;
        }
    }
}