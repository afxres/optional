using System;
using System.Threading.Tasks;

namespace Mikodev.Optional
{
    public static partial class Extensions
    {
        private static Func<Task<T>> MakeTaskFunc<T>(Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return async () =>
            {
                await Task.Yield();
                return func.Invoke();
            };
        }

        private static Func<Task<Unit>> MakeTaskFunc(Func<Task> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return async () =>
            {
                await func.Invoke();
                return new Unit();
            };
        }

        private static Func<T, Task<Unit>> MakeTaskFunc<T>(Func<T, Task> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return async item =>
            {
                await func.Invoke(item);
                return new Unit();
            };
        }
    }
}
