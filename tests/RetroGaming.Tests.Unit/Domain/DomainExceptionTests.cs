using FluentAssertions;
using RetroGaming.Domain.Exceptions;

namespace RetroGaming.Tests.Unit.Domain;

[TestFixture]
public class DomainExceptionTests
{
    [Test]
    public void EntityNotFoundException_WithEntityAndKey_ShouldSetCorrectMessage()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var exception = new EntityNotFoundException("Manufacturer", id);

        // Assert
        exception.Message.Should().Be($"Entity 'Manufacturer' with key '{id}' was not found.");
    }

    [Test]
    public void DomainExceptions_WithMessageAndInnerException_ShouldRetainValues()
    {
        // Arrange
        var inner = new InvalidOperationException("Inner error");

        // Act
        var validationEx = new DomainValidationException("Validation error", inner);
        var invalidOpEx = new InvalidDomainOperationException("Operation error", inner);
        var notFoundEx = new EntityNotFoundException("Not found error");

        // Assert
        validationEx.Message.Should().Be("Validation error");
        validationEx.InnerException.Should().Be(inner);

        invalidOpEx.Message.Should().Be("Operation error");
        invalidOpEx.InnerException.Should().Be(inner);

        notFoundEx.Message.Should().Be("Not found error");
    }
}
