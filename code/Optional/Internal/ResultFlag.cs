namespace Mikodev.Optional.Internal
{
    internal enum ResultFlag : byte { Invalid = default, Ok = 0x7F, Error = 0x80 }
}
