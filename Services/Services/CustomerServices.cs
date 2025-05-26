using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.Services;
using Services.ViewModels;
using DataAccessLayer.Data;

namespace Services.Services
{

    public class CustomerService
    {
        private readonly BankAppDataContext _context;
        private const int PageSize = 50;

        public CustomerService(BankAppDataContext context)
        {
            _context = context;
        }

        public async Task<(List<CustomerSearchResultViewModel> Results, int TotalCount)> SearchCustomersAsync(string? name, string? city, int page)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(c => (c.Givenname + " " + c.Surname).Contains(name));

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(c => c.City.Contains(city));

            int totalCount = await query.CountAsync();

            var results = await query
                .OrderBy(c => c.CustomerId)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new CustomerSearchResultViewModel
                {
                    CustomerId = c.CustomerId,
                    PersonalNumber = c.NationalId ?? "",
                    Name = c.Givenname + " " + c.Surname,
                    Address = c.Streetaddress,
                    City = c.City,
                    Phone = c.Telephonenumber ?? ""
                })
                .ToListAsync();

            return (results, totalCount);
        }

        public async Task<CustomerSearchResultViewModel?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers
                .Where(c => c.CustomerId == customerId)
                .Select(c => new CustomerSearchResultViewModel
                {
                    CustomerId = c.CustomerId,
                    PersonalNumber = c.NationalId ?? "",
                    Name = c.Givenname + " " + c.Surname,
                    Address = c.Streetaddress,
                    City = c.City,
                    Phone = c.Telephonenumber ?? ""
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerDetailsViewModel?> GetCustomerDetailsAsync(int customerId)
        {
            return await _context.Customers
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Dispositions)
                    .ThenInclude(d => d.Account)
                        .ThenInclude(a => a.Transactions)
                .Select(c => new CustomerDetailsViewModel
                {
                    CustomerId = c.CustomerId,
                    Name = c.Givenname + " " + c.Surname,
                    Country = c.Country,
                    Address = c.Streetaddress,
                    Phone = c.Telephonenumber ?? "",
                    Accounts = c.Dispositions.Select(d => d.Account).Select(a => new AccountViewModel
                    {
                        AccountId = a.AccountId,
                        Balance = a.Balance,
                        Transactions = a.Transactions.Select(t => new TransactionViewModel
                        {
                            TransactionId = t.TransactionId,
                            Amount = t.Amount,
                            Date = t.Date.ToDateTime(TimeOnly.MinValue),
                            Operation = t.Type
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public IQueryable<CustomerSearchResultViewModel> GetCustomersQuery(string? name, string? city)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(c => (c.Givenname + " " + c.Surname).Contains(name));

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(c => c.City.Contains(city));

            return query
                .OrderBy(c => c.CustomerId)
                .Select(c => new CustomerSearchResultViewModel
                {
                    CustomerId = c.CustomerId,
                    PersonalNumber = c.NationalId ?? "",
                    Name = c.Givenname + " " + c.Surname,
                    Address = c.Streetaddress,
                    City = c.City,
                    Phone = c.Telephonenumber ?? ""
                });
        }
    }
}
