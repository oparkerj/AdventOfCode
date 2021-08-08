using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode2019
{
    public class Network
    {
        private readonly List<Computer> _computers = new();
        private readonly Dictionary<int, DataLink> _inputs = new();
        private Action<Network, Dictionary<int, DataLink>> _setup;

        public int Count => _computers.Count;

        public int Add(string program)
        {
            var id = _computers.Count;
            _computers.Add(Computer.From(program));
            return id;
        }

        public void AddSeries(string program, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var id = Add(program);
                if (i > 0) Link(i - 1, id);
            }
        }
        
        public void AddLoop(string program, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var id = Add(program);
                if (i > 0) Link(i - 1, id);
            }
            Link(count - 1, 0);
        }

        private DataLink LinkInternal(int a, int b)
        {
            var data = new DataLink();
            data.Link(_computers[a], _computers[b]);
            _inputs[b] = data;
            return data;
        }

        public void Link(int a, int b) => LinkInternal(a, b);

        public void Link(int a, int b, long initial) => LinkInternal(a, b).Insert(initial);

        public static Action<Network, Dictionary<int, DataLink>> Insert(IList<long> data, bool create = true)
        {
            return (network, links) =>
            {
                for (var i = 0; i < data.Count; i++)
                {
                    if (links.TryGetValue(i, out var link))
                    {
                        link.Insert(data[i]);
                    }
                    else if (create)
                    {
                        link = network._inputs[i] = new DataLink(network._computers[i]);
                        link.Insert(data[i]);
                    }
                }
            };
        }

        public static Action<Network, Dictionary<int, DataLink>> Insert(int id, long data)
        {
            return (_, links) => links[id].Insert(data);
        }

        public Network WithSetup(Action<Network, Dictionary<int, DataLink>> setup)
        {
            _setup = setup;
            return this;
        }

        public Network WithSetup(params Action<Network, Dictionary<int, DataLink>>[] setups)
        {
            _setup = (network, links) =>
            {
                foreach (var action in setups)
                {
                    action(network, links);
                }
            };
            return this;
        }

        public long RunSeries()
        {
            _setup?.Invoke(this, _inputs);
            var output = new DataLink();
            _computers[^1].LineOut = output.Input;
            foreach (var computer in _computers)
            {
                computer.Execute();
            }
            output.TryTake(out var result);
            _computers[^1].LineOut = null;
            return result;
        }

        public Task<long> RunLoopAsync()
        {
            _setup?.Invoke(this, _inputs);
            return Task.Run(async () =>
            {
                var output = _inputs[0];
                await Task.WhenAll(_computers.Select(c => c.ExecuteAsync()));
                output.TryTake(out var result);
                return result;
            });
        }
    }
}