using Mikodev.Optional.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Mikodev.Optional
{
    public readonly struct Result<TOk, TError> : IEquatable<Result<TOk, TError>>
    {
        private readonly ResultFlag flag;

        private readonly TOk ok;

        private readonly TError error;

        private Result(ResultFlag flag, TOk ok, TError error)
        {
            Debug.Assert(flag is ResultFlag.Ok or ResultFlag.Error);
            this.flag = flag;
            this.ok = ok;
            this.error = error;
        }

        private void Except()
        {
            if (flag is ResultFlag.Ok or ResultFlag.Error)
                return;
            throw new InvalidOperationException("Can not operate on default value of result!");
        }

        internal ResultFlag Intent(out TOk ok, out TError error)
        {
            Except();
            ok = this.ok;
            error = this.error;
            return flag;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            Except();
            return obj is Result<TOk, TError> result && Equals(result);
        }

        public bool Equals(Result<TOk, TError> other)
        {
            Except();
            other.Except();
            return flag == other.flag && EqualityComparer<TOk>.Default.Equals(ok, other.ok) && EqualityComparer<TError>.Default.Equals(error, other.error);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            Except();
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
            result.Except();
            return new Result<TOk, TError>(result.flag, result.ok, default);
        }

        public static implicit operator Result<TOk, TError>(Result<Unit, TError> result)
        {
            result.Except();
            return new Result<TOk, TError>(result.flag, default, result.error);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => flag is ResultFlag.Ok ? $"Ok({ok})" : flag is ResultFlag.Error ? $"Error({error})" : "Result()";
    }
}
