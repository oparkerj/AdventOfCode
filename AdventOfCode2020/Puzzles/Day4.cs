using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles
{
    public class Day4 : Puzzle
    {
        public Day4()
        {
            Part = 2;
        }

        public bool Valid(string passport)
        {
            var parts = passport.Split(' ');
            byte valid = 0;
            foreach (var part in parts)
            {
                var strings = part.Split(':');
                valid = (byte) (valid | strings[0] switch
                {
                    "byr" => 1,
                    "iyr" => 1 << 1,
                    "eyr" => 1 << 2,
                    "hgt" => 1 << 3,
                    "hcl" => 1 << 4,
                    "ecl" => 1 << 5,
                    "pid" => 1 << 6,
                    "cid" => 1 << 7,
                    _ => 0
                });
            }
            return (valid | (1 << 7)) == 255;
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
            byte valid = 0;
            foreach (var part in parts)
            {
                var strings = part.Split(':');
                byte p = strings[0] switch
                {
                    "byr" when Range(strings[1], 1920, 2003) => 1,
                    "iyr" when Range(strings[1], 2010, 2021) => 1 << 1,
                    "eyr" when Range(strings[1], 2020, 2031) => 1 << 2,
                    "hgt" when ValidHeight(strings[1]) => 1 << 3,
                    "hcl" when MatchAll(strings[1], "#[0-9a-fA-F]{6}") => 1 << 4,
                    "ecl" when "amb blu brn gry grn hzl oth".Contains(strings[1]) => 1 << 5,
                    "pid" when MatchAll(strings[1], "\\d{9}") => 1 << 6,
                    "cid" => 1 << 7,
                    _ => 0
                };
                if (p == 0 && strings[0] != "cid") return false;
                valid |= p;
            }
            return (valid | (1 << 7)) == 255;
        }

        public bool Valid3(string passport)
        {
            var parts = passport.Split(' ');
            var values = new Dictionary<string, string>();
            foreach (var part in parts)
            {
                var p = part.Split(':');
                values[p[0]] = p[1];
            }
            bool Has(string key, Func<string, bool> valid)
            {
                return values.TryGetValue(key, out var value) && valid(value);
            }

            return Has("byr", s => Range(s, 1920, 2003)) &&
                   Has("iyr", s => Range(s, 2010, 2021)) &&
                   Has("eyr", s => Range(s, 2020, 2031)) &&
                   Has("hgt", ValidHeight) &&
                   Has("hcl", s => s[0] == '#' && int.TryParse(s[1..], NumberStyles.HexNumber, null, out _)) &&
                   Has("ecl", s => "amb blu brn gry grn hzl oth".Contains(s)) &&
                   Has("pid", s => s.Length == 9 && int.TryParse(s, out _));
        }

        public bool ValidHeight(string height)
        {
            if (height.EndsWith("cm")) return Range(height[..^2], 150, 194);
            if (height.EndsWith("in")) return Range(height[..^2], 59, 77);
            return false;
        }

        public override void PartTwo()
        {
            WriteLn(Groups.Join(" ").Count(Valid2));
        }
    }
}