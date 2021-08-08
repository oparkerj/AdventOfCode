using System;

namespace AdventToolkit.Utilities
{
    public readonly struct Pos3D : IAdd<Pos3D>
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public Pos3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Pos3D Origin => new();
        public static Pos3D Up => new(0, 1, 0);
        public static Pos3D Right = new(1, 0, 0);
        public static Pos3D Down = new(0, -1, 0);
        public static Pos3D Left = new(-1, 0, 0);
        public static Pos3D Forward = new(0, 0, 1);
        public static Pos3D Backward = new(0, 0, -1);

        public static implicit operator Pos3D((int x, int y, int z) p) => new(p.x, p.y, p.z);

        public override bool Equals(object obj)
        {
            return obj is Pos3D (var x, var y, var z) && x == X && y == Y && z == Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode());
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public static Pos3D operator -(in Pos3D p) => new(-p.X, -p.Y, -p.Z);

        public static Pos3D operator +(in Pos3D a, in Pos3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public Pos3D Add(Pos3D other) => this + other;

        public static Pos3D operator -(in Pos3D a, in Pos3D b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Pos3D operator *(in Pos3D p, int s) => new(p.X * s, p.Y * s, p.Z * s);

        public static Pos3D operator *(int s, in Pos3D p) => p * s;

        public int MDist(in Pos3D p)
        {
            return Math.Abs(X - p.X) + Math.Abs(Y - p.Y) + Math.Abs(Z - p.Z);
        }
    }
}