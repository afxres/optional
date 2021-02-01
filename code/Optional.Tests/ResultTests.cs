using System;
using System.Collections.Generic;
using Xunit;

namespace Mikodev.Optional.Tests
{
    public class ResultTests
    {
        private const string ErrorMessage = "Can not operate on default value of result!";

        [Theory(DisplayName = "New Ok & To String")]
        [InlineData(2, "two")]
        [InlineData("zero", 0)]
        public void NewOk<T, E>(T ok, E error)
        {
            var result = Result<T, E>.Ok(ok);
            var target = result.ToString();
            Assert.Equal($"Ok({ok})", target);
        }

        [Theory(DisplayName = "New Error & To String")]
        [InlineData(3, "three")]
        [InlineData("ok", -1)]
        public void NewError<T, E>(T ok, E error)
        {
            var result = Result<T, E>.Error(error);
            var target = result.ToString();
            Assert.Equal($"Error({error})", target);
        }

        [Theory(DisplayName = "Call 'Equals' & 'GetHashCode' On Default Value")]
        [InlineData(0, "default")]
        [InlineData("true", false)]
        [InlineData(1.1F, 2.3)]
        public void InvalidEquals<T, E>(T ok, E error)
        {
            var alpha = Assert.Throws<InvalidOperationException>(() => default(Result<T, E>).Equals(new object()));
            var bravo = Assert.Throws<InvalidOperationException>(() => default(Result<T, E>).Equals(Result<T, E>.Ok(ok)));
            var delta = Assert.Throws<InvalidOperationException>(() => Result<T, E>.Error(error).Equals(default));
            var hotel = Assert.Throws<InvalidOperationException>(() => default(Result<T, E>).GetHashCode());
            Assert.Equal(ErrorMessage, alpha.Message);
            Assert.Equal(ErrorMessage, bravo.Message);
            Assert.Equal(ErrorMessage, delta.Message);
            Assert.Equal(ErrorMessage, hotel.Message);
        }

        [Theory(DisplayName = "Cast With Default Value")]
        [InlineData(true, 1)]
        [InlineData(0, "false")]
        public void InvalidCast<T, E>(T ok, E error)
        {
            var alpha = Assert.Throws<InvalidOperationException>(() => { Result<T, E> result = default(Result<Unit, E>); });
            var bravo = Assert.Throws<InvalidOperationException>(() => { Result<T, E> result = default(Result<T, Unit>); });
            Assert.Equal(ErrorMessage, alpha.Message);
            Assert.Equal(ErrorMessage, bravo.Message);
        }

        [Theory(DisplayName = "Cast With Unit")]
        [InlineData(true, 1)]
        [InlineData(0, "false")]
        public void InvalidCastOfUnit<T, E>(T ok, E error)
        {
            var alpha = Assert.Throws<InvalidCastException>(() => { Result<T, E> result = Result<Unit, E>.Ok(new Unit()); });
            var bravo = Assert.Throws<InvalidCastException>(() => { Result<T, E> result = Result<T, Unit>.Error(new Unit()); });
            var delta = $"Can not convert 'Ok<Unit>' to 'Ok<{typeof(T).Name}>'";
            var hotel = $"Can not convert 'Error<Unit>' to 'Error<{typeof(E).Name}>'";
            Assert.Equal(delta, alpha.Message);
            Assert.Equal(hotel, bravo.Message);
        }

        public static IEnumerable<object[]> EqualData => new object[][]
        {
            new object[] { Result<int, string>.Ok(1024), Result<int, string>.Ok(1024), },
            new object[] { Result<string, int>.Ok("ok"), Result<string, int>.Ok("ok"), },
            new object[] { Result<float, double>.Error(-1.2), Result<float, double>.Error(-1.2) },
        };

        [Theory(DisplayName = "Equal")]
        [MemberData(nameof(EqualData))]
        public void Equal<T, E>(Result<T, E> source, Result<T, E> target)
        {
            Assert.True(source.Equals((object)target));
            Assert.True(target.Equals((object)source));
            Assert.True(source.Equals(target));
            Assert.True(target.Equals(source));
            Assert.True(source == target);
            Assert.True(target == source);
            Assert.False(source != target);
            Assert.False(target != source);

            Assert.Equal(source.GetHashCode(), target.GetHashCode());
        }

        public static IEnumerable<object[]> NotEqualData => new object[][]
        {
            new object[] { Result<int, string>.Ok(1024), Result<int, string>.Ok(2048), },
            new object[] { Result<string, int>.Ok("ok"), Result<string, int>.Ok("error"), },
            new object[] { Result<float, double>.Ok(-1.2F), Result<float, double>.Error(-1.2) },
        };

        [Theory(DisplayName = "Not Equal")]
        [MemberData(nameof(NotEqualData))]
        public void NotEqual<T, E>(Result<T, E> source, Result<T, E> target)
        {
            Assert.False(source.Equals((object)target));
            Assert.False(target.Equals((object)source));
            Assert.False(source.Equals(target));
            Assert.False(target.Equals(source));
            Assert.False(source == target);
            Assert.False(target == source);
            Assert.True(source != target);
            Assert.True(target != source);
        }

        [Theory(DisplayName = "Cast With Ok")]
        [InlineData(1, 2.3F)]
        [InlineData("one", 2.3)]
        public void ImplicitCastWithOk<TOk, TError>(TOk ok, TError error)
        {
            Result<TOk, TError> target = Result<TOk, Unit>.Ok(ok);
            Assert.Equal(Result<TOk, TError>.Ok(ok), target);
        }

        [Theory(DisplayName = "Cast With Error")]
        [InlineData(1, 2.3F)]
        [InlineData(1.2, "three")]
        public void ImplicitCastWithError<TOk, TError>(TOk ok, TError error)
        {
            Result<TOk, TError> target = Result<Unit, TError>.Error(error);
            Assert.Equal(Result<TOk, TError>.Error(error), target);
        }
    }
}
