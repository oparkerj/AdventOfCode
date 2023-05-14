using System;
using AdventToolkit.Utilities.Computer.Core;

namespace AdventToolkit.Utilities.Computer.Pipelines;

public class PipelineFilter<TArch> : IPipeline<TArch>
{
    public readonly IPipeline<TArch> Pipeline;

    public Func<Cpu<TArch>, bool> Filter { get; set; }

    public PipelineFilter(IPipeline<TArch> pipeline, Func<Cpu<TArch>, bool> filter = default)
    {
        Pipeline = pipeline;
        Filter = filter;
    }

    public bool Tick(Cpu<TArch> cpu)
    {
        return !Filter(cpu) || Pipeline.Tick(cpu);
    }

    public void JumpRelative(Cpu<TArch> cpu, int offsetToNext)
    {
        Pipeline.JumpRelative(cpu, offsetToNext);
    }

    public void JumpTo(Cpu<TArch> cpu, int next)
    {
        Pipeline.JumpTo(cpu, next);
    }
}