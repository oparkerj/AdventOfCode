using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2020.Puzzles
{
    public class Day14 : Puzzle
    {
        public Day14()
        {
            Part = 2;
        }

        public Dictionary<long, long> Memory = new();

        public long Zeroes(string mask)
        {
            mask = Regex.Replace(mask, "[^0]", "1");
            return Convert.ToInt64(mask, 2);
        }

        public long Ones(string mask)
        {
            mask = Regex.Replace(mask, "[^1]", "0");
            return Convert.ToInt64(mask, 2);
        }

        public override void PartOne()
        {
            var zeroes = 0L;
            var ones = 0L;
            foreach (var line in Input)
            {
                if (line.StartsWith("mask"))
                {
                    var mask = line[7..];
                    zeroes = Zeroes(mask);
                    ones = Ones(mask);
                }
                else
                {
                    var (addr, v) = line.Extract<(int, long)>(@"^mem\[(\d+)\] = (.+)$");
                    v &= zeroes;
                    v |= ones;
                    Memory[addr] = v;
                }
            }
            WriteLn(Memory.Values.Sum());
        }

        public string Modify(string mask, long addr)
        {
            var s = Convert.ToString(addr, 2);
            s = Enumerable.Repeat('0', 36 - s.Length).Str() + s;
            return mask.Select((c, i) => c != 'X' ? (c == '1' ? '1' : s[i]) : 'X').Str();
        }

        public override void PartTwo()
        {
            var mask = "";
            foreach (var line in Input)
            {
                if (line.StartsWith("mask"))
                {
                    mask = line[7..];
                }
                else
                {
                    var (addr, v) = line.Extract<(int, long)>(@"^mem\[(\d+)\] = (.+)$");
                    var tm = Modify(mask, addr);
                    foreach (var p in Permutations(tm))
                    {
                        Memory[Convert.ToInt64(p, 2)] = v;
                    }
                }
            }
            WriteLn(Memory.Values.Sum());
        }

        public IEnumerable<string> Permutations(string mask)
        {
            // TODO make custom permutation function
            var pos = mask.Select((c, i) => i).Where(i => mask[i] == 'X').ToArray();
            foreach (var seq in printSequences(2, pos.Length))
            {
                var s = seq.Select(i => i - 1).ToArray();
                var values = pos.Select((i, ind) => (i, s[ind])).ToArray();
                var str = mask.Select((c, i) => pos.Contains(i) ? values.First(tuple => tuple.i == i).Item2.ToString()[0] : c).Str();
                yield return str;
            }
        }

        static int getSuccessor(int[] arr, int k, int n)  
        { 
            /* start from the rightmost side and 
            find the first number less than n */
            int p = k - 1; 
            while (arr[p] == n) 
            { 
                p--; 
                if (p < 0) 
                { 
                    break; 
                } 
            } 
  
            /* If all numbers are n in the array 
            then there is no successor, return 0 */
            if (p < 0) 
            { 
                return 0; 
            } 
  
            /* Update []arr so that it contains successor */
            arr[p] = arr[p] + 1; 
            for (int i = p + 1; i < k; i++)  
            {
                arr[i] = 1;
            }
            return 1; 
        } 
  
        /* The main function that prints all 
        sequences from 1, 1, ..1 to n, n, ..n */
        static IEnumerable<int[]> printSequences(int n, int k)  
        { 
            int[] arr = new int[k]; 
  
            /* Initialize the current sequence as  
            the first sequence to be printed */
            for (int i = 0; i < k; i++)  
            { 
                arr[i] = 1; 
            } 
  
            /* The loop breaks when there are  
            no more successors to be printed */
            while (true)  
            { 
                /* Print the current sequence */
                yield return arr;
  
                /* Update []arr so that it contains  
                next sequence to be printed. And if 
                there are no more sequences then 
                break the loop */
                if (getSuccessor(arr, k, n) == 0) 
                { 
                    break; 
                } 
            } 
        } 
    }
}