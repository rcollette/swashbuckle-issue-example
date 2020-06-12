using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Swashbuckle.Issue.Example.Repository.Test
{
    public class ValueModelValidatorTests
    {
        private readonly ValueModelValidator _validator;

        public ValueModelValidatorTests()
        {
            _validator = new ValueModelValidator();
        }

        [Fact]
        public void WhenValueIsNull_ShouldError()
        {
            var results = _validator.ShouldHaveValidationErrorFor(value => value.Value, null as string).ToArray();
            results[0].ErrorMessage.Should().BeEquivalentTo("'Value' must not be empty.");
        }

        [Fact]
        public void WhenValueIsEmpty_ShouldError()
        {
            var results = _validator.ShouldHaveValidationErrorFor(value => value.Value, string.Empty).ToArray();
            results[0].ErrorMessage.Should().BeEquivalentTo("'Value' is not a valid email address.");
        }

        [Fact]
        public void WhenValueNotAnEmail_ShouldError()
        {
            var results = _validator.ShouldHaveValidationErrorFor(value => value.Value, "hello").ToArray();
            results[0].ErrorMessage.Should().BeEquivalentTo("'Value' is not a valid email address.");
        }

        [Fact]
        public void WhenValueLessThan6Characters_ShouldError()
        {
            var valueModel = new ValueModel { Value = "a@b.x" };
            var results = _validator.ShouldHaveValidationErrorFor(value => value.Value, valueModel).ToArray();
            results[0].ErrorMessage.Should()
                .BeEquivalentTo("The length of 'Value' must be at least 6 characters. You entered 5 characters.");
        }

        [Fact]
        public void WhenValueMoreThan10Characters_ShouldError()
        {
            var results = _validator.ShouldHaveValidationErrorFor(value => value.Value, "1234567890@b.x").ToArray();
            results[0].ErrorMessage.Should()
                .BeEquivalentTo("The length of 'Value' must be 10 characters or fewer. You entered 14 characters.");
        }
    }
}
