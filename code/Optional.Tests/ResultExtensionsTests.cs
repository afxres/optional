using Xunit;

namespace Mikodev.Optional.Tests
{
    public class ResultExtensionsTests
    {
        [Fact]
        public void IsOk()
        {
            var alpha = Result<int, string>.Ok(-3);
            var bravo = Result<int, string>.Error("Some error message");
            Assert.True(alpha.IsOk());
            Assert.False(bravo.IsOk());
        }

        [Fact]
        public void IsError()
        {
            var alpha = Result<int, string>.Ok(-3);
            var bravo = Result<int, string>.Error("Some error message");
            Assert.False(alpha.IsError());
            Assert.True(bravo.IsError());
        }

        [Fact]
        public void Ok()
        {
            var alpha = Result<int, string>.Ok(2);
            var bravo = Result<int, string>.Error("Nothing here");
            Assert.Equal(Option<int>.Some(2), alpha.Ok());
            Assert.Equal(Option<int>.None(), bravo.Ok());
        }

        [Fact]
        public void Error()
        {
            var alpha = Result<int, string>.Ok(2);
            var bravo = Result<int, string>.Error("Nothing here");
            Assert.Equal(Option<string>.None(), alpha.Error());
            Assert.Equal(Option<string>.Some("Nothing here"), bravo.Error());
        }

        [Fact]
        public void Map()
        {
            var alpha = Result<int, string>.Ok(2);
            var bravo = Result<int, string>.Error("Invalid character");
            Assert.Equal(Result<double, string>.Ok(4.0), alpha.Map(x => x * 2.0));
            Assert.Equal(Result<double, string>.Error("Invalid character"), bravo.Map(x => x * 2.0));
        }

        [Fact]
        public void MapOrElse()
        {
            var k = 21;
            var alpha = Result<string, string>.Ok("foo");
            var bravo = Result<string, string>.Error("bar");
            Assert.Equal(3, alpha.MapOrElse(_ => k * 2, x => x.Length));
            Assert.Equal(42, bravo.MapOrElse(_ => k * 2, x => x.Length));
        }

        [Fact]
        public void MapError()
        {
            string ToString(int data) => $"error code: {data}";

            var alpha = Result<int, int>.Ok(2);
            var bravo = Result<int, int>.Error(13);
            Assert.Equal(Result<int, string>.Ok(2), alpha.MapError(ToString));
            Assert.Equal(Result<int, string>.Error("error code: 13"), bravo.MapError(ToString));
        }

        [Fact]
        public void And()
        {
            var a = Result<int, string>.Ok(2);
            var b = Result<string, string>.Error("late error");
            Assert.Equal(Result<string, string>.Error("late error"), a.And(b));

            var e = Result<int, string>.Error("early error");
            var f = Result<string, string>.Ok("foo");
            Assert.Equal(Result<string, string>.Error("early error"), e.And(f));

            var m = Result<int, string>.Error("not a 2");
            var n = Result<string, string>.Error("late error");
            Assert.Equal(Result<string, string>.Error("not a 2"), m.And(n));

            var x = Result<int, string>.Ok(2);
            var y = Result<string, string>.Ok("different result type");
            Assert.Equal(Result<string, string>.Ok("different result type"), x.And(y));
        }

        [Fact]
        public void AndThen()
        {
            Result<int, int> Square(int x) => Result<int, int>.Ok(x * x);
            Result<int, int> Error(int x) => Result<int, int>.Error(x);

            Assert.Equal(Result<int, int>.Ok(16), Result<int, int>.Ok(2).AndThen(Square).AndThen(Square));
            Assert.Equal(Result<int, int>.Error(4), Result<int, int>.Ok(2).AndThen(Square).AndThen(Error));
            Assert.Equal(Result<int, int>.Error(2), Result<int, int>.Ok(2).AndThen(Error).AndThen(Square));
            Assert.Equal(Result<int, int>.Error(3), Result<int, int>.Error(3).AndThen(Square).AndThen(Square));
        }

