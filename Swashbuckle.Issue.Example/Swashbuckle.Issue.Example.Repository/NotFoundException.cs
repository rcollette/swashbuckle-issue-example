using System;

namespace Swashbuckle.Issue.Example.Repository
{
    /// <summary>
    ///     An Exception to be thrown when an operation requires an existing record
    /// </summary>
    /// <example>
    ///     When deleting a parent and its children but the parent is not found.
    /// </example>
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
