using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Common;

namespace AdventToolkit.Utilities;

public class PathRoom
{
    public readonly Pos Pos;
    public readonly string Path;

    public PathRoom(Pos pos, string path = "")
    {
        Path = path;
        Pos = pos;
    }

    public PathRoom Relative(char dir) => new(Pos + Pos.RelativeDirection(dir), Path + dir);

    public IEnumerable<PathRoom> Neighbors()
    {
        return Pos.RelativeDirections.Select(Relative);
    }

    protected bool Equals(PathRoom other)
    {
        return Pos.Equals(other.Pos) && Path == other.Path;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((PathRoom) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Pos, Path);
    }
}