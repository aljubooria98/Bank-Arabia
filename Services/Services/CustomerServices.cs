using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;
using DataAccessLayer.Models;

namespace Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly BankAppDataContext _context;

        public CustomerService(BankAppDataContext context)
        {
            _context = context;
        }

        // Tidigare metod - lämnas kvar ifall något annat använder den
        public async Task<CustomerSearchResultPageViewModel> SearchCustomersAsync(string? name, string? city, int page)
        {
            const int pageSize = 50;

            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(c => c.Givenname.Contains(name) || c.Surname.Contains(name));

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(c => c.City.Contains(city));

            var totalCount = await query.CountAsync();

            var customers = await query
                .OrderBy(c => c.CustomerId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerSearchResultViewModel
                {
                    CustomerId = c.CustomerId,
                    PersonalNumber = c.NationalId ?? "",
                    Name = $"{c.Givenname} {c.Surname}",
                    Address = c.Streetaddress ?? "",
                    City = c.City ?? "",
                    Phone = $"{c.Telephonecountrycode} {c.Telephonenumber}".Trim()
                })
                .ToListAsync();

            return new CustomerSearchResultPageViewModel
            {
                Customers = customers,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        public async Task<CustomerDetailsViewModel?> GetCustomerDetailsAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Dispositions)
                    .ThenInclude(d => d.Account)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                return null;

            var accounts = customer.Dispositions
                .Where(d => d.Account != null)
                .Select(d => new AccountDetailsViewModel
                {
                    AccountId = d.Account!.AccountId,
                    Balance = d.Account.Balance,
                    Transactions = d.Account.Transactions.Select(t => new TransactionViewModel
                    {
                        TransactionId = t.TransactionId,
                        Amount = t.Amount,
                        Date = t.Date.ToDateTime(TimeOnly.MinValue),
                        Operation = t.Operation
                    }).ToList()
                })
                .ToList();

            return new CustomerDetailsViewModel
            {
                CustomerId = customer.CustomerId,
                Gender = customer.Gender,
                Givenname = customer.Givenname,
                Surname = customer.Surname,
                Streetaddress = customer.Streetaddress,
                City = customer.City,
                Zipcode = customer.Zipcode,
                Country = customer.Country,
                CountryCode = customer.CountryCode,
                Birthday = customer.Birthday,
                NationalId = customer.NationalId,
                Telephonecountrycode = customer.Telephonecountrycode,
                Telephonenumber = customer.Telephonenumber,
                Emailaddress = customer.Emailaddress,
                Accounts = accounts
            };
        }

        public async Task<CustomerSearchResultViewModel?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers
                .Where(c => c.CustomerId == customerId)
                .Select(c => new CustomerSearchResultViewModel
                {
                    CustomerId = c.CustomerId,
                    PersonalNumber = c.NationalId ?? "",
                    Name = $"{c.Givenname} {c.Surname}",
                    Address = c.Streetaddress ?? "",
                    City = c.City ?? "",
                    Phone = $"{c.Telephonecountrycode} {c.Telephonenumber}".Trim()
                })
                .FirstOrDefaultAsync();
        }

        // ✅ Ny metod med generisk paginering
        public async Task<PagedResult<Customer>> GetPagedCustomersAsync(int page, int pageSize, string? name, string? city, int? customerId)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(c => c.Givenname.Contains(name) || c.Surname.Contains(name));

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(c => c.City.Contains(city));

            if (customerId.HasValue)
                query = query.Where(c => c.CustomerId == customerId.Value);

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.CustomerId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Customer>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
