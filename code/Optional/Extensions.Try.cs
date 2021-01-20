using System;

namespace Mikodev.Optional
{
    public static partial class Extensions
    {
        public static Result<Unit, Exception> Try(Action func) => Try<Unit, Exception>(MakeFunc(func));

        public static Result<TOk, Exception> Try<TOk>(Func<TOk> func) => Try<TOk, Exception>(func);

        public static Result<Unit, TError> Try<TError>(Action func) where TError : Exception => Try<Unit, TError>(MakeFunc(func));

        public static Result<TOk, TError> Try<TOk, TError>(Func<TOk> func) where TError : Exception
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            try { return Ok(func.Invoke()); } catch (TError error) { return Error(error); }
        }
    }
}
