using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections.Tree;

public class FileTree : Tree<string, FileVertex, Edge<string>>
{
    public const char Separator = '/';
    public const string Current = ".";
    public const string Parent = "..";
    
    private List<string> _path = new();

    public FileTree()
    {
        base.AddVertex(new FileVertex(CurrentDir) {IsDirectory = true});
    }

    private string Combine(string first, string second)
    {
        if (first.EndsWith(Separator)) first = first[..^1];
        if (second.StartsWith(Separator)) second = second[1..];
        return first + Separator + second;
    }

    private string MakePath(IEnumerable<string> parts) => Separator + string.Join(Separator, parts);

    public string CurrentDir => MakePath(_path);

    public IEnumerable<FileVertex> AllFiles => this.Where(vertex => !vertex.IsDirectory);

    public IEnumerable<FileVertex> AllDirectories => this.Where(vertex => vertex.IsDirectory);

    public IEnumerable<FileVertex> Entries => GetEntry(CurrentDir).Neighbors.Cast<FileVertex>();

    public IEnumerable<FileVertex> Files => Entries.Where(vertex => !vertex.IsDirectory);

    public IEnumerable<FileVertex> Directories => Entries.Where(vertex => vertex.IsDirectory);

    public long Size(FileVertex file) => Bfs(file).Select(vertex => vertex.Size).Sum();

    public FileVertex GetEntry(string path)
    {
        return TryGet(path, out var file) ? file : null;
    }

    public FileVertex GetDir(string dir)
    {
        return Entries.SingleOrDefault(vertex => vertex.IsDirectory && vertex.Name == dir);
    }
    
    public FileVertex GetFile(string name)
    {
        return Entries.SingleOrDefault(vertex => !vertex.IsDirectory && vertex.Name == name);
    }

    public string ResolvePath(string path)
    {
        var parts = path.Split(Separator).ToList();
        for (var i = 0; i < parts.Count; i++)
        {
            var part = parts[i];
            if (part == Current) parts.RemoveConcurrent(ref i);
            else if (part == Parent && i > 0 && parts[i - 1] != Parent)
            {
                parts.RemoveConcurrent(ref i);
                // This could be made into an error if you are not allowed
                // to go up past the root level
                if (parts[i] != "") parts.RemoveConcurrent(ref i);
            }
        }
        return string.Join(Separator, parts);
    }

    public FileVertex ChangeDir(string path)
    {
        if (path == Separator.ToString())
        {
            _path.Clear();
            return GetEntry(CurrentDir);
        }
        if (path == Parent) return Exit();
        
        if (!path.StartsWith(Separator))
        {
            path = ResolvePath(Combine(CurrentDir, path));
        }
        else
        {
            path = ResolvePath(path);
        }

        var dir = GetEntry(path);
        if (dir != null)
        {
            _path.Clear();
            _path.AddRange(path[1..].Split(Separator));
        }
        return dir;
    }
    
    public FileVertex ChangeDirCreate(string path)
    {
        if (path == Separator.ToString())
        {
            _path.Clear();
            return GetEntry(CurrentDir);
        }
        if (path == Parent) return Exit();
        
        if (!path.StartsWith(Separator))
        {
            path = ResolvePath(Combine(CurrentDir, path));
        }
        else
        {
            path = ResolvePath(path);
        }

        var dir = GetEntry(path);
        if (dir == null)
        {
            var current = CurrentDir;
            if (path.StartsWith(current))
            {
                path = path.Substring(current.Length);
                if (!path.StartsWith(Separator)) path = Separator + path;
            }
            else
            {
                _path.Clear();
            }
            foreach (var part in path[1..].Split(Separator))
            {
                dir = CreateAndEnter(part);
            }
        }
        return dir;
    }

    public FileVertex CreateAndEnter(string dir)
    {
        var parent = GetEntry(CurrentDir);
        var subDir = GetDir(dir);

        if (subDir == null)
        {
            subDir = new FileVertex(Combine(CurrentDir, dir)) {IsDirectory = true};
            AddVertex(subDir);
            parent.LinkTo(subDir);
        }
        
        _path.Add(dir);
        return subDir;
    }

    public FileVertex EnterDir(string dir)
    {
        var subDir = GetDir(dir);
        if (subDir != null) _path.Add(dir);
        return subDir;
    }

    public FileVertex Exit()
    {
        if (_path.Count > 0) _path.RemoveAt(_path.Count - 1);
        return GetEntry(CurrentDir);
    }

    public FileVertex CreateFile(string name, long size)
    {
        var parent = GetEntry(CurrentDir);
        var file = new FileVertex(Combine(parent.Path, name), size);
        AddVertex(file);
        parent.LinkTo(file);
        return file;
    }
}

public class FileVertex : QuantityVertexBase<string, Edge<string>, long>
{
    public bool IsDirectory { get; set; }
    
    public FileVertex(string value, long quantity = 0) : base(value, quantity) { }

    public string Path => Value;

    public string Name => Value.Substring(Value.LastIndexOf(FileTree.Separator) + 1);

    public long Size
    {
        get => Quantity;
        set => Quantity = value;
    }
}

public static class FileTreeExtensions
{
    public static long TotalSize(this FileVertex vertex)
    {
        return vertex.Descendants.Cast<FileVertex>().Select(file => file.Size).Sum() + vertex.Size;
    }
    
    public static IEnumerable<long> Sizes(this IEnumerable<FileVertex> vertices)
    {
        return vertices.Select(TotalSize);
    }
}