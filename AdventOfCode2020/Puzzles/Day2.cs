using System.Linq;
using AdventToolkit;

namespace AdventOfCode2020.Puzzles
{
    public class Day2 : Puzzle
    {
        public Day2()
        {
            Part = 2;
        }

        public bool IsValid(string entry)
        {
            var parts = entry.Split(' ');
            var bounds = parts[0].Split('-');
            var lower = int.Parse(bounds[0]);
            var upper = int.Parse(bounds[1]);
            var c = parts[1][0]; // get char
            var count = parts[2].Count(ch => ch == c);
            return count >= lower && count <= upper;
        }
        
        public override void PartOne()
        {
            WriteLn(Input.Count(IsValid));
        }
        
        public bool IsValid2(string entry)
        {
            var parts = entry.Split(' ');
            var bounds = parts[0].Split('-');
            var lower = int.Parse(bounds[0]) - 1;
            var upper = int.Parse(bounds[1]) - 1;
            var c = parts[1][0]; // get char
            return parts[2][lower] == c ^ parts[2][upper] == c;
        }

        public override void PartTwo()
        {
            WriteLn(Input.Count(IsValid2));
        }
    }
}