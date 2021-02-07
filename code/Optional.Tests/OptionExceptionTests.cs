using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

        [Fact]
        public void Serialization()
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            var source = new OptionException("Message", new ArgumentException("Some error"));
            formatter.Serialize(stream, source);
            var result = Assert.IsType<OptionException>(formatter.Deserialize(new MemoryStream(stream.ToArray())));
            Assert.Equal("Message", result.Message);
            var target = Assert.IsType<ArgumentException>(result.InnerException);
            Assert.Equal("Some error", target.Message);
        }
    }
}
