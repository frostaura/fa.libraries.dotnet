using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FrostAura.Libraries.Core.Extensions.Validation;
using Xunit;

namespace Core.Tests.Extensions.Validation
{
    internal class ValidationObject
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Surname { get; set; }
    }
    
    public class ObjectExtensionsTests
    {
        [Fact]
        public void ThrowIfNull_WhileDefined_ShouldNotThrow()
        {
            // Setup
            var obj = new object();

            // Perform action 'ThrowIfNull'
            obj.ThrowIfNull(nameof(obj));

            // Assert that 'ShouldNotThrow' = 'WhileDefined'
        }
        
        [Fact]
        public void ThrowIfNull_WhileNotDefined_ShouldThrowArgumentNullException()
        {
            // Setup
            object obj = null;

            // Perform action 'ThrowIfNull'
            var exception = Assert
                .Throws<ArgumentNullException>(() => obj.ThrowIfNull(nameof(obj)));

            // Assert that 'ShouldThrowArgumentNullException' = 'WhileNotDefined'
            Assert.NotNull(exception);
            Assert.Equal(nameof(obj), exception.ParamName);
        }
        
        [Fact]
        public void ThrowIfNullT_WhileDefined_ShouldNotThrow()
        {
            // Setup
            var obj = new object();
            
            // Perform action 'ThrowIfNullT'
            obj = obj.ThrowIfNull<object>(nameof(obj));
            
            // Assert that 'ShouldNotThrow' = 'WhileDefined'
        }
        
        [Fact]
        public void ThrowIfNullT_WhileNotDefined_ShouldThrowArgumentNullException()
        {
            // Setup
            object obj = null;
            
            // Perform action 'ThrowIfNullT'
            var exception = Assert
                .Throws<ArgumentNullException>(() => obj = obj.ThrowIfNull<object>(nameof(obj)));
            
            // Assert that 'ShouldThrowArgumentNullException' = 'WhileNotDefined'
            Assert.NotNull(exception);
            Assert.Equal(nameof(obj), exception.ParamName);
        }
        
        [Fact]
        public void IsValid_WithValidObject_ShouldReturnTrue()
        {
            // Setup
            var validationObject = new object();
            var validationErrors = new List<ValidationResult>();

            // Perform action 'IsValid'
            var isValid = validationObject.IsValid(out validationErrors);

            // Assert that 'ShouldReturnTrue' = 'WithValidObject'
            Assert.True(isValid);
            Assert.Empty(validationErrors);
        }
        
        [Fact]
        public void IsValid_WithOneInvalidProperty_ShouldReturnFalseWithOneValidationError()
        {
            // Setup
            var validationObject = new ValidationObject { Name = "Test User"};
            var validationErrors = new List<ValidationResult>();
            
            // Perform action 'IsValid'
            var isValid = validationObject.IsValid(out validationErrors);
            
            // Assert that 'ShouldReturnFalseWithOneValidationError' = 'WithOneInvalidProperty'
            Assert.False(isValid);
            Assert.Equal(1, validationErrors.Count);
        }
        
        [Fact]
        public void IsValid_WithTwoInvalidProperties_ShouldReturnFalseWithTwoValidationErrors()
        {
            // Setup
            var validationObject = new ValidationObject();
            var validationErrors = new List<ValidationResult>();
            
            // Perform action 'IsValid'
            var isValid = validationObject.IsValid(out validationErrors);
            
            // Assert that 'ShouldReturnFalseWithTwoValidationErrors' = 'WithTwoInvalidProperties'
            Assert.False(isValid);
            Assert.Equal(2, validationErrors.Count);
        }
        
        [Fact]
        public void ThrowIfInvalid_WithValidationErrors_ShouldThrowValidationException()
        {
            // Setup
            var objectToValidate = new ValidationObject
            {
                Name = "Hello"
            };
            
            // Perform action 'ThrowIfInvalid'
            Assert.Throws<FrostAura.Libraries.Core.Exceptions.Validation.ValidationException>(() =>
            {
                objectToValidate
                    .ThrowIfInvalid(nameof(objectToValidate));
            });

            // Assert that 'ShouldThrowValidationException' = 'WithValidationErrors'
        }
        
        [Fact]
        public void ThrowIfInvalid_WithNoValidationErrors_ShouldNotThrow()
        {
            // Setup
            var objectToValidate = new ValidationObject
            {
                Name = "Hello",
                Surname = "World"
            };

            // Perform action 'ThrowIfInvalid'
            objectToValidate
                .ThrowIfInvalid(nameof(objectToValidate));

            // Assert that 'ShouldNotThrow' = 'WithNoValidationErrors'
        }
    }
}