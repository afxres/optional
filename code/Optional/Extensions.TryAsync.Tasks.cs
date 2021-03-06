﻿using System;
using System.Threading.Tasks;

namespace Mikodev.Optional
{
    public static partial class Extensions
    {
        public static Task<Result<Unit, Exception>> TryAsync(Task task) => TryAsync<Exception>(task);

        public static Task<Result<Unit, Exception>> TryAsync(Func<Task> func) => TryAsync<Exception>(func);

        public static Task<Result<TOk, Exception>> TryAsync<TOk>(Task<TOk> task) => TryAsync<TOk, Exception>(task);

        public static Task<Result<TOk, Exception>> TryAsync<TOk>(Func<Task<TOk>> func) => TryAsync<TOk, Exception>(func);

        public static Task<Result<Unit, TError>> TryAsync<TError>(Task task) where TError : Exception => TryAsync<Unit, TError>(MakeTaskFunc(task));

        public static Task<Result<Unit, TError>> TryAsync<TError>(Func<Task> func) where TError : Exception => TryAsync<Unit, TError>(MakeTaskFunc(func));

        public static Task<Result<TOk, TError>> TryAsync<TOk, TError>(Task<TOk> task) where TError : Exception => TryAsync<TOk, TError>(MakeTaskFunc(task));

        public static Task<Result<TOk, TError>> TryAsync<TOk, TError>(Func<Task<TOk>> func) where TError : Exception
        {
            static async Task<Result<TOk, TError>> Invoke(Func<Task<TOk>> func)
            {
                try
                {
                    return Ok(await func.Invoke());
                }
                catch (TError error)
                {
                    return Error(error);
                }
            }

            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return Invoke(func);
        }
    }
}
