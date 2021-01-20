using System;

namespace Mikodev.Optional.Tests.Models
{
    public class Disposable<T> : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        public T Value { get; }

        public Disposable(T value) => Value = value;

        public void Dispose() => IsDisposed = true;
    }
}
