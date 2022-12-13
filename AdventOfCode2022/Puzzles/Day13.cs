using System.Collections;
using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day13 : Puzzle<int>
{
    public IListOrInt Parse(string s)
    {
        if (s.StartsWith('['))
        {
            var inner = s[1..^1].SplitOuter(',', '[', ']');
            return new IntList {Stuff = inner.Select(Parse).ToList()};
        }
        return new Int {Value = s.AsInt()};
    }

    public bool? InOrder(IListOrInt left, IListOrInt right)
    {
        if (left is Int li && right is Int ri)
        {
            if (li.Value < ri.Value) return true;
            if (li.Value > ri.Value) return false;
            return null;
        }
        if (left is IntList ll && right is IntList rl)
        {
            foreach (var (lz, rz) in ll.Stuff.ZipLongest(rl.Stuff, (ls, rs) => (ls, rs)))
            {
                if (lz == null && rz != null) return true;
                if (lz != null && rz == null) return false;
                if (InOrder(lz, rz) is { } result) return result;
            }
            return null;
        }
        var lt = left is Int leftInt ? new IntList {Stuff = new List<IListOrInt> {leftInt}} : left;
        var rt = right is Int rightInt ? new IntList {Stuff = new List<IListOrInt> {rightInt}} : right;
        return InOrder(lt, rt);
    }
    
    public override int PartOne()
    {
        return AllGroups.Select(pair => (left: Parse(pair[0]), right: Parse(pair[1])))
            .Index()
            .Where(item => InOrder(item.Value.left, item.Value.right) == true)
            .Keys()
            .Select(i => i + 1)
            .Sum();
    }

    public override int PartTwo()
    {
        var list = Input.Where(s => s != "")
            .Then("[[2]]")
            .Then("[[6]]")
            .Select(Parse)
            .ToList();
        list.Sort((a, b) =>
        {
            var c = InOrder(a, b);
            if (c == true) return -1;
            if (c == null) return 0;
            return 1;
        });
        
        var twoPacket = list.FindIndex(i => i is IntList {Stuff: [IntList {Stuff: [Int {Value: 2}]}]});
        var sixPacket = list.FindIndex(i => i is IntList {Stuff: [IntList {Stuff: [Int {Value: 6}]}]});
        return (twoPacket + 1) * (sixPacket + 1);
    }

    public interface IListOrInt : IEnumerable<int> { }

    public class IntList : IListOrInt
    {
        public List<IListOrInt> Stuff { get; set; }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<int> GetEnumerator()
        {
            return Stuff.SelectMany(i => i).GetEnumerator();
        }
    }
    
    public class Int : IListOrInt
    {
        public int Value { get; set; }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<int> GetEnumerator()
        {
            yield return Value;
        }
    }
}