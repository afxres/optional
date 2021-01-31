using Mikodev.Optional.Internal;
using System;

namespace Mikodev.Optional
{
    /// <summary>
    /// https://doc.rust-lang.org/std/result/enum.Result.html
    /// </summary>
    public static class ResultExtensions
    {
        public static bool IsOk<T, E>(this Result<T, E> result)
        {
            return result.Intent(out _, out _) is ResultData.Ok;
        }

        public static bool IsError<T, E>(this Result<T, E> result)
        {
            return result.Intent(out _, out _) is ResultData.Error;
        }

        public static Option<T> Ok<T, E>(this Result<T, E> result)
        {
            return result.Intent(out var ok, out _) is ResultData.Ok ? Option<T>.Some(ok) : Option<T>.None();
        }

        public static Option<E> Error<T, E>(this Result<T, E> result)
        {
            return result.Intent(out _, out var error) is ResultData.Error ? Option<E>.Some(error) : Option<E>.None();
        }

        public static Result<U, E> Map<T, U, E>(this Result<T, E> result, Func<T, U> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return result.Intent(out var ok, out var error) is ResultData.Ok ? Result<U, E>.Ok(func.Invoke(ok)) : Result<U, E>.Error(error);
        }

        public static U MapOrElse<T, U, E>(this Result<T, E> result, Func<E, U> fallback, Func<T, U> func)
        {
            if (fallback == null)
                throw new ArgumentNullException(nameof(fallback));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return result.Intent(out var ok, out var error) is ResultData.Ok
                ? func.Invoke(ok)
                : fallback.Invoke(error);
        }

        public static Result<T, F> MapError<T, E, F>(this Result<T, E> result, Func<E, F> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return result.Intent(out var ok, out var error) is ResultData.Error ? Result<T, F>.Error(func.Invoke(error)) : Result<T, F>.Ok(ok);
        }

        public static Result<U, E> And<T, U, E>(this Result<T, E> result, Result<U, E> other)
        {
            return result.Intent(out _, out var error) is ResultData.Ok ? other : Result<U, E>.Error(error);
        }

        public static Result<U, E> AndThen<T, U, E>(this Result<T, E> result, Func<T, Result<U, E>> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return result.Intent(out var ok, out var error) is ResultData.Ok ? func.Invoke(ok) : Result<U, E>.Error(error);
        }

        public static Result<T, F> Or<T, E, F>(this Result<T, E> result, Result<T, F> other)
        {
            return result.Intent(out var ok, out _) is ResultData.Error ? other : Result<T, F>.Ok(ok);
        }

        public static Result<T, F> OrElse<T, E, F>(this Result<T, E> result, Func<E, Result<T, F>> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return result.Intent(out var ok, out var error) is ResultData.Error ? func.Invoke(error) : Result<T, F>.Ok(ok);
        }

        public static T UnwrapOr<T, E>(this Result<T, E> result, T @default)
        {
            return result.Intent(out var ok, out _) is ResultData.Ok ? ok : @default;
        }

        public static T UnwrapOrElse<T, E>(this Result<T, E> result, Func<E, T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return result.Intent(out var ok, out var error) is ResultData.Ok ? ok : func.Invoke(error);
        }

        public static T Unwrap<T, E>(this Result<T, E> result)
        {
            if (result.Intent(out var ok, out _) is ResultData.Ok)
                return ok;
            throw new ResultException();
        }

        public static T Except<T, E>(this Result<T, E> result, string message)
        {
            if (result.Intent(out var ok, out _) is ResultData.Ok)
                return ok;
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            throw new ResultException(message);
        }

        public static E UnwrapError<T, E>(this Result<T, E> result)
        {
            if (result.Intent(out _, out var error) is ResultData.Error)
                return error;
            throw new ResultException();
        }

        public static E ExceptError<T, E>(this Result<T, E> result, string message)
        {
            if (result.Intent(out _, out var error) is ResultData.Error)
                return error;
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            throw new ResultException(message);
        }

        public static T UnwrapOrDefault<T, E>(this Result<T, E> result)
        {
            return result.Intent(out var ok, out _) is ResultData.Ok ? ok : (default);
        }

        public static Option<Result<T, E>> Transpose<T, E>(this Result<Option<T>, E> result)
        {
            return result.Intent(out var ok, out var error) is ResultData.Ok
                ? ok.Intent(out var data) is OptionData.Some
                    ? Option<Result<T, E>>.Some(Result<T, E>.Ok(data))
                    : Option<Result<T, E>>.None()
                : Option<Result<T, E>>.Some(Result<T, E>.Error(error));
        }
    }
}
