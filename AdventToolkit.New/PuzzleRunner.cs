using AdventToolkit.New.Util;
using TextCopy;

namespace AdventToolkit.New;

public static class PuzzleRunner
{
    public static void Run(PuzzleBase puzzle) => puzzle.Run();

    public static void Run<T>()
        where T : PuzzleBase, new() => Run(new T());

    public static void RunClip(PuzzleBase puzzle)
    {
        using var capture = new ConsoleCapture();
        using (capture.Start())
        {
            Run(puzzle);
        }
        var result = capture.Text;
        ClipboardService.SetText(result);
        puzzle.WriteLn(result);
    }

    public static void RunClip<T>()
        where T : PuzzleBase, new() => RunClip(new T());
}