using System;

namespace Mikodev.Optional
{
    public static partial class Extensions
    {
        public static Disposable Disposable(Action action) => new Disposable(action);
    }
}
