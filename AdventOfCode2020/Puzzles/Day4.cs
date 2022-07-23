using System;
using System.Linq;
using System.Text;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles;

public class Day4 : Puzzle
{
    public Day4()
    {
        Part = 2;
    }

    [Flags]
    public enum Field
    {
        None = 0,
        Byr = 1,
        Iyr = 1 << 1,
        Eyr = 1 << 2,
        Hgt = 1 << 3,
        Hcl = 1 << 4,
        Ecl = 1 << 5,
        Pid = 1 << 6,
        Cid = 1 << 7,
        All = 255
    }

    public bool Valid(string passport)
    {
        var parts = passport.Split(' ');
        var valid = parts.Select(part => part.Split(':')[0])
            .Aggregate(Field.None, (current, field) => current | field switch
            {
                "byr" => Field.Byr,
                "iyr" => Field.Iyr,
                "eyr" => Field.Eyr,
                "hgt" => Field.Hgt,
                "hcl" => Field.Hcl,
                "ecl" => Field.Ecl,
                "pid" => Field.Pid,
                "cid" => Field.Cid,
                _ => Field.None
            });
        return (valid | Field.Cid) == Field.All;
    }

    public override void PartOne()
    {
        var temp = new StringBuilder();
        var count = 0;
        foreach (var line in Input.Append(""))
        {
            if (line == "")
            {
                if (Valid(temp.ToString())) count++;
                temp.Length = 0;
            }
            else
            {
                if (temp.Length > 0) temp.Append(' ');
                temp.Append(line);
            }
        }
        WriteLn(count);
    }

    public bool Valid2(string passport)
    {
        var parts = passport.Split(' ');
        var valid = Field.None;
        foreach (var part in parts)
        {
            var strings = part.Split(':');
            var p = strings[0] switch
            {
                "byr" when Interval.Range(1920, 2003).Contains(strings[1].AsInt()) => Field.Byr,
                "iyr" when Interval.Range(2010, 2021).Contains(strings[1].AsInt()) => Field.Iyr,
                "eyr" when Interval.Range(2020, 2031).Contains(strings[1].AsInt()) => Field.Eyr,
                "hgt" when ValidHeight(strings[1]) => Field.Hgt,
                "hcl" when strings[1].Matches("^#[0-9a-fA-F]{6}$") => Field.Hcl,
                "ecl" when "amb blu brn gry grn hzl oth".Contains(strings[1]) => Field.Ecl,
                "pid" when strings[1].Matches("^\\d{9}$") => Field.Pid,
                "cid" => Field.Cid,
                _ => Field.None
            };
            if (p == Field.None && strings[0] != "cid") return false;
            valid |= p;
        }
        return (valid | Field.Cid) == Field.All;
    }

    public bool ValidHeight(string height)
    {
        if (height.EndsWith("cm")) return (150..194).Interval().Contains(height[..^2].AsInt());
        if (height.EndsWith("in")) return (59..77).Interval().Contains(height[..^2].AsInt());
        return false;
    }

    public override void PartTwo()
    {
        WriteLn(Groups.JoinEach(" ").Count(Valid2));
    }
}