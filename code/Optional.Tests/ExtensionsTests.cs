using Mikodev.Optional.Tests.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Mikodev.Optional.Extensions;

namespace Mikodev.Optional.Tests
{
    public class ExtensionsTests
    {
        [Theory(DisplayName = "None")]
        [InlineData(0)]
        [InlineData("empty")]
        public void TestNone<T>(T data)
        {
            var source = Option<T>.None();
            Assert.Equal(source, Assert.IsType<Option<Unit>>(None()));
            Assert.Equal(source, None<T>());
        }

        [Theory(DisplayName = "Some")]
        [InlineData(0)]
        [InlineData("empty")]
        public void TestSome<T>(T data)
        {
            var source = Option<T>.Some(data);
            Assert.Equal(source, Some(data));
        }

        [Theory(DisplayName = "Ok")]
        [InlineData(0, "empty")]
        [InlineData("empty", 0)]
        public void TestOk<TOk, TError>(TOk ok, TError error)
        {
            var source = Result<TOk, TError>.Ok(ok);
            Assert.Equal(source, Assert.IsType<Result<TOk, Unit>>(Ok(ok)));
            Assert.Equal(source, Ok<TOk, TError>(ok));
        }

        [Theory(DisplayName = "Error")]
        [InlineData(0, "empty")]
        [InlineData("empty", 0)]
        public void TestError<TOk, TError>(TOk ok, TError error)
        {
            var source = Result<TOk, TError>.Error(error);
            Assert.Equal(source, Assert.IsType<Result<Unit, TError>>(Error(error)));
            Assert.Equal(source, Error<TOk, TError>(error));
        }

        [Fact(DisplayName = "Lock")]
        public void TestLock()
        {
            var locker = new object();
            _ = Lock(locker, () =>
            {
                var alpha = false;
                Monitor.TryEnter(locker, ref alpha);
                Assert.True(alpha);

                Task.Run(() =>
                {
                    var bravo = false;
                    Monitor.TryEnter(locker, ref bravo);
                    Assert.False(bravo);
                })
                .Wait();

                var delta = false;
                Monitor.TryEnter(locker, ref delta);
                Assert.True(delta);
            });
        }

        [Theory(DisplayName = "Lock T")]
        [InlineData(1)]
        [InlineData("single")]
        public void TestLockT<T>(T data)
        {
            var locker = new object();
            var result = Lock(locker, () =>
            {
                var alpha = false;
                Monitor.TryEnter(locker, ref alpha);
                Assert.True(alpha);

                Task.Run(() =>
                {
                    var bravo = false;
                    Monitor.TryEnter(locker, ref bravo);
                    Assert.False(bravo);
                })
                .Wait();

                var delta = false;
                Monitor.TryEnter(locker, ref delta);
                Assert.True(delta);

                return data;
            });
            Assert.Equal(data, result);
        }

        [Fact(DisplayName = "Try")]
        public void TestTry()
        {
            var alpha = Try(() => { });
            Assert.True(Assert.IsType<Result<Unit, Exception>>(alpha).IsOk());
            var bravo = Try(() => throw new InvalidOperationException());
            Assert.True(Assert.IsType<Result<Unit, Exception>>(bravo).IsError());
            _ = Assert.IsType<InvalidOperationException>(bravo.UnwrapError());
        }

        [Theory(DisplayName = "Try T")]
        [InlineData(0)]
        [InlineData("zero")]
        public void TestTryT<T>(T data)
        {
            var alpha = Try(() => data);
            Assert.True(Assert.IsType<Result<T, Exception>>(alpha).IsOk());
            Assert.Equal(data, alpha.Unwrap());
            var bravo = Try<T>(() => throw new ArgumentException());
            Assert.True(Assert.IsType<Result<T, Exception>>(bravo).IsError());
            _ = Assert.IsType<ArgumentException>(bravo.UnwrapError());
        }

        [Fact(DisplayName = "Try Exception")]
        public void TestTryException()
        {
            var alpha = Try<ArgumentException>(() => { });
            Assert.True(Assert.IsType<Result<Unit, ArgumentException>>(alpha).IsOk());
            var bravo = Try<ArgumentNullException>(new Action(() => throw new ArgumentNullException()));
            Assert.True(Assert.IsType<Result<Unit, ArgumentNullException>>(bravo).IsError());
            _ = Assert.IsType<ArgumentNullException>(bravo.UnwrapError());
        }

