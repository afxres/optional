using Xunit;

namespace Mikodev.Optional.Tests
{
    public class UnitTests
    {
        [Fact(DisplayName = "Unit")]
        public void Unit()
        {
            var unit = new Unit();
            Assert.True(unit.Equals(new Unit()));
            Assert.False(unit.Equals(new object()));
            Assert.Equal(0, unit.GetHashCode());
            Assert.Equal("()", unit.ToString());
        }
    }
}
