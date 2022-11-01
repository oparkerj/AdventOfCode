using System.Threading;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities.Threads;

public struct Lock
{
    private int _locked;

    public bool Locked
    {
        get => _locked.AsBool();
        set => Interlocked.Exchange(ref _locked, value.AsInt());
    }

    public static implicit operator bool(Lock @lock) => @lock.Locked;

    public void Toggle(bool? state = null) => Locked = state ?? !Locked;
}