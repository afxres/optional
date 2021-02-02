using System;
using Xunit;

namespace Mikodev.Optional.Tests
{
    public class OptionExtensionsTests
    {
        [Fact]
        public void IsSome()
        {
            var alpha = Option<int>.Some(2);
            var bravo = Option<int>.None();
            Assert.True(alpha.IsSome());
            Assert.False(bravo.IsSome());
        }

        [Fact]
        public void IsNone()
        {
            var alpha = Option<int>.Some(2);
            var bravo = Option<int>.None();
            Assert.False(alpha.IsNone());
            Assert.True(bravo.IsNone());
        }

        [Fact]
        public void Expect()
        {
            var message = "The world is ending";
            var alpha = Option<string>.Some("value");
            var bravo = Option<string>.None();
            Assert.Equal("value", alpha.Except(message));
            var error = Assert.Throws<OptionException>(() => bravo.Except(message));
            Assert.Equal(message, error.Message);
        }

        [Fact]
        public void Unwrap()
        {
            var alpha = Option<string>.Some("air");
            var bravo = Option<string>.None();
            Assert.Equal("air", alpha.Unwrap());
            var error = Assert.Throws<OptionException>(() => bravo.Unwrap());
        }

        [Fact]
        public void UnwrapOr()
        {
            Assert.Equal("car", Option<string>.Some("car").UnwrapOr("bike"));
            Assert.Equal("bike", Option<string>.None().UnwrapOr("bike"));
        }

        [Fact]
        public void UnwrapOrElse()
        {
            var k = 10;
            Assert.Equal(4, Option<int>.Some(4).UnwrapOrElse(() => 2 * k));
            Assert.Equal(20, Option<int>.None().UnwrapOrElse(() => 2 * k));
        }

        [Fact]
        public void Map()
        {
            var f = new Func<string, int>(x => x.Length);
            Assert.Equal(Option<int>.None(), Option<string>.None().Map(f));
            Assert.Equal(Option<int>.Some(13), Option<string>.Some("Hello, world!").Map(f));
        }

        [Fact]
        public void MapOr()
        {
            var alpha = Option<string>.Some("foo");
            var bravo = Option<string>.None();
            Assert.Equal(3, alpha.MapOr(42, x => x.Length));
            Assert.Equal(42, bravo.MapOr(42, x => x.Length));
        }

        [Fact]
        public void MapOrElse()
        {
            var k = 21;
            var alpha = Option<string>.Some("foo");
            var bravo = Option<string>.None();
            Assert.Equal(3, alpha.MapOrElse(() => 2 * k, x => x.Length));
            Assert.Equal(42, bravo.MapOrElse(() => 2 * k, x => x.Length));
        }

        [Fact]
        public void OkOr()
        {
            var alpha = Option<string>.Some("foo");
            var bravo = Option<string>.None();
            Assert.Equal(Result<string, int>.Ok("foo"), alpha.OkOr(0));
            Assert.Equal(Result<string, int>.Error(0), bravo.OkOr(0));
        }

        [Fact]
        public void OkOrElse()
        {
            var alpha = Option<string>.Some("foo");
            var bravo = Option<string>.None();
            Assert.Equal(Result<string, int>.Ok("foo"), alpha.OkOrElse(() => 0));
            Assert.Equal(Result<string, int>.Error(0), bravo.OkOrElse(() => 0));
        }

        [Fact]
        public void And()
        {
            var a = Option<int>.Some(2);
            var b = Option<string>.None();
            Assert.Equal(Option<string>.None(), a.And(b));

            var e = Option<uint>.None();
            var f = Option<string>.Some("foo");
            Assert.Equal(Option<string>.None(), e.And(f));

            var m = Option<int>.Some(2);
            var n = Option<string>.Some("foo");
            Assert.Equal(Option<string>.Some("foo"), m.And(n));

            var x = Option<uint>.None();
            var y = Option<string>.None();
            Assert.Equal(Option<string>.None(), x.And(y));
        }

