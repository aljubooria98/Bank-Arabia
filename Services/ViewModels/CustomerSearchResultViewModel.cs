﻿namespace Services.ViewModels
{
    public class CustomerSearchResultViewModel
    {
        public int CustomerId { get; set; }

        public string PersonalNumber { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
    }
}
