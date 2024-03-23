using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using AdventToolkit.New.Data;

namespace AdventToolkit.New.Reflect;

/// <summary>
/// Type helpers.
/// </summary>
public static class Types
{
    /// <summary>
    /// Primary size of built-in tuples before nesting occurs.
    /// </summary>
    public const int PrimaryTupleSize = 7;

    /// <summary>
    /// Add a human friendly name of a type to the builder.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="builder"></param>
    private static void AddSimpleName(Type type, StringBuilder builder)
    {
        var name = type.Name;
        if (type.IsArray)
        {
            AddSimpleName(type.GetElementType()!, builder);
            builder.Append("[]");
        }
        else if (type.TryGetTupleTypes(out var tupleTypes) && tupleTypes.Length > 1)
        {
            builder.Append('(');
            
            ReadOnlySpan<Type> types = tupleTypes;
            AddSimpleName(types[0], builder);
            foreach (var innerType in types[1..])
            {
                builder.Append(", ");
                AddSimpleName(innerType, builder);
            }
            
            builder.Append(')');
        }
        else if (type.IsGenericType)
        {
            builder.Append(name.AsSpan(0, name.LastIndexOf('`')));
            builder.Append('<');

            ReadOnlySpan<Type> types = type.GetGenericArguments();
            AddSimpleName(types[0], builder);
            foreach (var innerType in types[1..])
            {
                builder.Append(", ");
                AddSimpleName(innerType, builder);
            }
            
            builder.Append('>');
        }
        else
        {
            builder.Append(name);
        }
    }

    /// <summary>
    /// Get a simple version of the type name.
    /// This version displays the type name the way you would type it into a code editor.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string SimpleName(this Type type)
    {
        var builder = new StringBuilder();
        AddSimpleName(type, builder);
        return builder.ToString();
    }

    /// <summary>
    /// Get the <see cref="SimpleName"/> of a type followed by its string
    /// representation in parenthesis.
    /// </summary>
    /// <param name="t"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string SimpleValueString<T>(T t)
    {
        if (t is null) return "null";
        
        var builder = new StringBuilder();
        AddSimpleName(t.GetType(), builder);
        builder.Append('(');
        builder.Append(t);
        builder.Append(')');
        return builder.ToString();
    }
    
    /// <summary>
    /// Get a generic version of a type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>Generic type definition.</returns>
    public static Type Generic(this Type type) => type.IsGenericType ? type.GetGenericTypeDefinition() : type;
    
    /// <summary>
    /// Try to get the type arguments of a particular interface or base class
    /// implemented by the type.
    /// </summary>
    /// <param name="type">Source type to search.</param>
    /// <param name="which">Target type to find. Must be a generic type definition.</param>
    /// <param name="types">Resulting generic types.</param>
    /// <returns>True if the search type was found on the given type.</returns>
    public static bool TryGetTypeArguments(this Type type, Type which, out Type[] types)
    {
        Debug.Assert(which.IsGenericTypeDefinition);
        
        // Search the interfaces
        foreach (var @interface in type.GetInterfaces())
        {
            if (@interface.Generic() != which) continue;
            types = @interface.GetGenericArguments();
            return true;
        }
        
        // Search base classes
        while (true)
        {
            if (type.Generic() == which)
            {
                types = type.GetGenericArguments();
                return true;
            }
            if (type.BaseType is { } @base)
            {
                type = @base;
            }
            else break;
        }

        types = default!;
        return false;
    }

    /// <summary>
    /// Try to get the type arguments of a particular interface or base class
    /// implemented by the object.
    /// </summary>
    /// <param name="value">Source object to search.</param>
    /// <param name="which">Target type to find. Must be a generic type definition.</param>
    /// <param name="types">Resulting generic types.</param>
    /// <returns>True if the search type was found on the given object.</returns>
    public static bool TryGetTypeArgumentsOf(this object value, Type which, out Type[] types)
    {
        return value.GetType().TryGetTypeArguments(which, out types);
    }

    /// <summary>
    /// Get the generic arguments of a particular interface or base class
    /// on the type.
    /// </summary>
    /// <param name="type">Type to search.</param>
    /// <param name="which">Type to find. Must be a generic type definition.</param>
    /// <returns>Generic arguments of found type.</returns>
    /// <exception cref="ArgumentException">The search type was not found on the given type.</exception>
    public static Type[] GetTypeArguments(this Type type, Type which)
    {
        if (type.TryGetTypeArguments(which, out var types)) return types;
        throw new ArgumentException($"Could not get generic arguments. Search = {which}, Given = {type}");
    }

    /// <summary>
    /// Get the generic arguments of a particular interface or base class
    /// on the object.
    /// </summary>
    /// <param name="value">Value to search.</param>
    /// <param name="which">Type to find. Must be a generic type definition.</param>
    /// <returns>Generic arguments of found type.</returns>
    /// <exception cref="ArgumentException">The search type was not found on the given object.</exception>
    public static Type[] GetTypeArgumentsOf(this object value, Type which)
    {
        return value.GetType().GetTypeArguments(which);
    }

