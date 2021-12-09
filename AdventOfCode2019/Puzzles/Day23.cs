using System.Linq;
using System.Threading;
using AdventOfCode2019.IntCode;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2019.Puzzles;

public class Day23 : Puzzle
{
    public Day23()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var network = new Network();
        network.AddCount(InputLine, 50, true, data => data.FallbackOutput = () =>
        {
            // Prevent 100% cpu, so that other computers can do their work
            Thread.Sleep(1);
            return -1;
        });
        network.SetOutput(() =>
        {
            return new OutputSequence().ThenMultiple(3, data =>
            {
                network.SendPacket((int) data[0], data[1..]);
            }).Line;
        });
        network.WithSetup(Network.Insert(Enumerable.Range(0, 50).Longs().ToList()));
        var task = network.RunUntilOutputAsync(255, 1);
        var result = task.GetAwaiter().GetResult();
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var network = new Network();
        network.AddCount(InputLine, 50, true, data => data.FallbackOutput = () =>
        {
            // Prevent 100% cpu, so that other computers can do their work
            Thread.Sleep(1);
            return -1;
        });
        network.SetOutput(() =>
        {
            return new OutputSequence().ThenMultiple(3, data =>
            {
                network.SendPacket((int) data[0], data[1..]);
            }).Line;
        });
        network.WithSetup(Network.Insert(Enumerable.Range(0, 50).Longs().ToList()));
        var nat = network.GetLink(255);
        var computers = network.RunAllAsync();

        var lastX = 0L;
        var lastY = 0L;
        var sendY = -1L;
            
        var natTask = Async.RunUntil((out long r) =>
        {
            Thread.Sleep(100);
            if (nat.TryTake(out var x))
            {
                var y = nat.Take();
                Interlocked.Exchange(ref lastX, x);
                Interlocked.Exchange(ref lastY, y);
            }
            if (network.Inputs.Values.All(link => link.Count == 0))
            {
                if (lastY == sendY)
                {
                    r = sendY;
                    return true;
                }
                network.SendPacket(0, lastX, lastY);
                Interlocked.Exchange(ref sendY, lastY);
            }
            r = 0;
            return false;
        });
        var result = natTask.GetAwaiter().GetResult();
        network.StopAll();
        computers.Wait();
        WriteLn(result);
    }
}