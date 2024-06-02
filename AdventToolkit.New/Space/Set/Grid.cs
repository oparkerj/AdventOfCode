using System.Numerics;
using AdventToolkit.New.Parsing;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;
using AdventToolkit.New.Space.Bound;
using AdventToolkit.New.Space.Dimension;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Set;

/// <summary>
/// Two-dimensional sparse space.
/// </summary>
/// <typeparam name="TNum"></typeparam>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TDim"></typeparam>
public class Grid<TNum, T, TDim> : SparseSpace<Pos<TNum>, T, Rect<TNum>>, IGrid<TNum, T, TDim>, IAdapterLookup
    where TNum : INumber<TNum>
    where TDim : IDimension<Pos<TNum>>, new()
{
    public TDim Dimension { get; set; } = new();

    public bool TryLookup(Type from, Type to, IParseContext context, out IParser parser)
    {
        if (!to.TryGetTypeArguments(typeof(Grid<,,>), out var baseType, out var toTypes))
        {
            parser = default!;
            return false;
        }
        
        if (ParseUtil.TryGetInnerType(from, context, out var inner, out var selector)
            && ParseUtil.TryGetInnerType(inner, context, out var inner1, out var selector1)
            && ParseAdapt.TryAdapt(inner1, toTypes[1], context, out var adapt))
        {
            var input = ParseAdapt.MaybeInnerJoin(selector1, adapt, context, 1);
            input = ParseAdapt.MaybeInnerJoin(selector, input, context, 1);
            
            var adapter = baseType.MakeNestedType(typeof(Adapter<>).Name, to).NewParser();
            parser = ParseAdapt.MaybeJoin(input, adapter);
            return true;
        }

        parser = default!;
        return false;
    }

    /// <summary>
    /// Converts a double enumerable to a grid.
    /// The first element will be placed at <see cref="Pos{T}.Zero"/>.
    /// Each inner element is placed to the <see cref="Pos{T}.Right"/>.
    /// The next inner sequence begins <see cref="Pos{T}.Down"/> from the previous.
    /// </summary>
    /// <typeparam name="TGrid"></typeparam>
    public class Adapter<TGrid> : IParser<IEnumerable<IEnumerable<T>>, TGrid>
        where TGrid : Grid<TNum, T, TDim>, new()
    {
        public TGrid Parse(IEnumerable<IEnumerable<T>> input)
        {
            var result = new TGrid();
            var y = TNum.Zero;
            foreach (var row in input)
            {
                var x = TNum.Zero;
                foreach (var t in row)
                {
                    result[new Pos<TNum>(x, y)] = t;
                    x++;
                }
                y--;
            }
            return result;
        }
    }
}

/// <inheritdoc cref="Grid{TNum,T,TDim}"/>
public class Grid<TNum, T> : Grid<TNum, T, PosAdjacent<Pos<TNum>>>
    where TNum : INumber<TNum>;

/// <inheritdoc cref="Grid{TNum,T,TDim}"/>
public class Grid<T> : Grid<int, T>;