    /// <summary>
    /// Get the first type argument from a type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type GetSingleTypeArgument(this Type type)
    {
        Debug.Assert(type.IsGenericType);
        var types = type.GetGenericArguments();
        Debug.Assert(types.Length > 0);
        return types[0];
    }

    /// <summary>
    /// Create an instance of a type using a constructor that matches the
    /// given parameters.
    /// </summary>
    /// <param name="type">Type to construct.</param>
    /// <param name="args">Constructor arguments.</param>
    /// <returns>Reference to new object.</returns>
    public static object New(this Type type, params object?[] args)
    {
        var result = Activator.CreateInstance(type, args);
        Debug.Assert(result is not null);
        return result;
    }

    /// <summary>
    /// Same as <see cref="New"/> but casts to a type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T New<T>(this Type type, params object?[] args)
    {
        return (T) type.New(args);
    }

    /// <summary>
    /// Same as <see cref="New"/> but first constructs a generic type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="generic"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static object NewGeneric(this Type type, Type[] generic, params object?[] args)
    {
        return type.MakeGenericType(generic).New(args);
    }

    /// <summary>
    /// Same as <see cref="NewGeneric"/> but casts to a type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="generic"></param>
    /// <param name="args"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T NewGeneric<T>(this Type type, Type[] generic, params object?[] args)
    {
        return (T) type.NewGeneric(generic, args);
    }

    /// <summary>
    /// Determine if a type is a tuple type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsTupleType(this Type type)
    {
        type = type.Generic();
        
        return type == typeof(ValueTuple<>)
               || type == typeof(ValueTuple<,>)
               || type == typeof(ValueTuple<,,>)
               || type == typeof(ValueTuple<,,,>)
               || type == typeof(ValueTuple<,,,,>)
               || type == typeof(ValueTuple<,,,,,>)
               || type == typeof(ValueTuple<,,,,,,>)
               || type == typeof(ValueTuple<,,,,,,,>);
    }

    /// <summary>
    /// Get the number of elements in a tuple type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>Tuple size.</returns>
    /// <exception cref="ArgumentException">The input type is not a tuple type.</exception>
    public static int GetTupleSize(this Type type)
    {
        Debug.Assert(type.IsTupleType());
        
        var types = type.GetGenericArguments();
        var result = 0;
        while (true)
        {
            result += types.Length;
            if (types.Length <= PrimaryTupleSize) return result;
            types = types[PrimaryTupleSize].GetGenericArguments();
            result--;
        }
    }
    
    /// <summary>
    /// Get the component types of a tuple.
    /// </summary>
    /// <param name="type">Tuple type.</param>
    /// <returns>Component types.</returns>
    public static Type[] GetTupleTypes(this Type type)
    {
        Debug.Assert(type.IsTupleType());
        
        var buffer = new List<Type>();

        ReadOnlySpan<Type> remaining = type.GetGenericArguments();
        while (remaining.Length > PrimaryTupleSize)
        {
            AddAll(remaining[..PrimaryTupleSize]);
            remaining = remaining[PrimaryTupleSize].GetGenericArguments();
        }
        AddAll(remaining);
        
        return buffer.ToArray();
        
        // Add the types to the buffer
        void AddAll(ReadOnlySpan<Type> section)
        {
            buffer.EnsureCapacity(buffer.Count + section.Length);
            foreach (var t in section)
            {
                buffer.Add(t);
            }
        }
    }

    /// <summary>
    /// Try to get the types of a tuple.
    /// </summary>
    /// <param name="type">Input type</param>
    /// <param name="types">Tuple types.</param>
    /// <returns>True if the type is a tuple, false otherwise.</returns>
    public static bool TryGetTupleTypes(this Type type, out Type[] types)
    {
        if (type.IsTupleType())
        {
            types = type.GetTupleTypes();
            return true;
        }
        
        types = default!;
        return false;
    }

    /// <summary>
    /// Create a tuple type from the given component types.
    /// </summary>
    /// <param name="items">Component types.</param>
    /// <returns>Tuple types.</returns>
    public static Type CreateTupleType(ReadOnlySpan<Type> items)
    {
        Debug.Assert(items.Length > 0);
        return TupleTypeInner(items);
        
        Type TupleTypeInner(ReadOnlySpan<Type> types)
        {
            return types.Length switch
            {
                1 => typeof(ValueTuple<>).MakeGenericType([..types]),
                2 => typeof(ValueTuple<,>).MakeGenericType([..types]),
                3 => typeof(ValueTuple<,,>).MakeGenericType([..types]),
                4 => typeof(ValueTuple<,,,>).MakeGenericType([..types]),
                5 => typeof(ValueTuple<,,,,>).MakeGenericType([..types]),
                6 => typeof(ValueTuple<,,,,,>).MakeGenericType([..types]),
                7 => typeof(ValueTuple<,,,,,,>).MakeGenericType([..types]),
                _ => typeof(ValueTuple<,,,,,,,>).MakeGenericType([
                    ..types[..PrimaryTupleSize],
                    TupleTypeInner(types[PrimaryTupleSize..])
                ])
            };
        }
    }
    
