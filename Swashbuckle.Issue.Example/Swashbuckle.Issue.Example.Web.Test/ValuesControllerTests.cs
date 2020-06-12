using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using Swashbuckle.Issue.Example.Repository;
using Swashbuckle.Issue.Example.Service;
using Swashbuckle.Issue.Example.Web.Controllers;
using Xunit;

namespace Swashbuckle.Issue.Example.Web.Test
{
    public class ValuesControllerTests
    {
        private readonly ValuesController _valuesController;

        private readonly Mock<IValueService> _valuesService = new Mock<IValueService>();

        public ValuesControllerTests()
        {
            _valuesController = new ValuesController(_valuesService.Object);
        }

        [Fact]
        public async void Delete_ShouldCallServiceDeleteWithId()
        {
            // Arrange

            // Act
            await _valuesController.Delete(1);

            // Assert
            _valuesService.Verify(s => s.Delete(1));
            _valuesService.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Get_ShouldReturnExpectedValues()
        {
            // Arrange
            IEnumerable<ValueModel> expected = new[] { new ValueModel { Id = 1, Value = "theValue" } };

            _valuesService.Setup(s => s.GetAllValues()).ReturnsAsync(expected);

            // Act
            IEnumerable<ValueModel> actual = await _valuesController.Get();

            // Assert
            actual.Should().BeEquivalentTo(expected);
            VerifyAll();
        }

        [Fact]
        public async void Post_WhenValidValueProvided_ShouldCreateAndReturnUpdatedValueModel()
        {
            // Arrange
            ValueModel valueModel = new ValueModel { Value = "testValue" };
            ValueModel expected = new ValueModel { Id = 1, Value = "testValue" };
            _valuesService.Setup(s => s.Create(valueModel)).ReturnsAsync(expected);

            // Per Microsoft, do not test model validation as part of controller tests.
            // https://github.com/aspnet/Mvc/issues/8640#issuecomment-432745861
            // Test model validation separately see https://fluentvalidation.net/testing
            var objectModelValidatorMock = new MockObjectModelValidator();
            _valuesController.ObjectValidator = objectModelValidatorMock.Object;

            // Act
            ValueModel actual = await _valuesController.Post(valueModel);

            // Assert
            // Note that the test will succeed without this assertion.  It is incumbent upon the developer
            // to use proper assertions.
            objectModelValidatorMock.VerifyValidatorCalled(valueModel);
            actual.Should().BeEquivalentTo(expected);
            VerifyAll();
        }

        private void VerifyAll()
        {
            _valuesService.VerifyAll();
        }
    }
}
