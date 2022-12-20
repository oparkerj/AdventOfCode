using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day20 : Puzzle<int, long>
{
    public override int PartOne()
    {
        var nums = Input.Ints().IndexedTuple().ToList();
        foreach (var i in Enumerable.Range(0, nums.Count))
        {
            var index = nums.FirstIndex(tuple => tuple.Index == i);
            var n = nums[index].Value;
            nums.RemoveAt(index);
            var insert = (index + n).CircularMod(nums.Count);
            nums.Insert(insert, (i, n));
        }

        var zero = nums.FindIndex(tuple => tuple.Value == 0);
        var first = nums[(zero + 1000).CircularMod(nums.Count)].Value;
        var second = nums[(zero + 2000).CircularMod(nums.Count)].Value;
        var third = nums[(zero + 3000).CircularMod(nums.Count)].Value;
        
        return first + second + third;
    }

    public override long PartTwo()
    {
        const long key = 811589153;
        var nums = Input.Longs().Select(n => n * key).IndexedTuple().ToList();
        foreach (var _ in Enumerable.Range(0, 10))
        {
            foreach (var i in Enumerable.Range(0, nums.Count))
            {
                var index = nums.FirstIndex(tuple => tuple.Index == i);
                var n = nums[index].Value;
                nums.RemoveAt(index);
                var insert = (index + n).CircularMod(nums.Count);
                nums.Insert((int) insert, (i, n));
            }
        }
        
        var zero = nums.FindIndex(tuple => tuple.Value == 0);
        var first = nums[(zero + 1000).CircularMod(nums.Count)].Value;
        var second = nums[(zero + 2000).CircularMod(nums.Count)].Value;
        var third = nums[(zero + 3000).CircularMod(nums.Count)].Value;
        
        return first + second + third;
    }
}