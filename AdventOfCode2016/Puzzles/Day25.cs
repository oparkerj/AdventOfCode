namespace AdventOfCode2016.Puzzles;

public class Day25 : Day12
{
    private int Last;
    private bool Valid;

    public Day25()
    {
        Part = 1;
    }

    public override void Execute(string inst)
    {
        // Hard code parts of the program that are done with loops
        if (Ptr == 3)
        {
            Regs[3] += Regs[1] * Regs[2];
            Regs[1] = 0;
            Regs[2] = 0;
            Ptr = 7;
        }
        else if (Ptr == 13)
        {
            Regs[0] += Regs[1] / 2;
            Regs[2] = Regs[1] % 2;
            Regs[1] = 0;
            if (Regs[2] == 0) Regs[2] = 2;
            Ptr = 19;
        }
        else if (Ptr == 21)
        {
            Regs[1] -= Regs[2];
            Regs[2] = 0;
            Ptr = 25;
        }
        else if (inst.StartsWith("out"))
        {
            var value = (int) Val(inst[4..]);
            if (value == Last) Valid = false;
            Last = value;
        }
        else base.Execute(inst);
    }

    public (int, string) State => (Ptr, string.Join(',', Regs));

    public override void PartOne()
    {
        var states = new HashSet<(int, string)>();

        var a = 1;
        while (true)
        {
            states.Clear();
            Regs[0] = a;
            Regs[1] = Regs[2] = Regs[3] = 0;
            Ptr = 0;
            Last = 1;
            Valid = true;

            while (Ptr < Input.Length)
            {
                var state = State;
                if (states.Contains(state))
                {
                    if (Valid)
                    {
                        WriteLn(a);
                        return;
                    }
                    break;
                }
                Execute(Input[Ptr]);
                if (!Valid) break;
                Ptr++;
                states.Add(state);
            }
            a++;
        }
    }
}