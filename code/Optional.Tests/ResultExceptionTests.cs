using System;
using Xunit;

namespace Mikodev.Optional.Tests
{
    public class ResultExceptionTests
    {
        [Fact(DisplayName = "New")]
        public void New()
        {
            _ = new ResultException();
        }

        [Fact(DisplayName = "New With Message")]
        public void NewWithMessage()
        {
            var error = new ResultException("Message context");
            Assert.Equal("Message context", error.Message);
        }

        [Fact(DisplayName = "New With Inner Exception")]
        public void NewWithInnerException()
        {
            var inner = new ArgumentException();
            var error = new ResultException("Message", inner);
            Assert.Equal("Message", error.Message);
            Assert.True(ReferenceEquals(inner, error.InnerException));
        }
    }
}
