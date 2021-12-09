using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2020.Puzzles;

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
        var variables = mask.IndicesOf('X').ToArray();
        var str = mask.ToCharArray();
        foreach (var combination in Algorithms.Sequences(variables.Length, 2, true))
        {
            for (var i = 0; i < variables.Length; i++)
            {
                str[variables[i]] = combination[i].ToString()[0];
            }
            yield return new string(str);
        }
    }
}