using System;
using System.Collections.Generic;

namespace Services.ViewModels
{
    public class CustomerSearchResultPageViewModel
    {
        public List<CustomerSearchResultViewModel> Customers { get; set; } = new();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; } = 50;

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
