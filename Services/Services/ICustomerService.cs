using Services.ViewModels;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface ICustomerService
    {
        Task<CustomerSearchResultPageViewModel> SearchCustomersAsync(string? name, string? city, int page);
        Task<CustomerDetailsViewModel?> GetCustomerDetailsAsync(int customerId);
        Task<CustomerSearchResultViewModel?> GetCustomerByIdAsync(int customerId);
    }
}
