using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AdventToolkit.Utilities;

public class InputReader : IEnumerable<string>
{
    public readonly string InputFile;

    private DummyDay _day;

    public InputReader(string inputFile)
    {
        InputFile = inputFile;
        _day = new DummyDay {InputName = inputFile};
    }

    public static InputReader ForDay(int year, int day)
    {
        var path = Path.Combine($"AdventOfCode{year}", Path.Combine("Input", $"Day{day}.txt"));
        return new InputReader(path);
    }

    private class DummyDay : PuzzleBase { }

    public string[][] AllGroups => _day.AllGroups;

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<string> GetEnumerator()
    {
        return ((IEnumerable<string>) _day.Input).GetEnumerator();
    }
}