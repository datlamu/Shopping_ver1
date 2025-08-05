namespace Shopping_ver1.Models.ViewModels
{
    public class DashboardStatsViewModel
    {
        public decimal MonthlyIncome { get; set; }
        public decimal LastMonthIncome { get; set; }
        public decimal YearlyIncome { get; set; }
        public string MonthlyRevenueGrowthText { get; set; }
        public List<string> MonthlyLabels { get; set; } = new();
        public List<decimal> MonthlyData { get; set; } = new();
        public List<string> CategoryLabels { get; set; } = new();
        public List<decimal> CategoryData { get; set; } = new();
    }
}
