using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using RegExtract;

namespace AdventOfCode2017.Puzzles;

public class Day20 : Puzzle
{
    public Day20()
    {
        Part = 2;
    }

    public IEnumerable<Particle> ReadParticles()
    {
        return Input.Extract<Particle>(@"p=(?<Pos>.+?), v=(?<Velocity>.+?), a=(?<Acceleration>.+)")
            .Peek((particle, i) => particle.Num = i);
    }

    public override void PartOne()
    {
        var order = Comparing<Particle>.By(p => p.Acceleration.Magnitude())
            .ThenBy(p => p.Velocity.Magnitude())
            .ThenBy(p => p.Pos.Magnitude());
        var result = ReadParticles().OrderBy(order).First().Num;
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var particles = ReadParticles().ToList();
            
        foreach (var _ in Enumerable.Range(0, 1000))
        {
            foreach (var p in particles)
            {
                p.Step();
            }
            var remove = particles.GroupBy(p => p.Pos).SelectMany(g => g.Multiple()).ToList();
            foreach (var particle in remove)
            {
                particles.Remove(particle);
            }
        }

        WriteLn(particles.Count);
    }

    public class Particle
    {
        public int Num;
        public Pos3D Pos { get; set; }
        public Pos3D Velocity { get; set; }
        public Pos3D Acceleration { get; set; }

        public void Step()
        {
            Velocity += Acceleration;
            Pos += Velocity;
        }
    }
}