using System.Collections.Concurrent;
using System.Numerics;
using System.Threading;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;

namespace AdventOfCode2017.Puzzles
{
    public class Day18 : Puzzle
    {

        public Day18()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var program = new Program(Input);
            program.Run();
            WriteLn(program.Last);
        }

        public override void PartTwo()
        {
            var a = new Program(Input);
            var b = new Program(Input);
            a.Target = b;
            b.Target = a;
            b.Reg['p'] = 1;
            var at = new Thread(a.Run);
            var bt = new Thread(b.Run);
            at.Start();
            bt.Start();
            at.Join();
            bt.Join();
            WriteLn(b.Sends);
        }

        public class Program
        {
            public readonly DefaultDict<char, BigInteger> Reg = new();
            public readonly BlockingCollection<BigInteger> Data = new();
            public string[] Input;

            public Program(string[] input) => Input = input;

            private int _waiting;
            public bool Waiting
            {
                get => _waiting.AsBool();
                set => Interlocked.Exchange(ref _waiting, value.AsInt());
            }

            public Program Target;
            public bool Terminate;

            public BigInteger Last;
            public int Sends;

            public BigInteger Read(string val)
            {
                if (int.TryParse(val, out var i)) return i;
                return Reg[val![0]];
            }
            
            public int Exec(string inst)
            {
                var op = inst[..3];
                var args = inst.Split(' ')[1..];
                var x = args[0][0];
                if (op == "snd")
                {
                    if (Target == null) Last = Read(args[0]);
                    else Target.Data.Add(Read(args[0]));
                    Sends++;
                }
                else if (op == "set") Reg[x] = Read(args[1]);
                else if (op == "add") Reg[x] += Read(args[1]);
                else if (op == "mul") Reg[x] *= Read(args[1]);
                else if (op == "mod") Reg[x] %= Read(args[1]);
                else if (op == "rcv")
                {
                    if (Target == null)
                    {
                        if (Read(args[0]) != 0) Terminate = true;
                    }
                    else
                    {
                        if (Target.Waiting && Target.Data.Count == 0 && Data.Count == 0)
                        {
                            Terminate = true;
                            Target.Data.Add(0);
                        }
                        else
                        {
                            Waiting = true;
                            Reg[x] = Data.Take();
                            if (Target.Terminate) Terminate = true;
                            Waiting = false;
                        }
                    }
                }
                else if (op == "jgz")
                {
                    if (Read(args[0]) > 0) return (int) Read(args[1]);
                }
                return 1;
            }

            public void Run()
            {
                var ptr = 0;
                while (!Terminate)
                {
                    ptr += Exec(Input[ptr]);
                }
            }
        }
    }
}