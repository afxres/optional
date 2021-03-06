﻿using System;
using Xunit;

namespace Mikodev.Optional.Tests
{
    public class OptionTests
    {
        private const string ErrorMessage = "Can not operate on default value of option!";

        [Theory(DisplayName = "Default & To String")]
        [InlineData(0)]
        [InlineData("empty")]
        public void Default<T>(T data)
        {
            var option = new Option<T>();
            var result = option.ToString();
            Assert.Equal("Option()", result);
        }

        [Theory(DisplayName = "New None & To String")]
        [InlineData(0)]
        [InlineData("empty")]
        public void NewNone<T>(T data)
        {
            var option = Option<T>.None();
            var result = option.ToString();
            Assert.Equal("None()", result);
        }

        [Theory(DisplayName = "New Some & To String")]
        [InlineData(-1)]
        [InlineData("some data")]
        public void NewSome<T>(T data)
        {
            var option = Option<T>.Some(data);
            var result = option.ToString();
            Assert.Equal($"Some({data})", result);
        }

        [Theory(DisplayName = "Call 'Equals' & 'GetHashCode' On Default Value")]
        [InlineData(1024)]
        [InlineData("quick")]
        public void InvalidEquals<T>(T data)
        {
            var alpha = Assert.Throws<InvalidOperationException>(() => default(Option<T>).Equals(new object()));
            var bravo = Assert.Throws<InvalidOperationException>(() => default(Option<T>).Equals(Option<T>.Some(data)));
            var delta = Assert.Throws<InvalidOperationException>(() => Option<T>.Some(data).Equals(default));
            var hotel = Assert.Throws<InvalidOperationException>(() => default(Option<T>).GetHashCode());
            Assert.Equal(ErrorMessage, alpha.Message);
            Assert.Equal(ErrorMessage, bravo.Message);
            Assert.Equal(ErrorMessage, delta.Message);
            Assert.Equal(ErrorMessage, hotel.Message);
        }

        [Theory(DisplayName = "Cast With Default Value")]
        [InlineData(512)]
        [InlineData("key")]
        public void InvalidCast<T>(T data)
        {
            var error = Assert.Throws<InvalidOperationException>(() => { Option<T> option = default(Option<Unit>); });
            Assert.Equal(ErrorMessage, error.Message);
        }

        [Theory(DisplayName = "Cast With Unit")]
        [InlineData(512)]
        [InlineData("key")]
        public void InvalidCastOfUnit<T>(T data)
        {
            var error = Assert.Throws<InvalidCastException>(() => { Option<T> option = Option<Unit>.Some(new Unit()); });
            var message = $"Can not convert 'Some<Unit>' to 'Some<{typeof(T).Name}>'";
            Assert.Equal(message, error.Message);
        }

        [Theory(DisplayName = "Equal")]
        [InlineData(2)]
        [InlineData("two")]
        public void Equal<T>(T data)
        {
            var source = Option<T>.Some(data);
            var target = Option<T>.Some(data);
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

        [Theory(DisplayName = "Not Equal")]
        [InlineData(3, 4)]
        [InlineData("left", "right")]
        public void NotEqual<T>(T left, T right)
        {
            var source = Option<T>.Some(left);
            var target = Option<T>.Some(right);
            Assert.False(source.Equals((object)target));
            Assert.False(target.Equals((object)source));
            Assert.False(source.Equals(target));
            Assert.False(target.Equals(source));
            Assert.False(source.Equals(Option<T>.None()));
            Assert.False(target.Equals(Option<T>.None()));
            Assert.False(source == target);
            Assert.False(target == source);
            Assert.True(source != target);
            Assert.True(target != source);
        }

        [Theory(DisplayName = "Not Equal Type Mismatch")]
        [InlineData(1, 2.0)]
        [InlineData(3, "4")]
        public void NotEqualTypeMismatch<T, R>(T some, R data)
        {
            Assert.False(Option<T>.None().Equals(Option<R>.None()));
            Assert.False(Option<T>.Some(some).Equals(Option<R>.Some(data)));
            Assert.False(Option<T>.None().Equals(data));
            Assert.False(Option<T>.Some(some).Equals(data));
        }

        [Fact(DisplayName = "Equality With Nested Option Or Result")]
        public void EqualityWithNestedOptionOrResult()
        {
            var a = Option<Result<int, string>>.None();
            var b = Option<Result<int, string>>.None();
            var c = Option<Result<int, string>>.Some(Result<int, string>.Ok(1));
            var d = c;
            Assert.True(a.Equals(b));
            Assert.True(a == b);
            Assert.True(a != c);
            Assert.False(a.Equals(c));
            Assert.False(a == c);
            Assert.False(a != b);

            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.Equal(c.GetHashCode(), d.GetHashCode());
        }

        [Theory(DisplayName = "Cast With None")]
        [InlineData(2)]
        [InlineData("data")]
        public void ImplicitCast<T>(T data)
        {
            Option<T> target = Option<Unit>.None();
            Assert.Equal(Option<T>.None(), target);
        }
    }
}