        [Fact]
        public void AndThen()
        {
            Option<uint> Ok(uint x) => Option<uint>.Some(x * x);
            Option<uint> No(uint _) => Option<uint>.None();

            Assert.Equal(Option<uint>.Some(16), Option<uint>.Some(2).AndThen(Ok).AndThen(Ok));
            Assert.Equal(Option<uint>.None(), Option<uint>.Some(2).AndThen(Ok).AndThen(No));
            Assert.Equal(Option<uint>.None(), Option<uint>.Some(2).AndThen(No).AndThen(Ok));
            Assert.Equal(Option<uint>.None(), Option<uint>.None().AndThen(Ok).AndThen(Ok));
        }

        [Fact]
        public void Or()
        {
            var a = Option<int>.Some(2);
            var b = Option<int>.None();
            Assert.Equal(Option<int>.Some(2), a.Or(b));

            var e = Option<int>.None();
            var f = Option<int>.Some(100);
            Assert.Equal(Option<int>.Some(100), e.Or(f));

            var m = Option<int>.Some(2);
            var n = Option<int>.Some(100);
            Assert.Equal(Option<int>.Some(2), m.Or(n));

            var x = Option<uint>.None();
            var y = Option<uint>.None();
            Assert.Equal(Option<uint>.None(), x.Or(y));
        }

        [Fact]
        public void OrElse()
        {
            Option<string> Nobody() => Option<string>.None();
            Option<string> Vikings() => Option<string>.Some("vikings");
            Assert.Equal(Option<string>.Some("barbarians"), Option<string>.Some("barbarians").OrElse(Vikings));
            Assert.Equal(Option<string>.Some("vikings"), Option<string>.None().OrElse(Vikings));
            Assert.Equal(Option<string>.None(), Option<string>.None().OrElse(Nobody));
        }

        [Fact]
        public void Xor()
        {
            var a = Option<int>.Some(2);
            var b = Option<int>.None();
            Assert.Equal(Option<int>.Some(2), a.Xor(b));

            var e = Option<int>.None();
            var f = Option<int>.Some(2);
            Assert.Equal(Option<int>.Some(2), e.Xor(f));

            var m = Option<int>.Some(2);
            var n = Option<int>.Some(2);
            Assert.Equal(Option<int>.None(), m.Xor(n));

            var x = Option<int>.None();
            var y = Option<int>.None();
            Assert.Equal(Option<int>.None(), x.Xor(y));
        }

        [Fact]
        public void UnwrapOrDefault()
        {
            var alpha = Option<int>.Some(1909);
            var bravo = Option<int>.None();
            Assert.Equal(1909, alpha.UnwrapOrDefault());
            Assert.Equal(0, bravo.UnwrapOrDefault());
        }

        [Fact]
        public void Transpose()
        {
            var a = Result<Option<int>, string>.Ok(Option<int>.Some(5));
            var b = Option<Result<int, string>>.Some(Result<int, string>.Ok(5));
            Assert.Equal(a, b.Transpose());

            var m = Result<Option<int>, string>.Error("error");
            var n = Option<Result<int, string>>.Some(Result<int, string>.Error("error"));
            Assert.Equal(m, n.Transpose());

            var x = Result<Option<int>, string>.Ok(Option<int>.None());
            var y = Option<Result<int, string>>.None();
            Assert.Equal(x, y.Transpose());
        }

        [Fact]
        public void Flatten()
        {
            var alpha = Option<Option<int>>.Some(Option<int>.Some(6));
            var bravo = Option<Option<int>>.Some(Option<int>.None());
            var delta = Option<Option<int>>.None();
            Assert.Equal(Option<int>.Some(6), alpha.Flatten());
            Assert.Equal(Option<int>.None(), bravo.Flatten());
            Assert.Equal(Option<int>.None(), delta.Flatten());
        }
    }
}
