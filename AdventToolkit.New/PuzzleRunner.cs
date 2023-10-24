using AdventToolkit.New.Util;
using TextCopy;

namespace AdventToolkit.New;

/// <summary>
/// Provides basic capability to execute puzzles.
/// </summary>
public static class PuzzleRunner
{
    /// <summary>
    /// Run a puzzle instance.
    /// </summary>
    /// <param name="puzzle"></param>
    public static void Run(PuzzleBase puzzle) => puzzle.Run();

    /// <summary>
    /// Run a puzzle, using its default constructor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void Run<T>()
        where T : PuzzleBase, new() => Run(new T());
    
    /// <summary>
    /// Run a puzzle instance, capturing console output.
    /// </summary>
    /// <param name="puzzle"></param>
    /// <returns>Captured output.</returns>
    public static string RunCapture(PuzzleBase puzzle)
    {
        using var capture = new ConsoleCapture();
        using (capture.Start())
        {
            Run(puzzle);
        }
        return capture.Text;
    }

    /// <summary>
    /// Executes <see cref="RunCapture"/>, then copy the text
    /// to the clipboard and call the puzzles <see cref="PuzzleBase.WriteLn(string)"/>
    /// method.
    /// </summary>
    /// <param name="puzzle"></param>
    public static void RunClip(PuzzleBase puzzle)
    {
        var result = RunCapture(puzzle);
        ClipboardService.SetText(result);
        puzzle.WriteLn(result);
    }

    /// <summary>
    /// Execute <see cref="RunClip(PuzzleBase)"/> using its default constructor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void RunClip<T>()
        where T : PuzzleBase, new() => RunClip(new T());
}