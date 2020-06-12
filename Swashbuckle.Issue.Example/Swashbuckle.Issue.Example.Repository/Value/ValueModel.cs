using System.ComponentModel.DataAnnotations;

namespace Swashbuckle.Issue.Example.Repository
{
    //TODO - Remove me.   This is only an example.
    public class ValueModel
    {
        public int Id { get; set; }

        // Note that validation of FluentValidation is more easily tested than using DataAnnotation attributes.
        public string? Value { get; set; }
    }
}
