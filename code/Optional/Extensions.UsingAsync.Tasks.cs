using System;
using System.Threading.Tasks;

namespace Mikodev.Optional
{
    public static partial class Extensions
    {
        public static Task<Unit> UsingAsync<T>(Func<T> data, Func<T, Task> func) where T : IDisposable => UsingAsync(MakeTaskFunc(data), MakeTaskFunc(func));

        public static Task<Unit> UsingAsync<T>(Func<Task<T>> data, Func<T, Task> func) where T : IDisposable => UsingAsync(data, MakeTaskFunc(func));

        public static Task<U> UsingAsync<T, U>(Func<T> data, Func<T, Task<U>> func) where T : IDisposable => UsingAsync(MakeTaskFunc(data), func);

        public static Task<U> UsingAsync<T, U>(Func<Task<T>> data, Func<T, Task<U>> func) where T : IDisposable
        {
            static async Task<U> Invoke(Func<Task<T>> data, Func<T, Task<U>> func)
            {
                using (var item = await data.Invoke())
                    return await func.Invoke(item);
            }

            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return Invoke(data, func);
        }
    }
}
