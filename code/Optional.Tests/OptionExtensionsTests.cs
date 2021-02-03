using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Mikodev.Optional.Tests
{
    public class OptionExtensionsTests
    {
        [Theory(DisplayName = "Argument Null")]
        [InlineData(nameof(OptionExtensions.Except), 1)]
        [InlineData(nameof(OptionExtensions.Except), "2")]
        [InlineData(nameof(OptionExtensions.UnwrapOrElse), 1)]
        [InlineData(nameof(OptionExtensions.UnwrapOrElse), "2")]
        [InlineData(nameof(OptionExtensions.Map), 1)]
        [InlineData(nameof(OptionExtensions.Map), "2")]
        [InlineData(nameof(OptionExtensions.OkOrElse), 1)]
        [InlineData(nameof(OptionExtensions.OkOrElse), "2")]
        [InlineData(nameof(OptionExtensions.AndThen), 1)]
        [InlineData(nameof(OptionExtensions.AndThen), "2")]
        [InlineData(nameof(OptionExtensions.OrElse), 1)]
        [InlineData(nameof(OptionExtensions.OrElse), "2")]
        public void ArgumentNull<T>(string method, T some)
        {
            var methodData = typeof(OptionExtensions).GetMethod(method);
            Assert.NotNull(methodData);
            var values = Enumerable.Range(0, methodData.GetGenericArguments().Length).Select(_ => typeof(T)).ToArray();
            var methodInfo = methodData.MakeGenericMethod(values);
            var parameters = methodInfo.GetParameters();
            Assert.Equal(2, parameters.Length);
            var parameter = parameters.Last();
            var source = Expression.Parameter(typeof(Option<T>), "source");
            var invoke = Expression.Call(methodInfo, source, Expression.Constant(null, parameter.ParameterType));
            var lambda = Expression.Lambda<Action<Option<T>>>(invoke, source);
            var action = lambda.Compile();
            var alpha = Assert.Throws<ArgumentNullException>(() => action.Invoke(Option<T>.None()));
            var bravo = Assert.Throws<ArgumentNullException>(() => action.Invoke(Option<T>.Some(some)));
            Assert.Equal(parameter.Name, alpha.ParamName);
            Assert.Equal(parameter.Name, bravo.ParamName);
        }

        public static IEnumerable<object[]> ArgumentNullData()
        {
            static object[] Make(Expression<Action> lambda, string name) => new object[] { lambda, name };
            yield return Make(() => Option<int>.None().MapOr("1", null), "func");
            yield return Make(() => Option<string>.Some("2").MapOr(3, null), "func");
            yield return Make(() => Option<int>.None().MapOrElse(null, Convert.ToString), "default");
            yield return Make(() => Option<string>.Some("4").MapOrElse(null, int.Parse), "default");
            yield return Make(() => Option<int>.None().MapOrElse(() => "5", null), "func");
            yield return Make(() => Option<string>.Some("6").MapOrElse(() => 7, null), "func");
        }

        [Theory(DisplayName = "Argument Null Advance")]
        [MemberData(nameof(ArgumentNullData))]
        public void ArgumentNullAdvance(Expression<Action> lambda, string parameterName)
        {
            var methodCall = Assert.IsAssignableFrom<MethodCallExpression>(lambda.Body);
            var methodInfo = methodCall.Method;
            var argument = methodCall.Arguments.Where(x => x is ConstantExpression { Value: null }).Single();
            var argumentIndex = methodCall.Arguments.IndexOf(argument);
            var parameter = methodInfo.GetParameters()[argumentIndex];
            var action = lambda.Compile();
            var error = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal(parameterName, parameter.Name);
            Assert.Equal(parameterName, error.ParamName);
        }

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
