using System;
using Xunit;

namespace Mikodev.Optional.Tests
{
    public class OptionExceptionTests
    {
        [Fact(DisplayName = "New")]
        public void New()
        {
            _ = new OptionException();
        }

        [Fact(DisplayName = "New With Message")]
        public void NewWithMessage()
        {
            var error = new OptionException("Message context");
            Assert.Equal("Message context", error.Message);
        }

        [Fact(DisplayName = "New With Inner Exception")]
        public void NewWithInnerException()
        {
            var inner = new ArgumentException();
            var error = new OptionException("Message", inner);
            Assert.Equal("Message", error.Message);
            Assert.True(ReferenceEquals(inner, error.InnerException));
        }
    }
}
