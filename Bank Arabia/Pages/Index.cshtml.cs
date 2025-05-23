using Services.ViewModels;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services;

public class IndexModel : PageModel
{
    private readonly StatisticsService _statisticsService;

    public IndexModel(StatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public StatisticsViewModel Stats { get; set; } = new();

    public async Task OnGetAsync()
    {
        Stats = await _statisticsService.GetStatisticsAsync();
    }
}
