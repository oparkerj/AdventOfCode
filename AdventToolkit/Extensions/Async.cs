using System.Threading.Tasks;

namespace AdventToolkit.Extensions
{
    public static class Async
    {
        public delegate bool PartialTask<T>(out T result);

        public static Task<T> RunUntil<T>(PartialTask<T> task)
        {
            return Task.Run(() =>
            {
                Run:
                if (!task(out var result)) goto Run;
                return result;
            });
        }
    }
}