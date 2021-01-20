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
            return async () => { await Task.Yield(); return func.Invoke(); };
        }

        private static Func<T, Task<Unit>> MakeTaskFunc<T>(Func<T, Task> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return async item => { await func.Invoke(item); return default; };
        }

        public static Task<Unit> UsingAsync<T>(Func<T> data, Func<T, Task> func) where T : IDisposable => UsingAsync(MakeTaskFunc(data), MakeTaskFunc(func));

        public static Task<Unit> UsingAsync<T>(Func<Task<T>> data, Func<T, Task> func) where T : IDisposable => UsingAsync(data, MakeTaskFunc(func));

        public static Task<U> UsingAsync<T, U>(Func<T> data, Func<T, Task<U>> func) where T : IDisposable => UsingAsync(MakeTaskFunc(data), func);

        public static async Task<U> UsingAsync<T, U>(Func<Task<T>> data, Func<T, Task<U>> func) where T : IDisposable
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            using (var item = await data.Invoke())
                return await func.Invoke(item);
        }
    }
}
