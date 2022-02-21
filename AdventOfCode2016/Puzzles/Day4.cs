using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2016.Puzzles;

public class Day4 : Puzzle
{
    public Day4()
    {
        Part = 2;
    }

    public IEnumerable<Room> ReadRooms()
    {
        return Input.Extract<Room>(@"([-\w]+)-(\d+)\[(\w+)\]");
    }

    public override void PartOne()
    {
        var count = ReadRooms().Where(room => room.IsValid).Select(room => room.Sector).Sum();
        WriteLn(count);
    }

    public override void PartTwo()
    {
        var (_, sector, _) = ReadRooms().Where(room => room.IsValid).First(room => room.DecryptedName == "northpole-object-storage");
        WriteLn(sector);
    }

    public record Room(string Name, int Sector, string Checksum)
    {
        public bool IsValid
        {
            get
            {
                var freq = Name.Frequencies().ToDefaultDictFirst();
                freq.Remove('-');
                var correct = freq.OrderByDescending(pair => pair.Value)
                    .ThenBy(pair => pair.Key)
                    .Take(Checksum.Length)
                    .Keys()
                    .Str();
                return Checksum == correct;
            }
        }

        public string DecryptedName => Name.Caesar(Sector);
    }
}