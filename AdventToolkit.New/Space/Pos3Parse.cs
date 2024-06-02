using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;
using AdventToolkit.New.Util;

namespace AdventToolkit.New.Space;

public class Pos3
{
    public class Descriptor : ITypeDescriptor
    {
        public static bool Match(Type type) => type.Generic() == typeof(Pos3<>);

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

        public static bool TryConstruct(Type type, IParseContext context, TypeSpan types, out IParser constructor)
        {
            return Impl.Default(out constructor);
        }

        public static bool TryUnpack(Type type, IParseContext context, int amount, out IParser unpack)
        {
            return Impl.Default(out unpack);
        }

        // TODO make constructor and unpack
    }
}