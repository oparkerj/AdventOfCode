using System.Numerics;
using AdventToolkit2.Parsing;
using AdventToolkit2.Parsing.Interface;
using AdventToolkit2.Reflect;
using AdventToolkit2.Util;

namespace AdventToolkit2.Space;

public class Pos3
{
    public class Descriptor : ITypeDescriptor
    {
        public static bool Match(Type type) => type.Generic() == typeof(Pos3<>);
        
        public static bool TryConstruct(Type type, IParseContext context, TypeSpan types, out IParser constructor)
        {
            var numType = type.GetSingleTypeArgument();
            if (types.TryAdaptTuple(Types.CreateTupleType(numType, 3), context, out var convert))
            {
                var posConstructor = typeof(Constructor<>).NewParserGeneric([numType]);
                constructor = ParseAdapt.MaybeJoin(convert, posConstructor);
                return true;
            }
    
            constructor = default!;
            return false;
        }

        public static bool TryUnpack(Type type, IParseContext context, int amount, out IParser unpack)
        {
            if (amount != 3)
            {
                unpack = default!;
                return false;
            }

            var numType = type.GetSingleTypeArgument();
            unpack = typeof(Unpack<>).NewParserGeneric([numType]);
            return true;
        }

        public static bool PassiveSelect => false;
    
        public static bool TrySelect(Type type, out Type inner, out IParser selector)
        {
            return Impl.Default(out inner, out selector);
        }

        public static bool TryCollect(Type type, Type inner, IParseContext context, out IParser collector)
        {
            return Impl.Default(out collector);
        }

        public static bool TryGetCollectType(Type type, IParseContext context, out Type inner)
        {
            return Impl.Default(out inner);
        }

        /// <summary>
        /// Construct a Pos from a pair.
        /// </summary>
        public class Constructor<T> : IParser<(T, T, T), Pos3<T>>
            where T : INumber<T>
        {
            public Pos3<T> Parse((T, T, T) input) => new(input.Item1, input.Item2, input.Item3);
        }
    
        /// <summary>
        /// Unpack a Pos into a pair.
        /// </summary>
        public class Unpack<T> : IParser<Pos3<T>, (T, T, T)>
            where T : INumber<T>
        {
            public (T, T, T) Parse(Pos3<T> input) => (input.X, input.Y, input.Z);
        }
    }
}