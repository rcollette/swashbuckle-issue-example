using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;

namespace Swashbuckle.Issue.Example.Web.Test
{
    public class MockObjectModelValidator : Mock<IObjectModelValidator>
    {
        public void VerifyValidatorCalled<T>(T model)
        {
            Verify(s => s.Validate(
                It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsIn(model)));
        }
    }
}
