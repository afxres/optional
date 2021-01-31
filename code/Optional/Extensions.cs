using System;

namespace Mikodev.Optional
{
    public static partial class Extensions
    {
        public static Option<Unit> None() => None<Unit>();

        public static Option<T> None<T>() => Option<T>.None();

        public static Option<T> Some<T>(T data) => Option<T>.Some(data);

        public static Result<TOk, Unit> Ok<TOk>(TOk ok) => Ok<TOk, Unit>(ok);

        public static Result<TOk, TError> Ok<TOk, TError>(TOk ok) => Result<TOk, TError>.Ok(ok);

        public static Result<Unit, TError> Error<TError>(TError error) => Error<Unit, TError>(error);

        public static Result<TOk, TError> Error<TOk, TError>(TError error) => Result<TOk, TError>.Error(error);

        public static Unit Lock<T>(T locker, Action action) where T : class => Lock(locker, MakeFunc(action));

        public static U Lock<T, U>(T locker, Func<U> func) where T : class
        {
            if (locker == null)
                throw new ArgumentNullException(nameof(locker));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            lock (locker)
                return func.Invoke();
        }
    }
}