        [Theory(DisplayName = "Try T Exception")]
        [InlineData(0)]
        [InlineData("zero")]
        public void TestTryTException<T>(T data)
        {
            var alpha = Try<T, ArgumentOutOfRangeException>(() => data);
            Assert.True(Assert.IsType<Result<T, ArgumentOutOfRangeException>>(alpha).IsOk());
            Assert.Equal(data, alpha.Unwrap());
            var bravo = Try<T, FileLoadException>(() => throw new FileLoadException());
            Assert.True(Assert.IsType<Result<T, FileLoadException>>(bravo).IsError());
            _ = Assert.IsType<FileLoadException>(bravo.UnwrapError());
        }

        [Fact(DisplayName = "Try Async Task")]
        public async Task TestTryAsyncTask()
        {
            var alpha = await TryAsync((Task)Task.FromResult(0));
            Assert.True(Assert.IsType<Result<Unit, Exception>>(alpha).IsOk());
            var bravo = await TryAsync(Task.FromException(new ArgumentException()));
            Assert.True(Assert.IsType<Result<Unit, Exception>>(bravo).IsError());
            _ = Assert.IsType<ArgumentException>(bravo.UnwrapError());
        }

        [Theory(DisplayName = "Try Async Task T")]
        [InlineData(0)]
        [InlineData("zero")]
        public async Task TestTryAsyncTaskT<T>(T data)
        {
            var alpha = await TryAsync(Task.FromResult(data));
            Assert.True(Assert.IsType<Result<T, Exception>>(alpha).IsOk());
            Assert.Equal(data, alpha.Unwrap());
            var bravo = await TryAsync(Task.FromException<T>(new ArgumentNullException()));
            Assert.True(Assert.IsType<Result<T, Exception>>(bravo).IsError());
            _ = Assert.IsType<ArgumentNullException>(bravo.UnwrapError());
        }

        [Fact(DisplayName = "Try Async Func Task")]
        public async Task TestTryAsyncFuncTask()
        {
            var alpha = await TryAsync(() => (Task)Task.FromResult(0));
            Assert.True(Assert.IsType<Result<Unit, Exception>>(alpha).IsOk());
            var bravo = await TryAsync(() => Task.FromException(new ArgumentOutOfRangeException()));
            Assert.True(Assert.IsType<Result<Unit, Exception>>(bravo).IsError());
            _ = Assert.IsType<ArgumentOutOfRangeException>(bravo.UnwrapError());
        }

        [Theory(DisplayName = "Try Async Func Task T")]
        [InlineData(0)]
        [InlineData("zero")]
        public async Task TestTryAsyncFuncTaskT<T>(T data)
        {
            var alpha = await TryAsync(() => Task.FromResult(data));
            Assert.True(Assert.IsType<Result<T, Exception>>(alpha).IsOk());
            Assert.Equal(data, alpha.Unwrap());
            var bravo = await TryAsync(() => Task.FromException<T>(new ArgumentNullException()));
            Assert.True(Assert.IsType<Result<T, Exception>>(bravo).IsError());
            _ = Assert.IsType<ArgumentNullException>(bravo.UnwrapError());
        }

        [Fact(DisplayName = "Try Async Task Exception")]
        public async Task TestTryAsyncTaskException()
        {
            var alpha = await TryAsync<ArgumentNullException>(Task.FromResult(0));
            Assert.True(Assert.IsType<Result<Unit, ArgumentNullException>>(alpha).IsOk());
            var bravo = await TryAsync<ArgumentOutOfRangeException>(Task.FromException(new ArgumentOutOfRangeException()));
            Assert.True(Assert.IsType<Result<Unit, ArgumentOutOfRangeException>>(bravo).IsError());
            _ = Assert.IsType<ArgumentOutOfRangeException>(bravo.UnwrapError());
        }

