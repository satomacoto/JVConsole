using System;
using Xunit;
using FluentAssertions;
using JVParquet.TypeMapping;

namespace JVParquet.Tests.TypeMapping
{
    public class TypeMappingManagerTests
    {
        [Fact]
        public void GetFieldType_WithSERecord_ShouldReturnCorrectTypes()
        {
            // Arrange
            var manager = TypeMappingManager.Instance;
            
            // Act & Assert
            manager.GetFieldType("SE", "Umaban").Should().Be(typeof(int));
            manager.GetFieldType("SE", "Bamei").Should().Be(typeof(string));
            manager.GetFieldType("SE", "Odds").Should().Be(typeof(int));
            manager.GetFieldType("SE", "Honsyokin").Should().Be(typeof(int));
        }

        [Theory]
        [InlineData("ChakuKaisu", typeof(int))]
        [InlineData("ChakuKaisu_0", typeof(int))]
        [InlineData("ChakuKaisu_10", typeof(int))]
        [InlineData("Syokin", typeof(int))]
        [InlineData("HonSyokin", typeof(int))]
        [InlineData("Odds", typeof(int))]
        [InlineData("Bamei", typeof(string))]
        public void InferTypeFromFieldName_ShouldReturnCorrectType(string fieldName, Type expectedType)
        {
            // Act
            var result = RecordTypeMappingBase.InferTypeFromFieldName(fieldName);
            
            // Assert
            result.Should().Be(expectedType);
        }

        [Fact]
        public void GetMapping_WithValidRecordSpec_ShouldReturnMapping()
        {
            // Arrange
            var manager = TypeMappingManager.Instance;
            
            // Act
            var mapping = manager.GetMapping("SE");
            
            // Assert
            mapping.Should().NotBeNull();
            mapping.RecordSpec.Should().Be("SE");
            mapping.IndexColumns.Should().Contain("Umaban");
        }

        [Fact]
        public void GetMapping_WithInvalidRecordSpec_ShouldReturnNull()
        {
            // Arrange
            var manager = TypeMappingManager.Instance;
            
            // Act
            var mapping = manager.GetMapping("XX");
            
            // Assert
            mapping.Should().BeNull();
        }
    }
}