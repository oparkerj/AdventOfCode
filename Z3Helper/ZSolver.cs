using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;

namespace Z3Helper;

public static class ZSolver
{
    public static T Get<T>(this Solver solver, Expr expr)
        where T : IParsable<T>
    {
        return solver.Model.Get<T>(expr);
    }

    public static IEnumerable<T> GetAll<T>(this Solver solver, params Expr[] exprs)
        where T : IParsable<T>
    {
        return exprs.Select(solver.Get<T>);
    }
}