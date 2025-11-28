using CoLearn.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Extensions
{
    public static class PagedResultExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex <= 0 ? PagedResult<T>.DefaultPageIndex : pageIndex;
            pageSize = pageSize <= 0
                ? PagedResult<T>.DefaultPageSize
                : pageSize > PagedResult<T>.UpperPageSize
                    ? PagedResult<T>.UpperPageSize
                    : pageSize;

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResult<T>(items, pageIndex, pageSize, totalCount);
        }
    }
}
