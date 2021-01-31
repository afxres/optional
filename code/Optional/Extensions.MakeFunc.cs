using System;

namespace Mikodev.Optional
{
    public static partial class Extensions
    {
        private static Func<Unit> MakeFunc(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            return () =>
            {
                action.Invoke();
                return new Unit();
            };
        }

        private static Func<T, Unit> MakeFunc<T>(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            return item =>
            {
                action.Invoke(item);
                return new Unit();
            };
        }
    }
}
