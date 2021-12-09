using System;
using System.Linq;
using AdventToolkit;

namespace AdventOfCode2020.Puzzles;

public class Day9 : Puzzle
{
    public Day9()
    {
        Part = 2;
    }

    public bool Find(long[] last, long value)
    {
        var left = 0;
        var right = last.Length - 1;
        while (left < right)
        {
            var sum = last[left] + last[right];
            if (sum < value) left++;
            if (sum > value) right--;
            if (sum == value) return last[left] != last[right];
        }
        return false;
    }

    public void Insert(long[] arr, long value, long remove)
    {
        var index = Array.BinarySearch(arr, remove);
        arr[index] = value;
        Array.Sort(arr);
    }
        
    public override void PartOne()
    {
        var input = Input.Select(long.Parse).ToArray();
        var last = input[..25];
        Array.Sort(last);
        for (var i = 25; i < input.Length; i++)
        {
            var v = input[i];
            if (!Find(last, v))
            {
                WriteLn(v);
                return;
            }
            Insert(last, v, input[i - 25]);
        }
        WriteLn("None...");
    }

    public override void PartTwo()
    {
        var target = 466456641L;
        var input = Input.Select(long.Parse).ToArray();
        var start = 0;
        var end = 2;
        var sum = input[start..end].Sum();
        while (true)
        {
            if (sum < target) sum += input[end++];
            if (sum > target) sum -= input[start++];
            if (sum != target) continue;
            var range = input[start..end];
            WriteLn(range.Max() + range.Min());
            return;
        }
    }
}