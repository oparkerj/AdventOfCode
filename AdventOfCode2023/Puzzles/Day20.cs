using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day20 : Puzzle<long>
{
    public ModuleManager SetupManager()
    {
        var manager = new ModuleManager();
        
        foreach (var s in Input)
        {
            var name = s.Before(' ');
            if (!char.IsLetter(s[0]))
            {
                name = name[1..];
            }
            IModule module = s[0] switch
            {
                '%' => new FlipFlop(name),
                '&' => new Conjunction(name),
                _ => new Broadcaster(name)
            };
            var outputs = s.After('>').Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            module.Outputs.UnionWith(outputs);
            manager.Modules[name] = module;
        }
        
        // Conjunctions need to know their inputs
        foreach (var conjunction in manager.Modules.Values.WhereType<Conjunction>())
        {
            foreach (var module in manager.Modules.Values)
            {
                if (!module.Outputs.Contains(conjunction.Name)) continue;
                conjunction.Inputs[module.Name] = false;
            }
        }
        
        return manager;
    }
    
    public override long PartOne()
    {
        var manager = SetupManager();

        var low = 0L;
        var high = 0L;
        manager.OnPulse += (_, _, highPulse) =>
        {
            if (highPulse) high++;
            else low++;
        };
        
        for (var i = 0; i < 1000; i++)
        {
            manager.Send("", "broadcaster", false);
            manager.ProcessPulses();
        }
        
        return low * high;
    }

    public override long PartTwo()
    {
        var manager = SetupManager();
        var modules = manager.Modules;

        // The input is crafted to behave in a special way, this detects the answer for the special input
        var result = 1L;
        foreach (var section in modules["broadcaster"].Outputs)
        {
            var start = modules[section];
            IModule end = start.Get<Conjunction>(manager)!;
            IModule? next = start.Get<FlipFlop>(manager);
            List<bool> bits = [true];
            while (next is not null)
            {
                bits.Add(!end.Outputs.Contains(next.Name));
                next = next.Get<FlipFlop>(manager);
            }
            bits.Reverse();
            result *= bits.BitsToInt();
        }
        return result;
    }

    public class ModuleManager
    {
        public Dictionary<string, IModule> Modules = new();

        public Queue<(string From, string To, bool High)> Pulses = new();

        public event Action<string, string, bool>? OnPulse;

        public void Send(string from, string target, bool high)
        {
            OnPulse?.Invoke(from, target, high);
            Pulses.Enqueue((from, target, high));
        }

        public void SendAll(IModule module, bool high)
        {
            foreach (var output in module.Outputs)
            {
                Send(module.Name, output, high);
            }
        }

        public void ProcessPulses()
        {
            while (Pulses.Count > 0)
            {
                var (from, to, high) = Pulses.Dequeue();
                if (Modules.TryGetValue(to, out var module))
                {
                    module.Receive(this, from, high);
                }
            }
        }
    }

    public interface IModule
    {
        string Name { get; }
        
        HashSet<string> Outputs { get; }

        void Receive(ModuleManager manager, string from, bool high);

        IEnumerable<T> GetAll<T>(ModuleManager manager)
        {
            return Outputs.Select(s => manager.Modules[s]).WhereType<T>();
        }

        T? Get<T>(ModuleManager manager) => GetAll<T>(manager).FirstOrDefault();
    }

    public class FlipFlop : IModule
    {
        public string Name { get; }
        
        public HashSet<string> Outputs { get; }
        
        public bool IsOn { get; private set; }

        public FlipFlop(string name)
        {
            Name = name;
            Outputs = [];
        }

        public void Receive(ModuleManager manager, string from, bool high)
        {
            if (!high)
            {
                IsOn = !IsOn;
                manager.SendAll(this, IsOn);
            }
        }
    }

    public class Conjunction : IModule
    {
        public string Name { get; }
        
        public HashSet<string> Outputs { get; }

        public Conjunction(string name)
        {
            Name = name;
            Outputs = [];
        }

        public Dictionary<string, bool> Inputs = new();
        
        public void Receive(ModuleManager manager, string from, bool high)
        {
            Inputs[from] = high;
            manager.SendAll(this, !Inputs.Values.All(b => b));
        }
    }

    public class Broadcaster : IModule
    {
        public string Name { get; }
        
        public HashSet<string> Outputs { get; }

        public Broadcaster(string name)
        {
            Name = name;
            Outputs = [];
        }

        public void Receive(ModuleManager manager, string from, bool high)
        {
            manager.SendAll(this, high);
        }
    }
}