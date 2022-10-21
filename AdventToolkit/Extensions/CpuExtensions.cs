using System.Numerics;
using AdventToolkit.Utilities.Computer;

namespace AdventToolkit.Extensions;

public static class CpuExtensions
{
    public static void JumpRelative<T>(this Cpu<T> cpu, long offset)
    {
        cpu.JumpRelative((int) offset);
    }
    
    public static void JumpRelative<T>(this Cpu<T> cpu, BigInteger offset)
    {
        cpu.JumpRelative((int) offset);
    }
}