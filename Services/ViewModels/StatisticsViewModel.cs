using System.Collections.Generic;

namespace Services.ViewModels
{
    public class StatisticsViewModel
    {
        public int CustomerCount { get; set; }
        public int AccountCount { get; set; }
        public Dictionary<string, decimal> BalancePerCountry { get; set; } = new();
    }
}
