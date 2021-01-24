using Mikodev.Optional.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Mikodev.Optional
{
    public readonly partial struct Option<T> : IEquatable<Option<T>>
    {
        private readonly OptionFlag flag;

        private readonly T data;

        private Option(OptionFlag flag, T data)
        {
            Debug.Assert(flag is OptionFlag.None or OptionFlag.Some);
            this.flag = flag;
            this.data = data;
        }

        private void Except()
        {
            if (flag is OptionFlag.None or OptionFlag.Some)
                return;
            throw new InvalidOperationException("Can not operate on default value of option!");
        }

        internal OptionFlag Intent(out T data)
        {
            Except();
            data = this.data;
            return flag;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            Except();
            return obj is Option<T> option && Equals(option);
        }

        public bool Equals(Option<T> other)
        {
            Except();
            other.Except();
            return flag == other.flag && EqualityComparer<T>.Default.Equals(data, other.data);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            Except();
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
            option.Except();
            return new Option<T>(option.flag, default);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => flag is OptionFlag.None ? $"None()" : flag is OptionFlag.Some ? $"Some({data})" : "Option()";
    }
}
