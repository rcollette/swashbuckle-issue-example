using System;

namespace Swashbuckle.Issue.Example.Repository
{
    public class UniqueConstraintException : Exception
    {
        /// <summary>
        ///     An exception to normalize database constraint errors to a simple exception
        /// </summary>
        /// <remarks>
        ///     Use of UniqueConstraintException is helpful for mapping to HTTP 409 Conflict responses.
        /// </remarks>
        /// <example>
        ///     <code>
        /// try
        /// {
        ///   await _context.SaveChangesAsync();
        /// }
        /// catch (DbUpdateException ex) when (ex.InnerException is MySqlException ix &amp;&amp; ix.Number == 1062)
        /// {
        ///   //Cannot unit test because in memory database will not throw provider specific errors.
        ///   throw new UniqueConstraintException($"Cannot create some thing. {ex.InnerException.Message}", ex);
        /// }
        /// </code>
        /// </example>
        public UniqueConstraintException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
