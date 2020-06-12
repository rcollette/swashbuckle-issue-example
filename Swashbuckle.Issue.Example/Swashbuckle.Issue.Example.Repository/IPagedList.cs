using System.Collections.Generic;

namespace Swashbuckle.Issue.Example.Repository
{
    public interface IPagedList<T>
    {
        int PageNumber { get; }

        int PageSize { get; }

        bool HasNextPage { get; }

        List<T> Items { get; }
    }
}
