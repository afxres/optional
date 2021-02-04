using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Mikodev.Optional.Tests
{
    public class ArgumentNullTests
    {
        public static IEnumerable<object[]> ArgumentNullData()
        {
            static object[] Make(Expression<Action> lambda, string name) => new object[] { lambda, name };
            yield return Make(() => Option<int>.None().MapOr("1", null), "func");
            yield return Make(() => Option<int>.None().MapOrElse(null, Convert.ToString), "default");
            yield return Make(() => Option<int>.None().MapOrElse(() => "2", null), "func");
            yield return Make(() => Option<string>.Some("3").MapOr(4, null), "func");
            yield return Make(() => Option<string>.Some("5").MapOrElse(null, int.Parse), "default");
            yield return Make(() => Option<string>.Some("6").MapOrElse(() => 7, null), "func");

            yield return Make(() => Option<int>.None().Except(null), "message");
            yield return Make(() => Option<int>.None().UnwrapOrElse(null), "func");
            yield return Make(() => Option<int>.None().Map<int, string>(null), "func");
            yield return Make(() => Option<int>.None().OkOrElse<int, string>(null), "func");
            yield return Make(() => Option<int>.None().AndThen<int, string>(null), "func");
            yield return Make(() => Option<int>.None().OrElse(null), "func");
            yield return Make(() => Option<int>.Some(1).Except(null), "message");
            yield return Make(() => Option<int>.Some(1).UnwrapOrElse(null), "func");
            yield return Make(() => Option<int>.Some(1).Map<int, string>(null), "func");
            yield return Make(() => Option<int>.Some(1).OkOrElse<int, string>(null), "func");
            yield return Make(() => Option<int>.Some(1).AndThen<int, string>(null), "func");
            yield return Make(() => Option<int>.Some(1).OrElse(null), "func");

            yield return Make(() => Result<int, string>.Ok(1).Map<int, double, string>(null), "func");
            yield return Make(() => Result<int, string>.Ok(1).MapOrElse(null, Convert.ToDouble), "fallback");
            yield return Make(() => Result<int, string>.Ok(1).MapOrElse(Convert.ToDouble, null), "func");
            yield return Make(() => Result<int, string>.Ok(1).MapError<int, string, double>(null), "func");
            yield return Make(() => Result<int, string>.Ok(1).AndThen<int, double, string>(null), "func");
            yield return Make(() => Result<int, string>.Ok(1).OrElse<int, string, double>(null), "func");
            yield return Make(() => Result<int, string>.Ok(1).UnwrapOrElse(null), "func");
            yield return Make(() => Result<int, string>.Ok(1).Except(null), "message");
            yield return Make(() => Result<int, string>.Ok(1).ExceptError(null), "message");
            yield return Make(() => Result<int, string>.Error("2").Map<int, double, string>(null), "func");
            yield return Make(() => Result<int, string>.Error("2").MapOrElse(null, Convert.ToDouble), "fallback");
            yield return Make(() => Result<int, string>.Error("2").MapOrElse(Convert.ToDouble, null), "func");
            yield return Make(() => Result<int, string>.Error("2").MapError<int, string, double>(null), "func");
            yield return Make(() => Result<int, string>.Error("2").AndThen<int, double, string>(null), "func");
            yield return Make(() => Result<int, string>.Error("2").OrElse<int, string, double>(null), "func");
            yield return Make(() => Result<int, string>.Error("2").UnwrapOrElse(null), "func");
            yield return Make(() => Result<int, string>.Error("2").Except(null), "message");
            yield return Make(() => Result<int, string>.Error("2").ExceptError(null), "message");
        }

        [Theory(DisplayName = "Argument Null")]
        [MemberData(nameof(ArgumentNullData))]
        public void ArgumentNull(Expression<Action> lambda, string parameterName)
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
    }
}
