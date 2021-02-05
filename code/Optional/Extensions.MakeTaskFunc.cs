using System;
using System.Threading.Tasks;

namespace Mikodev.Optional
{
    public static partial class Extensions
    {
        private static Func<Task<T>> MakeTaskFunc<T>(Task<T> task)
        {
            if (task is null)
                throw new ArgumentNullException(nameof(task));
            return () => task;
        }

        private static Func<Task<T>> MakeTaskFunc<T>(Func<T> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            return async () =>
            {
                await Task.Yield();
                return data.Invoke();
            };
        }

        private static Func<Task<Unit>> MakeTaskFunc(Task task)
        {
            if (task is null)
                throw new ArgumentNullException(nameof(task));
            return async () =>
            {
                await task;
                return new Unit();
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
