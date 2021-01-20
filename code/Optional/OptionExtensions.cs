using Mikodev.Optional.Internal;
using System;

namespace Mikodev.Optional
{
    /// <summary>
    /// https://doc.rust-lang.org/std/option/enum.Option.html
    /// </summary>
    public static class OptionExtensions
    {
        public static bool IsSome<T>(this Option<T> option)
        {
            return option.GetData(out _) == OptionFlag.Some;
        }

        public static bool IsNone<T>(this Option<T> option)
        {
            return option.GetData(out _) == OptionFlag.None;
        }

        public static T Except<T>(this Option<T> option, string message)
        {
            if (option.GetData(out var data) == OptionFlag.Some)
                return data;
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            throw new OptionException(message);
        }

        public static T Unwrap<T>(this Option<T> option)
        {
            if (option.GetData(out var data) == OptionFlag.Some)
                return data;
            throw new OptionException();
        }

        public static T UnwrapOr<T>(this Option<T> option, T @default)
        {
            return option.GetData(out var data) == OptionFlag.Some ? data : @default;
        }

        public static T UnwrapOrElse<T>(this Option<T> option, Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return option.GetData(out var data) == OptionFlag.Some ? data : func.Invoke();
        }

        public static Option<U> Map<T, U>(this Option<T> option, Func<T, U> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return option.GetData(out var data) == OptionFlag.Some ? Option<U>.Some(func.Invoke(data)) : Option<U>.None();
        }

        public static U MapOr<T, U>(this Option<T> option, U @default, Func<T, U> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return option.GetData(out var data) == OptionFlag.Some ? func.Invoke(data) : @default;
        }

        public static U MapOrElse<T, U>(this Option<T> option, Func<U> @default, Func<T, U> func)
        {
            if (@default == null)
                throw new ArgumentNullException(nameof(@default));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return option.GetData(out var data) == OptionFlag.Some ? func.Invoke(data) : @default.Invoke();
        }

        public static Result<T, E> OkOr<T, E>(this Option<T> option, E error)
        {
            return option.GetData(out var data) == OptionFlag.Some ? Result<T, E>.Ok(data) : Result<T, E>.Error(error);
        }

        public static Result<T, E> OkOrElse<T, E>(this Option<T> option, Func<E> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return option.GetData(out var data) == OptionFlag.Some ? Result<T, E>.Ok(data) : Result<T, E>.Error(func.Invoke());
        }

        public static Option<U> And<T, U>(this Option<T> option, Option<U> other)
        {
            return option.GetData(out _) == OptionFlag.Some ? other : Option<U>.None();
        }

        public static Option<U> AndThen<T, U>(this Option<T> option, Func<T, Option<U>> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return option.GetData(out var data) == OptionFlag.Some ? func.Invoke(data) : Option<U>.None();
        }

        public static Option<T> Or<T>(this Option<T> option, Option<T> other)
        {
            return option.GetData(out _) == OptionFlag.Some ? option : other;
        }

        public static Option<T> OrElse<T>(this Option<T> option, Func<Option<T>> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return option.GetData(out _) == OptionFlag.Some ? option : func.Invoke();
        }

        public static Option<T> Xor<T>(this Option<T> option, Option<T> other)
        {
            return option.GetData(out _) == OptionFlag.Some
                ? other.GetData(out _) == OptionFlag.Some ? Option<T>.None() : option
                : other.GetData(out _) == OptionFlag.Some ? other : Option<T>.None();
        }

        public static T UnwrapOrDefault<T>(this Option<T> option)
        {
            return option.GetData(out var data) == OptionFlag.Some ? data : default;
        }

        public static Result<Option<T>, E> Transpose<T, E>(this Option<Result<T, E>> option)
        {
            return option.GetData(out var data) == OptionFlag.Some
                ? data.GetData(out var ok, out var error) == ResultFlag.Ok
                    ? Result<Option<T>, E>.Ok(Option<T>.Some(ok))
                    : Result<Option<T>, E>.Error(error)
                : Result<Option<T>, E>.Ok(Option<T>.None());
        }

        public static Option<T> Flatten<T>(this Option<Option<T>> option)
        {
            return option.GetData(out var data) == OptionFlag.Some && data.GetData(out var item) == OptionFlag.Some
                ? Option<T>.Some(item)
                : Option<T>.None();
        }
    }
}
