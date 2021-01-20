using Mikodev.Optional.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mikodev.Optional
{
    public readonly struct Result<TOk, TError> : IEquatable<Result<TOk, TError>>
    {
        private readonly ResultFlag flag;

        private readonly TOk ok;

        private readonly TError error;

        private Result(ResultFlag flag, TOk ok, TError error)
        {
            Debug.Assert(flag == ResultFlag.Ok || flag == ResultFlag.Error);
            this.flag = flag;
            this.ok = ok;
            this.error = error;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowInvalid() => throw new InvalidOperationException("Can not operate on default value of result!");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowOnInvalid() { if (flag == ResultFlag.Invalid) ThrowInvalid(); }

        internal ResultFlag GetData(out TOk ok, out TError error)
        {
            ThrowOnInvalid();
            ok = this.ok;
            error = this.error;
            return flag;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            ThrowOnInvalid();
            return obj is Result<TOk, TError> result && Equals(result);
        }

        public bool Equals(Result<TOk, TError> other)
        {
            ThrowOnInvalid();
            other.ThrowOnInvalid();
            return flag == other.flag && EqualityComparer<TOk>.Default.Equals(ok, other.ok) && EqualityComparer<TError>.Default.Equals(error, other.error);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            ThrowOnInvalid();
            var hashCode = 115863327;
            hashCode = hashCode * -1521134295 + flag.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TOk>.Default.GetHashCode(ok);
            hashCode = hashCode * -1521134295 + EqualityComparer<TError>.Default.GetHashCode(error);
            return hashCode;
        }

        public static bool operator ==(Result<TOk, TError> left, Result<TOk, TError> right) => left.Equals(right);

        public static bool operator !=(Result<TOk, TError> left, Result<TOk, TError> right) => !left.Equals(right);

        public static Result<TOk, TError> Ok(TOk ok) => new Result<TOk, TError>(ResultFlag.Ok, ok, default);

        public static Result<TOk, TError> Error(TError error) => new Result<TOk, TError>(ResultFlag.Error, default, error);

        public static implicit operator Result<TOk, TError>(Result<TOk, Unit> result)
        {
            result.ThrowOnInvalid();
            return new Result<TOk, TError>(result.flag, result.ok, default);
        }

        public static implicit operator Result<TOk, TError>(Result<Unit, TError> result)
        {
            result.ThrowOnInvalid();
            return new Result<TOk, TError>(result.flag, default, result.error);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => flag == ResultFlag.Ok ? $"Ok({ok})" : flag == ResultFlag.Error ? $"Error({error})" : "Result()";
    }
}
