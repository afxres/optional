using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Mikodev.Optional
{
    [StructLayout(LayoutKind.Sequential, Size = sizeof(byte))]
    public readonly struct Unit
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => obj is Unit;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => default;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => "()";
    }
}
