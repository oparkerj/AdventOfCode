using AdventToolkit;
using AdventToolkit.Collections.Tree;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day7 : Puzzle<long>
{
    public FileTree Tree;

    public void ReadInput()
    {
        Tree = new FileTree();

        foreach (var s in Input)
        {
            if (s.StartsWith("$"))
            {
                var cmd = s.After("$ ");
                if (cmd.StartsWith("cd"))
                {
                    var path = cmd.After("cd ");
                    Tree.ChangeDirCreate(path);
                }
            }
            else if (s.StartsWith("dir")) ;
            else
            {
                var size = s.TakeInt();
                var fileName = s.After(" ");
                Tree.CreateFile(fileName, size);
            }
        }
    }

    public override long PartOne()
    {
        ReadInput();
        return Tree.AllDirectories.Sizes().Where(size => size <= 100000).Sum();
    }

    public override long PartTwo()
    {
        const long available = 70000000;
        const long needed = 30000000;

        ReadInput();
        var unused = available - Tree.Size(Tree.Root);
        var remove = needed - unused;
        return Tree.AllDirectories.Sizes().Order().First(size => size >= remove);
    }
}