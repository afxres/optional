using System;

namespace Mikodev.Optional
{
    public static partial class Extensions
    {
        public static Unit Using<T>(Func<T> data, Action<T> func) where T : IDisposable => Using(data, MakeFunc(func));

        public static U Using<T, U>(Func<T> data, Func<T, U> func) where T : IDisposable
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            using (var item = data.Invoke())
                return func.Invoke(item);
        }
    }
}
