using System;
using ProcessM.Core.Services;
using Xunit;

namespace ProcessManager.Tests
{
    public class AffinityHelperTests
    {
        [Fact]
        public void BuildMask_ShouldCreateCorrectMask()
        {
            // Arrange
            bool[] cores = { true, false, true, false };
            // 1010 = 5

            // Act
            var mask = AffinityHelper.BuildMask(cores);

            // Assert
            Assert.Equal(new IntPtr(5), mask);
        }

        [Fact]
        public void IsCoreEnabled_ShouldReturnTrue_WhenBitSet()
        {
            var mask = new IntPtr(5); // 0101

            var result = AffinityHelper.IsCoreEnabled(mask, 2);

            Assert.True(result);
        }

        [Fact]
        public void IsCoreEnabled_ShouldReturnFalse_WhenBitNotSet()
        {
            var mask = new IntPtr(5);

            var result = AffinityHelper.IsCoreEnabled(mask, 1);

            Assert.False(result);
        }
    }
}