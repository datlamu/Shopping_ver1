using Shopping_ver1.Models.ViewModels;

namespace Shopping_ver1.Services.Abstract
{
    public interface IDashboardService
    {
        // Thu nhâp theo tháng
        Task<decimal> GetMonthlyIncomeAsync(int month, int year);
        // Thu nhâp theo năm
        Task<decimal> GetYearlyIncomeAsync(int year);
        // Tính tăng trưởng doanh thu theo tháng
        string CalculateRevenueGrowth(decimal currentMonthIncome, decimal lastMonthIncome);
        // Doanh thu hàng tháng trong năm
        Task<(List<string> Labels, List<decimal> Values)> GetMonthlyRevenueDataAsync(int year);
        // Doanh thu phân theo thể loại sản phẩm
        Task<(List<string> Labels, List<decimal> Values)> GetCategoryRevenueDataAsync(int year);
        // Lấy dữ liệu thống kê
        Task<DashboardStatsViewModel> GetDashboardStatsAsync();
    }
}