    /// <summary>
    /// Create a tuple type from a type and count.
    /// </summary>
    /// <param name="type">Component type.</param>
    /// <param name="count">Tuple size.</param>
    /// <returns></returns>
    public static Type CreateTupleType(Type type, int count)
    {
        Debug.Assert(count > 0);
        return TupleTypeInner(count);
        
        Type TupleTypeInner(int remaining)
        {
            return remaining switch
            {
                1 => typeof(ValueTuple<>).MakeGenericType([type]),
                2 => typeof(ValueTuple<,>).MakeGenericType([type, type]),
                3 => typeof(ValueTuple<,,>).MakeGenericType([type, type, type]),
                4 => typeof(ValueTuple<,,,>).MakeGenericType([type, type, type, type]),
                5 => typeof(ValueTuple<,,,,>).MakeGenericType([type, type, type, type, type]),
                6 => typeof(ValueTuple<,,,,,>).MakeGenericType([type, type, type, type, type, type]),
                7 => typeof(ValueTuple<,,,,,,>).MakeGenericType([type, type, type, type, type, type, type]),
                _ => typeof(ValueTuple<,,,,,,,>).MakeGenericType([
                    type, type, type, type, type, type, type,
                    TupleTypeInner(remaining - PrimaryTupleSize)
                ])
            };
        }
    }
    
    /// <summary>
    /// Create a tuple with the given component types and values.
    /// </summary>
    /// <param name="items">Component types.</param>
    /// <param name="values">Component values.</param>
    /// <returns>Tuple object.</returns>
    public static object CreateTuple(ReadOnlySpan<Type> items, ReadOnlySpan<object?> values)
    {
        Debug.Assert(items.Length == values.Length);
        Debug.Assert(values.Length > 0);
        
        if (values.Length <= PrimaryTupleSize) return CreateTupleType(items).New([..values]);
        return CreateTupleType(items).New([
            ..values[..PrimaryTupleSize],
            CreateTuple(items[PrimaryTupleSize..], values[PrimaryTupleSize..])
        ]);
    }
    
    /// <summary>
    /// Create a tuple where each element is the same type.
    /// </summary>
    /// <param name="type">Tuple element type.</param>
    /// <param name="values">Tuple element values.</param>
    /// <returns></returns>
    public static object CreateTuple(Type type, ReadOnlySpan<object?> values)
    {
        Debug.Assert(values.Length > 0);
        
        if (values.Length <= PrimaryTupleSize) return CreateTupleType(type, values.Length).New([..values]);
        return CreateTupleType(type, values.Length).New([
            ..values[..PrimaryTupleSize],
            CreateTuple(type, values[PrimaryTupleSize..])
        ]);
    }

    /// <summary>
    /// Create a tuple given an existing tuple type.
    /// </summary>
    /// <param name="tuple">Tuple type.</param>
    /// <param name="values">Tuple elements.</param>
    /// <returns></returns>
    public static object CreateTupleFrom(Type tuple, ReadOnlySpan<object?> values)
    {
        Debug.Assert(tuple.IsTupleType());
        Debug.Assert(tuple.GetTupleSize() == values.Length);

        if (values.Length <= PrimaryTupleSize) return tuple.New([..values]);
        return tuple.New([
            ..values[..PrimaryTupleSize],
            CreateTupleFrom(tuple.GetGenericArguments()[PrimaryTupleSize], values[PrimaryTupleSize..])
        ]);
    }

    /// <summary>
    /// Get a tuple type representing a smaller portion of a tuple.
    /// </summary>
    /// <param name="type">Original tuple.</param>
    /// <param name="start">Start element offset.</param>
    /// <param name="count">Number of elements to take.</param>
    /// <returns></returns>
    public static Type SliceTupleType(Type type, int start, int count)
    {
        Debug.Assert(type.IsTupleType());

        return CreateTupleType(type.GetTupleTypes().AsSpan(start, count));
    }
    
    /// <summary>
    /// Get a sliced portion of a tuple's element types.
    /// </summary>
    /// <param name="type">Original tuple.</param>
    /// <param name="start">Start element offset.</param>
    /// <param name="count">Number of elements to take.</param>
    /// <returns></returns>
    public static Type[] SliceTuple(Type type, int start, int count)
    {
        Debug.Assert(type.IsTupleType());
        
        return type.GetTupleTypes()[start..(start + count)];
    }

    /// <summary>
    /// Slice a tuple instance.
    /// </summary>
    /// <param name="tuple">Original tuple.</param>
    /// <param name="types">Types of the resulting tuple.</param>
    /// <param name="start">Offset of first element.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static object SliceTuple<T>(T tuple, Type[] types, int start)
        where T : ITuple
    {
        using Arr<object?> buffer = new(types.Length);

        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = tuple[i + start];
        }

        return CreateTuple(types, buffer);
    }
}