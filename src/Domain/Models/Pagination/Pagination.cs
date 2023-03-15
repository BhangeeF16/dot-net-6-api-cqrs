using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Domain.Models.Pagination
{
    public partial class Pagination
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? SortingCol { get; set; }
        public string? SortDirection { get; set; }
        public string? Keyword { get; set; }

        public static ValueTask<Pagination?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            const string pageSizeKey = "pageSize";
            const string sortingColKey = "sortingCol";
            const string sortTypeKey = "sortDirection";
            const string pageNumberKey = "pageNumber";
            const string keywordKey = "keyword";

            int.TryParse(context.Request.Query[pageNumberKey], out var pageNumber);
            int.TryParse(context.Request.Query[pageSizeKey], out var pageSize);

            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;

            var result = new Pagination
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortingCol = context.Request.Query[sortingColKey],
                SortDirection = context.Request.Query[sortTypeKey],
                Keyword = context.Request.Query[keywordKey]
            };

            return ValueTask.FromResult<Pagination?>(result);
        }
    }

}