        [Theory(DisplayName = "Try Async Task T Exception")]
        [InlineData(0)]
        [InlineData("zero")]
        public async Task TestTryAsyncTaskTException<T>(T data)
        {
            var alpha = await TryAsync<T, InvalidOperationException>(Task.FromResult(data));
            Assert.True(Assert.IsType<Result<T, InvalidOperationException>>(alpha).IsOk());
            Assert.Equal(data, alpha.Unwrap());
            var bravo = await TryAsync<T, InvalidDataException>(Task.FromException<T>(new InvalidDataException()));
            Assert.True(Assert.IsType<Result<T, InvalidDataException>>(bravo).IsError());
            _ = Assert.IsType<InvalidDataException>(bravo.UnwrapError());
        }

        [Fact(DisplayName = "Try Async Func Task Exception")]
        public async Task TestTryAsyncFuncTaskException()
        {
            var alpha = await TryAsync<FileLoadException>(() => Task.FromResult(0));
            Assert.True(Assert.IsType<Result<Unit, FileLoadException>>(alpha).IsOk());
            var bravo = await TryAsync<FileNotFoundException>(() => Task.FromException(new FileNotFoundException()));
            Assert.True(Assert.IsType<Result<Unit, FileNotFoundException>>(bravo).IsError());
            _ = Assert.IsType<FileNotFoundException>(bravo.UnwrapError());
        }

        [Theory(DisplayName = "Try Async Func Task T Exception")]
        [InlineData(0)]
        [InlineData("zero")]
        public async Task TestTryAsyncFuncTaskTException<T>(T data)
        {
            var alpha = await TryAsync<T, FormatException>(() => Task.FromResult(data));
            Assert.True(Assert.IsType<Result<T, FormatException>>(alpha).IsOk());
            Assert.Equal(data, alpha.Unwrap());
            var bravo = await TryAsync<T, NotSupportedException>(() => Task.FromException<T>(new NotSupportedException()));
            Assert.True(Assert.IsType<Result<T, NotSupportedException>>(bravo).IsError());
            _ = Assert.IsType<NotSupportedException>(bravo.UnwrapError());
        }

        [Fact(DisplayName = "Using")]
        public void TestUsing()
        {
            var disposable = new Disposable<int>(1);
            _ = Using(() => disposable, x => { Assert.False(disposable.IsDisposed); Assert.Equal(1, x.Value); });
            Assert.True(disposable.IsDisposed);
        }

        [Fact(DisplayName = "Using T")]
        public void TestUsingT()
        {
            var disposable = new Disposable<int>(2);
            var result = Using(() => disposable, x => { Assert.False(disposable.IsDisposed); return x.Value * 2; });
            Assert.True(disposable.IsDisposed);
            Assert.Equal(4, result);
        }

        [Fact(DisplayName = "Using Async Func T Task")]
        public async Task TestUsingAsyncFuncTTask()
        {
            var disposable = new Disposable<string>("value");
            _ = await UsingAsync(() => disposable, async x => { await Task.Yield(); Assert.False(disposable.IsDisposed); Assert.Equal("value", x.Value); });
            Assert.True(disposable.IsDisposed);
        }

        [Fact(DisplayName = "Using Async Func T Task T")]
        public async Task TestUsingAsyncFuncTTaskT()
        {
            var disposable = new Disposable<string>("value");
            var result = await UsingAsync(() => disposable, async x => { await Task.Yield(); Assert.False(disposable.IsDisposed); return x.Value.ToUpper(); });
            Assert.True(disposable.IsDisposed);
            Assert.Equal("VALUE", result);
        }

        [Fact(DisplayName = "Using Async Func Task T Task")]
        public async Task TestUsingAsyncFuncTaskTTask()
        {
            var disposable = new Disposable<string>("data");
            _ = await UsingAsync(() => Task.FromResult(disposable), async x => { await Task.Yield(); Assert.False(disposable.IsDisposed); Assert.Equal("data", x.Value); });
            Assert.True(disposable.IsDisposed);
        }

        [Fact(DisplayName = "Using Async Func Task T Task T")]
        public async Task TestUsingAsyncFuncTaskTTaskT()
        {
            var disposable = new Disposable<string>("item");
            var result = await UsingAsync(() => Task.FromResult(disposable), async x => { await Task.Yield(); Assert.False(disposable.IsDisposed); return x.Value.ToUpper(); });
            Assert.True(disposable.IsDisposed);
            Assert.Equal("ITEM", result);
        }
    }
}
