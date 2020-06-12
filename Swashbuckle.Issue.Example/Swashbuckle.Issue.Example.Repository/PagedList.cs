using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Swashbuckle.Issue.Example.Repository
{
    public class PagedList<T> : IPagedList<T>
    {
        /// <summary>
        ///     Creates a PagedList from an IEnumerable
        /// </summary>
        public PagedList(IEnumerable<T> items, int pageNumber, int pageSize, bool hasNextPage)
        {
            Items = new List<T>();
            Items.AddRange(items);
            PageNumber = pageNumber;
            PageSize = pageSize;
            HasNextPage = hasNextPage;
        }

        /// <summary>
        ///     Creates a PagedList from a List
        /// </summary>
        public PagedList(List<T> items, int pageNumber, int pageSize, bool hasNextPage)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            HasNextPage = hasNextPage;
        }

        public int PageNumber { get; }

        public int PageSize { get; }

        public bool HasNextPage { get; }

        public List<T> Items { get; }

        /// <summary>
        ///     Creates a PagedList object from an <see cref="System.Linq.IQueryable{T}" />.
        /// </summary>
        /// <param name="source">A queryable from which to construct the paged list.</param>
        /// <param name="pageNumber">The page number to return, starts at 1, defaults to 1.</param>
        /// <param name="pageSize">The number of records to fetch.  Defaults to 10.</param>
        /// <returns>Task{IPagedList{T}}.</returns>
        public static async Task<IPagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            int page = pageNumber <= 0 ? 1 : pageNumber;
            int size = pageSize <= 0 ? 10 : pageSize;
            IQueryable<T> items = source.Skip((page - 1) * size).Take(size + 1);
            List<T> result = await items.ToListAsync();
            if (result.Count <= size)
            {
                return new PagedList<T>(result, page, size, false);
            }

            //Remove the last item which was just to detect if there are more pages.
            result.RemoveAt(result.Count - 1);
            return new PagedList<T>(result, page, size, true);
        }

        /// <summary>
        ///     Creates a PagedList object from an <see cref="System.Linq.IQueryable{T}" />.
        /// </summary>
        /// <typeparam name="TProjection">The projection type (DTO) to which T will be mapped.</typeparam>
        /// <param name="source">A queryable from which to construct the paged list.</param>
        /// <param name="pageNumber">The page number to return, starts at 1, defaults to 1.</param>
        /// <param name="pageSize">The number of records to fetch.  Defaults to 10.</param>
        /// <param name="mapperConfiguration">A <see cref="MapperConfiguration"/> object</param>
        /// <returns>Task{IPagedList{TProjection}}.</returns>
        public static async Task<IPagedList<TProjection>> CreateAsync<TProjection>(
            IQueryable<T> source, int pageNumber, int pageSize, MapperConfiguration mapperConfiguration)
        {
            int page = pageNumber <= 0 ? 1 : pageNumber;
            int size = pageSize <= 0 ? 10 : pageSize;
            IQueryable<TProjection> items = source.Skip((page - 1) * size).Take(size + 1).ProjectTo<TProjection>(mapperConfiguration);
            List<TProjection> result = await items.ToListAsync();
            if (result.Count <= size)
            {
                return new PagedList<TProjection>(result, pageNumber, pageSize, false);
            }

            //Remove the last item which was just to detect if there are more pages.
            result.RemoveAt(result.Count - 1);
            return new PagedList<TProjection>(result, page, size, true);
        }
    }
}
