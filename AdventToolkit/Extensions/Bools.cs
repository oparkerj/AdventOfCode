namespace AdventToolkit.Extensions;

public static class Bools
{
    public static bool Or(bool a, bool b) => a || b;

    public static bool And(bool a, bool b) => a && b;

    public static bool Xor(bool a, bool b) => a ^ b;

    public static bool Not(bool a) => !a;
}