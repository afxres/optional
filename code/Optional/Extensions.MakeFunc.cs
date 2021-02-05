using System;

namespace Mikodev.Optional
{
    public static partial class Extensions
    {
        private static Func<Unit> MakeFunc(Action func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return () =>
            {
                func.Invoke();
                return new Unit();
            };
        }

        private static Func<T, Unit> MakeFunc<T>(Action<T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return item =>
            {
                func.Invoke(item);
                return new Unit();
            };
        }
    }
}
