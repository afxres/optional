using Mikodev.Optional.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mikodev.Optional
{
    public readonly partial struct Option<T> : IEquatable<Option<T>>
    {
        private readonly OptionFlag flag;

        private readonly T data;

        private Option(OptionFlag flag, T data)
        {
            Debug.Assert(flag == OptionFlag.None || flag == OptionFlag.Some);
            this.flag = flag;
            this.data = data;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowInvalid() => throw new InvalidOperationException("Can not operate on default value of option!");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowOnInvalid() { if (flag == OptionFlag.Invalid) ThrowInvalid(); }

        internal OptionFlag GetData(out T data)
        {
            ThrowOnInvalid();
            data = this.data;
            return flag;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            ThrowOnInvalid();
            return obj is Option<T> option && Equals(option);
        }

        public bool Equals(Option<T> other)
        {
            ThrowOnInvalid();
            other.ThrowOnInvalid();
            return flag == other.flag && EqualityComparer<T>.Default.Equals(data, other.data);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            ThrowOnInvalid();
            var hashCode = -814067692;
            hashCode = hashCode * -1521134295 + flag.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(data);
            return hashCode;
        }

        public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

        public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);

        public static Option<T> None() => new Option<T>(OptionFlag.None, default);

        public static Option<T> Some(T data) => new Option<T>(OptionFlag.Some, data);

        public static implicit operator Option<T>(Option<Unit> option)
        {
            option.ThrowOnInvalid();
            return new Option<T>(option.flag, default);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => flag == OptionFlag.None ? $"None()" : flag == OptionFlag.Some ? $"Some({data})" : "Option()";
    }
}
