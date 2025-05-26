using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Services.Services;

[Authorize(Roles = "Admin,Cashier")]
public class SearchModel : PageModel
{
    private readonly BankAppDataContext _context;

    public SearchModel(BankAppDataContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string Name { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public string City { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public int? CustomerId { get; set; }  // ✅ Ny egenskap

    [BindProperty(SupportsGet = true, Name = "p")]
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 50;
    public int TotalPages { get; set; }

    public List<Customer> Results { get; set; } = new();

    public void OnGet()
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(Name))
        {
            query = query.Where(c =>
                EF.Functions.Like(c.Givenname, $"%{Name}%") ||
                EF.Functions.Like(c.Surname, $"%{Name}%"));
        }

        if (!string.IsNullOrWhiteSpace(City))
        {
            query = query.Where(c =>
                EF.Functions.Like(c.City, $"%{City}%"));
        }

        if (CustomerId.HasValue) 
        {
            query = query.Where(c => c.CustomerId == CustomerId.Value);
        }

        int totalItems = query.Count();
        TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

        Results = query
            .OrderBy(c => c.CustomerId)
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }
}
