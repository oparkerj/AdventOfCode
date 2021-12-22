using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Common;

namespace AdventToolkit.Collections.Space;

public class Cloud : IEnumerable<Pos3D>
{
    public readonly HashSet<Pos3D> Points = new();

    public Pos3D Offset { get; private set; }

    public Cloud() { }

    public Cloud(IEnumerable<Pos3D> points) => AddAll(points);

    public int Count => Points.Count;

    public bool this[Pos3D pos]
    {
        get => Contains(pos);
        set
        {
            if (value) Add(pos);
            else Remove(pos);
        }
    }

    public void Add(Pos3D pos) => Points.Add(pos);

    public void AddAll(IEnumerable<Pos3D> points)
    {
        foreach (var pos in points) Points.Add(pos);
    }

    public void Remove(Pos3D pos) => Points.Remove(pos);

    public void RemoveAll(IEnumerable<Pos3D> points)
    {
        foreach (var pos in points) Points.Remove(pos);
    }

    public bool Contains(Pos3D pos) => Points.Contains(pos);

    private void Transform(Func<Pos3D, Pos3D> transformer)
    {
        var next = Points.Select(transformer).ToHashSet();
        Points.Clear();
        foreach (var pos in next) Points.Add(pos);
        Offset = transformer(Offset);
    }

    public void Shift(Pos3D offset)
    {
        Transform(p => p + offset);
    }

    public int CountOverlap(Cloud other) => Points.Count(other.Points.Contains);

    public bool TryAllOrientations(Func<Cloud, bool> test)
    {
        for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                if (test(this)) return true;
                Transform(p => p.XClockwise());

            }
            Transform(p => p.YClockwise());
        }
        Transform(p => p.ZClockwise());
        for (var i = 0; i < 4; i++)
        {
            if (test(this)) return true;
            Transform(p => p.XClockwise());
        }
        Transform(p => p.ZClockwise().ZClockwise());
        for (var i = 0; i < 4; i++)
        {
            if (test(this)) return true;
            Transform(p => p.XClockwise());
        }
        Transform(p => p.ZClockwise());
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<Pos3D> GetEnumerator() => Points.GetEnumerator();
}