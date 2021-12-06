using AdventToolkit.Collections;
using AdventToolkit.Common;

namespace AdventToolkit.Extensions
{
    public static class RectExtensions
    {
        public static Rect ToRect(this (Pos A, Pos B) corners)
        {
            return new Rect(corners.A, corners.B);
        }
    }
}