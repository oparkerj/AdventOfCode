using System.Linq;

namespace AdventOfCode2021.Puzzles;

public class Day18 : Puzzle
{
    public Day18()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var result = Input.Select(Pair.Parse).Aggregate(Pair.Add);
        WriteLn(result.Magnitude());
    }

    public override void PartTwo()
    {
        // Parse the pair each time because addition modifies the pair instance
        var max = Algorithms.ExclusivePairs(Input.Length, true)
            .Max(which => Input.Get(which).Select(Pair.Parse).Aggregate(Pair.Add).Magnitude());
        WriteLn(max);
    }

    public class Pair
    {
        private Pair _left;
        private Pair _right;
        public Pair Parent { get; private set; }
        public int Value { get; set; }

        public Pair Left
        {
            get => _left;
            set => Set(ref _left, value);
        }

        public Pair Right
        {
            get => _right;
            set => Set(ref _right, value);
        }

        public Pair(int value) => Value = value;

        public Pair(Pair left, Pair right)
        {
            Left = left;
            Right = right;
        }

        public static Pair Parse(string s)
        {
            var (left, right) = s[1..^1].SplitOuter(',', '[', ']').ToPair();
            var leftValue = left.StartsWith('[') ? Parse(left) : left.AsInt();
            var rightValue = right.StartsWith('[') ? Parse(right) : right.AsInt();
            return new Pair(leftValue, rightValue);
        }

        public static implicit operator Pair(int i) => new(i);

        private void ReplaceWith(Pair b)
        {
            var parent = Parent;
            Parent = null;
            parent.Remove(this);
            if (parent._left == null) parent.Left = b;
            if (parent._right == null) parent.Right = b;
        }
        
        private void Remove(Pair child)
        {
            if (_left == child) _left = null;
            if (_right == child) _right = null;
        }

        private void Set(ref Pair pair, Pair value)
        {
            if (pair != null) pair.Parent = null;
            value?.Parent?.Remove(value);
            if (value != null) value.Parent = this;
            pair = value;
        }

        private Pair Get(bool left) => left ? Left : Right;

        public bool IsRegular => _left == null;

        public Pair GetPair(bool left) => Parent?.Get(!left) == this ? Parent.Get(left) : Parent?.GetPair(left);

        public Pair FindNumber(bool left, bool dfs = false)
        {
            if (!dfs) return GetPair(left)?.FindNumber(left, true);
            return IsRegular ? this : Get(!left).FindNumber(left, true) ?? Get(left).FindNumber(left, true);
        }
        
        public bool Explode(int level = 0)
        {
            if (IsRegular) return false;
            if (level != 4) return _left.Explode(level + 1) || _right.Explode(level + 1);
            var left = FindNumber(true);
            if (left != null) left.Value += _left.Value;
            var right = FindNumber(false);
            if (right != null) right.Value += _right.Value;
            ReplaceWith(0);
            return true;
        }
        
        public bool Split()
        {
            if (!IsRegular) return _left.Split() || _right.Split();
            if (Value < 10) return false;
            ReplaceWith(new Pair(Value / 2, (Value + 1) / 2));
            return true;
        }

        public Pair Reduce()
        {
            while (Explode() || Split()) { }
            return this;
        }

        // Note: this operation is destructive on the original instance.
        // Don't reuse a pair for a different independent addition.
        public static Pair Add(Pair left, Pair right) => new Pair(left, right).Reduce();

        public int Magnitude() => IsRegular ? Value : _left.Magnitude() * 3 + _right.Magnitude() * 2;

        public override string ToString() => IsRegular ? Value.ToString() : $"[{Left},{Right}]";
    }
}