using System;
using System.Collections.Generic;
using Microsoft.Z3;

namespace Z3Helper;

public static class ZOptimize
{
    public static void LogResult(this Optimize o, params Optimize.Handle[] handles)
    {
        Console.WriteLine(o.Check());
        foreach (var handle in handles)
        {
            Console.WriteLine(handle.Value);
        }
    }
    
    public static Optimize.Handle Maximize(this Optimize o, Expr expr)
    {
        return o.MkMaximize(expr);
    }
    
    public static Optimize.Handle Minimize(this Optimize o, Expr expr)
    {
        return o.MkMinimize(expr);
    }

    public static Optimize AddAll(this Optimize o, IEnumerable<ZExpr> exprs, Func<ZExpr, ZbExpr> func)
    {
        foreach (var expr in exprs)
        {
            o.Add(func(expr));
        }
        return o;
    }
}