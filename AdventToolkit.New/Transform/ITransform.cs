using System.Collections;

namespace AdventToolkit.New.Transform;

public interface ITransform<in TIn, out TOut>
{
    TOut Apply(TIn input);
}

public interface ITransformResult<out T>
{
    T Result { get; }
}

public interface ITransformSource<in TIn, out TOut> : ITransform<TIn, TOut>, ITransformResult<TOut> { }

public interface ITransformSequence<in TIn, out TOut> : ITransform<TIn, IEnumerable<TOut>>, ITransformResult<IEnumerable<TOut>>, IEnumerable<TOut>
{
    IEnumerator<TOut> IEnumerable<TOut>.GetEnumerator() => Result.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}