using System;

namespace Mikodev.Optional
{
    public readonly struct Disposable : IDisposable
    {
        private readonly Action action;

        public Disposable(Action action) => this.action = action ?? throw new ArgumentNullException(nameof(action));

        public void Dispose() => action?.Invoke();
    }
}
