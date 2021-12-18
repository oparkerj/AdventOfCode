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
            set
            {
                if (_left != null) _left.Parent = null;
                value?.Parent?.Remove(value);
                if (value != null) value.Parent = this;
                _left = value;
            }
        }

        public Pair Right
        {
            get => _right;
            set
            {
                if (_right != null) _right.Parent = null;
                value?.Parent?.Remove(value);
                if (value != null) value.Parent = this;
                _right = value;
            }
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

        public bool IsRegular => _left == null;

        // Get the first pair/value to the left of this, or null
        public Pair GetLeft()
        {
            if (Parent == null) return null;
            var parent = Parent;
            return parent._right == this ? parent._left : parent.GetLeft();
        }

        // Get the first pair/value to the right of this, or null
        public Pair GetRight()
        {
            if (Parent == null) return null;
            var parent = Parent;
            return parent._left == this ? parent._right : parent.GetRight();
        }

        // Find the first regular number to the left of this, or null 
        public Pair FindLeftNumber(bool dfs = false)
        {
            if (!dfs) return GetLeft()?.FindLeftNumber(true);
            if (IsRegular) return this;
            return _right.FindLeftNumber(true) ?? _left.FindLeftNumber(true);
        }

        // Find the first regular number to the right of this, or null
        public Pair FindRightNumber(bool dfs = false)
        {
            if (!dfs) return GetRight()?.FindRightNumber(true);
            if (IsRegular) return this;
            return _left.FindRightNumber(true) ?? _right.FindRightNumber(true);
        }

        // Find the first pair nested 4 levels deep, or null
        public Pair GetNested(int level = 0)
        {
            if (IsRegular) return null;
            if (level == 4) return this;
            return _left.GetNested(level + 1) ?? _right.GetNested(level + 1);
        }

        // Find the first value >= 10, or null
        public Pair GetBigValue()
        {
            if (!IsRegular) return _left.GetBigValue() ?? _right.GetBigValue();
            return Value >= 10 ? this : null;
        }

        public Pair Reduce()
        {
            while (true)
            {
                var current = GetNested();
                if (current != null)
                {
                    var left = current.FindLeftNumber();
                    if (left != null) left.Value += current.Left.Value;
                    var right = current.FindRightNumber();
                    if (right != null) right.Value += current.Right.Value;
                    current.ReplaceWith(0);
                    continue;
                }
                current = GetBigValue();
                if (current != null)
                {
                    current.ReplaceWith(new Pair(current.Value / 2, (current.Value + 1) / 2));
                    continue;
                }
                break;
            }
            return this;
        }

        // Note: this operation is destructive on the original instance.
        // Don't reuse a pair for a different independent addition.
        public static Pair Add(Pair left, Pair right) => new Pair(left, right).Reduce();

        public int Magnitude()
        {
            return IsRegular ? Value : _left.Magnitude() * 3 + _right.Magnitude() * 2;
        }

        public override string ToString() => IsRegular ? Value.ToString() : $"[{Left},{Right}]";
    }
}