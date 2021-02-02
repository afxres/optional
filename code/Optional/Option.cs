using Mikodev.Optional.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Mikodev.Optional
{
    public readonly partial struct Option<T> : IEquatable<Option<T>>
    {
        private readonly OptionData data;

        private readonly T some;

        private Option(OptionData data, T some)
        {
            Debug.Assert(data is OptionData.None or OptionData.Some);
            this.data = data;
            this.some = some;
        }

        private void Except()
        {
            if (data is OptionData.None or OptionData.Some)
                return;
            throw new InvalidOperationException("Can not operate on default value of option!");
        }

        internal OptionData Intent(out T some)
        {
            Except();
            some = this.some;
            return data;
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
            return data is OptionData.None
                ? other.data is OptionData.None
                : other.data is OptionData.Some && EqualityComparer<T>.Default.Equals(some, other.some);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            Except();
            var hash = data is OptionData.None ? 0 : EqualityComparer<T>.Default.GetHashCode(some);
            return hash ^ (int)data;
        }

        public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

        public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);

        public static Option<T> None() => new Option<T>(OptionData.None, default);

        public static Option<T> Some(T some) => new Option<T>(OptionData.Some, some);

        public static implicit operator Option<T>(Option<Unit> option)
        {
            option.Except();
            if (option.data is OptionData.None)
                return new Option<T>(OptionData.None, default);
            throw new InvalidCastException($"Can not convert 'Some<{typeof(Unit).Name}>' to 'Some<{typeof(T).Name}>'");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => data is OptionData.None ? $"None()" : data is OptionData.Some ? $"Some({some})" : "Option()";
    }
}
