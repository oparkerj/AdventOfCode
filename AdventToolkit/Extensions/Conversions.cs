namespace AdventToolkit.Extensions
{
    public static class Conversions
    {
        public static int AsInt(this string s) => int.Parse(s);
        
        public static int AsInt(this char c) => c - '0';
        
        public static int AsInt(this bool b) => b ? 1 : 0;
        
        public static bool AsBool(this int i) => i != 0;
        
        public static bool AsBool(this long i) => i != 0;
    }
}