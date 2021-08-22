using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventToolkit.Extensions;

namespace AdventOfCode2019.IntCode
{
    public class Network
    {
        private readonly List<Computer> _computers = new();
        private readonly Dictionary<int, DataLink> _inputs = new();
        private Action<Network, Dictionary<int, DataLink>> _setup;
        private int _running;

        public bool Running
        {
            get => _running != 0;
            set => Interlocked.Exchange(ref _running, value ? 1 : 0);
        }

        public int Count => _computers.Count;

        public IReadOnlyDictionary<int, DataLink> Inputs => _inputs;

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

        public void AddCount(string program, int count, bool addInput = false, Action<DataLink> action = null)
        {
            count.Times(() =>
            {
                var id = Add(program);
                if (addInput)
                {
                    var data = new DataLink(_computers[id]);
                    _inputs[id] = data;
                    action?.Invoke(data);
                }
            });
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

        public DataLink GetLink(int id)
        {
            if (_inputs.TryGetValue(id, out var link)) return link;
            return _inputs[id] = new DataLink();
        }

        public void SetOutput(Func<Action<long>> outputs)
        {
            foreach (var computer in _computers)
            {
                computer.LineOut = outputs();
            }
        }

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

        public void SendPacket(int id, params long[] data)
        {
            Console.WriteLine(id);
            if (_inputs.TryGetValue(id, out var link))
            {
                foreach (var d in data)
                {
                    link.Insert(d);
                }
            }
        }

        public void StopAll()
        {
            foreach (var c in _computers)
            {
                c.Interrupt = true;
            }
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

        public Task<long> RunUntilOutputAsync(int id, int skip = 0)
        {
            if (!_inputs.TryGetValue(id, out var link))
            {
                link = _inputs[id] = new DataLink();
            }
            _setup?.Invoke(this, _inputs);
            return Task.Run(async () =>
            {
                var computers = Task.WhenAll(_computers.Select(c => c.ExecuteAsync()));
                long result = 0;
                while (skip-- + 1 > 0) result = link.Take();
                StopAll();
                await computers;
                return result;
            });
        }

        public Task RunAllAsync()
        {
            _setup?.Invoke(this, _inputs);
            return Task.WhenAll(_computers.Select(c => c.ExecuteAsync()));
        }
    }
}