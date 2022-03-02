using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2016.Puzzles;

public class Day10 : Puzzle
{
    public Dictionary<int, Bot> Bots = new();
    public Dictionary<int, List<int>> Outputs = new();

    private void Setup()
    {
        // Sorting the input means all the bots will be created before
        // trying to add any chips to them
        foreach (var s in Input.Sorted())
        {
            if (s.StartsWith("bot"))
            {
                var bot = new Bot(s);
                Bots[bot.Num] = bot;
            }
            else
            {
                var (value, id) = s.Extract<(int, int)>(@"value (\d+) goes to bot (\d+)");
                Bots[id].Receive(value);
            }
        }
    }
    
    public override void PartOne()
    {
        Setup();
        
        while (true)
        {
            foreach (var b in Bots.Values)
            {
                if (b.Part1Target)
                {
                    WriteLn(b.Num);
                    return;
                }
                b.Step(this);
            }
        }
    }

    public override void PartTwo()
    {
        Setup();
        
        while (!Outputs.ContainsKey(0) || !Outputs.ContainsKey(1) || !Outputs.ContainsKey(2))
        {
            foreach (var b in Bots.Values)
            {
                b.Step(this);
            }
        }
        
        WriteLn(Outputs[0].First() * Outputs[1].First() * Outputs[2].First());
    }

    public class Bot
    {
        public readonly int Num;
        public readonly int Low;
        public readonly int High;
        public readonly bool LowOutput;
        public readonly bool HighOutput;

        public int Chip1 { get; set; } = -1;
        public int Chip2 { get; set; } = -1;

        public Bot(string spec)
        {
            var (id, lowTarget, low, highTarget, high) = spec.Extract<(int, string, int, string, int)>(@"bot (\d+) gives low to (\w+) (\d+) and high to (\w+) (\d+)");
            Num = id;
            Low = low;
            High = high;
            LowOutput = lowTarget == "output";
            HighOutput = highTarget == "output";
        }

        public void Receive(int chip)
        {
            if (Chip1 < 0) Chip1 = chip;
            else if (Chip2 < 0) Chip2 = chip;
            else throw new Exception("Bot can't receive chip.");
        }

        public bool Part1Target => Chip1 >= 0 && Chip2 >= 0 && ((Chip1 == 61 && Chip2 == 17) || (Chip1 == 17 && Chip2 == 61));

        public void Step(Day10 data)
        {
            if (Chip1 < 0 || Chip2 < 0) return;
            var (low, high) = Chip1 < Chip2 ? (Chip1, Chip2) : (Chip2, Chip1);
            if (LowOutput) data.Outputs.GetOrSetValue(Low, () => new List<int>()).Add(low);
            else data.Bots[Low].Receive(low);
            if (HighOutput) data.Outputs.GetOrSetValue(High, () => new List<int>()).Add(high);
            else data.Bots[High].Receive(high);
            Chip1 = -1;
            Chip2 = -2;
        }
    }
}