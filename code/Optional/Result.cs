using Mikodev.Optional.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Mikodev.Optional
{
    public readonly struct Result<TOk, TError> : IEquatable<Result<TOk, TError>>
    {
        private readonly ResultData data;

        private readonly TOk ok;

        private readonly TError error;

        private Result(ResultData data, TOk ok, TError error)
        {
            Debug.Assert(data is ResultData.Ok or ResultData.Error);
            this.data = data;
            this.ok = ok;
            this.error = error;
        }

        private void Except()
        {
            if (data is ResultData.Ok or ResultData.Error)
                return;
            throw new InvalidOperationException("Can not operate on default value of result!");
        }

        internal ResultData Intent(out TOk ok, out TError error)
        {
            Except();
            ok = this.ok;
            error = this.error;
            return data;
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
            return (data is ResultData.Ok)
                ? other.data is ResultData.Ok && EqualityComparer<TOk>.Default.Equals(ok, other.ok)
                : other.data is ResultData.Error && EqualityComparer<TError>.Default.Equals(error, other.error);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            Except();
            var hash = data is ResultData.Ok
                ? EqualityComparer<TOk>.Default.GetHashCode(ok)
                : EqualityComparer<TError>.Default.GetHashCode(error);
            return hash ^ (int)data;
        }

        public static bool operator ==(Result<TOk, TError> left, Result<TOk, TError> right) => left.Equals(right);

        public static bool operator !=(Result<TOk, TError> left, Result<TOk, TError> right) => !left.Equals(right);

        public static Result<TOk, TError> Ok(TOk ok) => new Result<TOk, TError>(ResultData.Ok, ok, default);

        public static Result<TOk, TError> Error(TError error) => new Result<TOk, TError>(ResultData.Error, default, error);

        public static implicit operator Result<TOk, TError>(Result<TOk, Unit> result)
        {
            result.Except();
            if (result.data is ResultData.Ok)
                return new Result<TOk, TError>(ResultData.Ok, result.ok, default);
            throw new InvalidCastException($"Can not convert 'Error<{typeof(Unit).Name}>' to 'Error<{typeof(TError).Name}>'");
        }

        public static implicit operator Result<TOk, TError>(Result<Unit, TError> result)
        {
            result.Except();
            if (result.data is ResultData.Error)
                return new Result<TOk, TError>(ResultData.Error, default, result.error);
            throw new InvalidCastException($"Can not convert 'Ok<{typeof(Unit).Name}>' to 'Ok<{typeof(TOk).Name}>'");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => data is ResultData.Ok ? $"Ok({ok})" : data is ResultData.Error ? $"Error({error})" : "Result()";
    }
}
