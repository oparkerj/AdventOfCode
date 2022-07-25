using AdventToolkit.Collections;
using AdventToolkit.Common;

namespace AdventToolkit.Extensions;

public static class RectExtensions
{
    public static Rect ToRect(this (Pos A, Pos B) corners)
    {
        return new Rect(corners.A, corners.B);
    }

    public static Rect ToRectCorners(this Pos4D corners)
    {
        var (a, b, c, d) = corners;
        return new Rect(Interval.RangeInclusive(a, c), Interval.RangeInclusive(b, d));
    }
}