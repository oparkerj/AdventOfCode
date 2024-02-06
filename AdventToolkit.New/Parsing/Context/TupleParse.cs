using System.Diagnostics;
using AdventToolkit.New.Algorithms;
using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Context;

public class TupleParse : ITypeDescriptor
{
    private static IndexOutOfRangeException NoElement => new("Reached end of enumerator before end of tuple.");
    
    public bool Match(Type type) => type.IsTupleType();

    public bool PassiveSelect => true;

    public bool TrySelect(Type type, out Type inner, out IParser selector)
    {
        if (ArrayParse.IsSingleType(type.GetTupleTypes()))
        {
            inner = type.GetSingleTypeArgument();
            selector = GetEnumerator(type);
            return true;
        }

        inner = default!;
        selector = default!;
        return false;
    }

    private IParser GetEnumerator(Type type)
    {
        Debug.Assert(type.IsTupleType());
        return GetEnumerator(type.GetGenericArguments());
    }

    private IParser GetEnumerator(ReadOnlySpan<Type> types)
    {
        var first = types[0];
        return types.Length switch
        {
            1 => typeof(Enumerator1<>).NewParserGeneric([first]),
            2 => typeof(Enumerator2<>).NewParserGeneric([first]),
            3 => typeof(Enumerator3<>).NewParserGeneric([first]),
            4 => typeof(Enumerator4<>).NewParserGeneric([first]),
            5 => typeof(Enumerator5<>).NewParserGeneric([first]),
            6 => typeof(Enumerator6<>).NewParserGeneric([first]),
            7 => typeof(Enumerator7<>).NewParserGeneric([first]),
            _ => typeof(EnumeratorRest<,>).NewParserGeneric([first, types[Types.PrimaryTupleSize]],
                GetEnumerator(types[Types.PrimaryTupleSize].GetGenericArguments()))
        };
    }

    public class Enumerator1<T> : IParser<ValueTuple<T>, IEnumerable<T>>
    {
        public IEnumerable<T> Parse(ValueTuple<T> input)
        {
            yield return input.Item1;
        }
    }
    
    public class Enumerator2<T> : IParser<(T, T), IEnumerable<T>>
    {
        public IEnumerable<T> Parse((T, T) input)
        {
            yield return input.Item1;
            yield return input.Item2;
        }
    }
    
    public class Enumerator3<T> : IParser<(T, T, T), IEnumerable<T>>
    {
        public IEnumerable<T> Parse((T, T, T) input)
        {
            yield return input.Item1;
            yield return input.Item2;
            yield return input.Item3;
        }
    }
    
    public class Enumerator4<T> : IParser<(T, T, T, T), IEnumerable<T>>
    {
        public IEnumerable<T> Parse((T, T, T, T) input)
        {
            yield return input.Item1;
            yield return input.Item2;
            yield return input.Item3;
            yield return input.Item4;
        }
    }
    
    public class Enumerator5<T> : IParser<(T, T, T, T, T), IEnumerable<T>>
    {
        public IEnumerable<T> Parse((T, T, T, T, T) input)
        {
            yield return input.Item1;
            yield return input.Item2;
            yield return input.Item3;
            yield return input.Item4;
            yield return input.Item5;
        }
    }
    
    public class Enumerator6<T> : IParser<(T, T, T, T, T, T), IEnumerable<T>>
    {
        public IEnumerable<T> Parse((T, T, T, T, T, T) input)
        {
            yield return input.Item1;
            yield return input.Item2;
            yield return input.Item3;
            yield return input.Item4;
            yield return input.Item5;
            yield return input.Item6;
        }
    }
    
    public class Enumerator7<T> : IParser<(T, T, T, T, T, T, T), IEnumerable<T>>
    {
        public IEnumerable<T> Parse((T, T, T, T, T, T, T) input)
        {
            yield return input.Item1;
            yield return input.Item2;
            yield return input.Item3;
            yield return input.Item4;
            yield return input.Item5;
            yield return input.Item6;
            yield return input.Item7;
        }
    }
    
    public class EnumeratorRest<T, TRest>(IParser<TRest, IEnumerable<T>> restEnumerator)
        : IParser<ValueTuple<T, T, T, T, T, T, T, TRest>, IEnumerable<T>>
        where TRest : struct
    {
        public IEnumerable<T> Parse(ValueTuple<T, T, T, T, T, T, T, TRest> input)
        {
            yield return input.Item1;
            yield return input.Item2;
            yield return input.Item3;
            yield return input.Item4;
            yield return input.Item5;
            yield return input.Item6;
            yield return input.Item7;
            
            foreach (var t in restEnumerator.Parse(input.Rest))
            {
                yield return t;
            }
        }

        public IEnumerable<IParser> GetChildren()
        {
            yield return restEnumerator;
        }
    }
}