        [Fact]
        public void Or()
        {
            var a = Result<int, string>.Ok(2);
            var b = Result<int, string>.Error("late error");
            Assert.Equal(Result<int, string>.Ok(2), a.Or(b));

            var e = Result<int, string>.Error("early error");
            var f = Result<int, string>.Ok(2);
            Assert.Equal(Result<int, string>.Ok(2), e.Or(f));

            var m = Result<int, string>.Error("not a 2");
            var n = Result<int, string>.Error("late error");
            Assert.Equal(Result<int, string>.Error("late error"), m.Or(n));

            var x = Result<int, string>.Ok(2);
            var y = Result<int, string>.Ok(100);
            Assert.Equal(Result<int, string>.Ok(2), x.Or(y));
        }

        [Fact]
        public void OrElse()
        {
            Result<int, int> Square(int x) => Result<int, int>.Ok(x * x);
            Result<int, int> Error(int x) => Result<int, int>.Error(x);

            Assert.Equal(Result<int, int>.Ok(2), Result<int, int>.Ok(2).OrElse(Square).OrElse(Square));
            Assert.Equal(Result<int, int>.Ok(2), Result<int, int>.Ok(2).OrElse(Error).OrElse(Square));
            Assert.Equal(Result<int, int>.Ok(9), Result<int, int>.Error(3).OrElse(Square).OrElse(Error));
            Assert.Equal(Result<int, int>.Error(3), Result<int, int>.Error(3).OrElse(Error).OrElse(Error));
        }

        [Fact]
        public void UnwrapOr()
        {
            var alpha = Result<int, string>.Ok(9);
            var bravo = Result<int, string>.Error("error");
            Assert.Equal(9, alpha.UnwrapOr(2));
            Assert.Equal(2, bravo.UnwrapOr(2));
        }

        [Fact]
        public void UnwrapOrElse()
        {
            int Count(string x) => x.Length;
            Assert.Equal(2, Result<int, string>.Ok(2).UnwrapOrElse(Count));
            Assert.Equal(3, Result<int, string>.Error("foo").UnwrapOrElse(Count));
        }

        [Fact]
        public void Unwrap()
        {
            var alpha = Result<int, string>.Ok(2);
            var bravo = Result<int, string>.Error("emergency failure");
            Assert.Equal(2, alpha.Unwrap());
            _ = Assert.Throws<ResultException>(() => bravo.Unwrap());
        }

        [Fact]
        public void Expect()
        {
            var alpha = Result<int, string>.Error("emergency failure");
            var error = Assert.Throws<ResultException>(() => alpha.Except("Testing expect"));
            Assert.Equal("Testing expect", error.Message);
        }

        [Fact]
        public void UnwrapError()
        {
            var alpha = Result<int, string>.Ok(2);
            var bravo = Result<int, string>.Error("emergency failure");
            _ = Assert.Throws<ResultException>(() => alpha.UnwrapError());
            Assert.Equal("emergency failure", bravo.UnwrapError());
        }

        [Fact]
        public void ExpectError()
        {
            var alpha = Result<int, string>.Ok(10);
            var error = Assert.Throws<ResultException>(() => alpha.ExceptError("Testing error"));
            Assert.Equal("Testing error", error.Message);
        }

        [Fact]
        public void UnwrapOrDefault()
        {
            var alpha = Result<int, string>.Ok(1909);
            var bravo = Result<int, string>.Error("Invalid format");
            Assert.Equal(1909, alpha.UnwrapOrDefault());
            Assert.Equal(0, bravo.UnwrapOrDefault());
        }

        [Fact]
        public void Transpose()
        {
            var alpha = Result<Option<int>, string>.Ok(Option<int>.Some(5));
            var bravo = Option<Result<int, string>>.Some(Result<int, string>.Ok(5));
            Assert.Equal(bravo, alpha.Transpose());
        }
    }
}
