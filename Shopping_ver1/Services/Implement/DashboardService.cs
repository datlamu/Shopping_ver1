using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models.ViewModels;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

public class DashboardService : IDashboardService
{
    private readonly DataContext _context;

    public DashboardService(DataContext context)
    {
        _context = context;
    }

    // Thu nhập theo tháng
    public async Task<decimal> GetMonthlyIncomeAsync(int month, int year)
    {
        return await _context.Orders
                .Where(o => o.CreateDate.Month == month && o.CreateDate.Year == year)
                .SumAsync(o => (decimal?)o.TotalPayment) ?? 0;
    }

    // Thu nhập theo năm
    public async Task<decimal> GetYearlyIncomeAsync(int year)
    {
        return await _context.Orders
                .Where(o => o.CreateDate.Year == year)
                .SumAsync(o => (decimal?)o.TotalPayment) ?? 0;
    }

    // Tính tăng trưởng doanh thu theo tháng
    public string CalculateRevenueGrowth(decimal currentMonthIncome, decimal lastMonthIncome)
    {
        if (lastMonthIncome == 0 && currentMonthIncome == 0)
            return "Không có dữ liệu";
        if (lastMonthIncome == 0)
            return "Mới";
        if (currentMonthIncome == 0)
            return "Tháng này chưa có đơn hàng nào";

        // So sánh tháng này với tháng trước - format theo %
        var growth = ((double)(currentMonthIncome - lastMonthIncome) / (double)lastMonthIncome) * 100;
        return $"{Math.Round(growth, 2)}%";
    }

    // Doanh thu hàng tháng trong năm
    public async Task<(List<string> Labels, List<decimal> Values)> GetMonthlyRevenueDataAsync(int year)
    {
        var culture = new CultureInfo("vi-VN");
        var monthlyLabels = Enumerable.Range(1, 12)
            .Select(m => new DateTime(year, m, 1).ToString("MMM", culture))
            .ToList();

        var monthlyData = new List<decimal>();
        for (int month = 1; month <= 12; month++)
        {
            var total = await _context.Orders
                .Where(o => o.CreateDate.Year == year && o.CreateDate.Month == month)
                .SumAsync(o => (decimal?)o.TotalPayment) ?? 0;
            monthlyData.Add(total);
        }
        return (monthlyLabels, monthlyData);
    }

    // Doanh thu phân theo thể loại sản phẩm
    public async Task<(List<string> Labels, List<decimal> Values)> GetCategoryRevenueDataAsync(int year)
    {
        // danh sách thể loại và tổng doanh thu thể loại đó
        var data = await (
            from od in _context.OrderDetails
            join o in _context.Orders on od.OrderCode equals o.OrderCode
            join p in _context.Products on od.ProductId equals p.Id
            join c in _context.Categories on p.CategoryId equals c.Id
            where o.CreateDate.Year == year
            group (od.Quantity * od.Price) by c.Name into g
            select new
            {
                CategoryName = g.Key,
                Revenue = g.Sum()
            }
        )
        .OrderByDescending(x => x.Revenue)
        .ToListAsync();

        // Lấy nhãn và giá trị
        List<string> labels = new();
        List<decimal> values = new();

        // Đảm bảo tối đa hiện 5 mục - 4 thể loại nhiều doanh thu nhấtt và mục khác 
        if (data.Count > 0)
        {
            var top = data.Take(4).ToList();
            var others = data.Skip(4).Sum(x => x.Revenue);

            foreach (var item in top)
            {
                labels.Add(item.CategoryName);
                values.Add(item.Revenue);
            }

            if (others > 0)
            {
                labels.Add("Khác");
                values.Add(others);
            }
        }

        return (labels, values);
    }

    // Lấy dữ liệu thống kê
    public async Task<DashboardStatsViewModel> GetDashboardStatsAsync()
    {
        // Thời gian hiện tại
        var now = DateTime.Now;
        var currentMonth = now.Month;
        var currentYear = now.Year;

        // Tháng trước
        var lastMonthDate = now.AddMonths(-1);
        var lastMonth = lastMonthDate.Month;
        var lastMonthYear = lastMonthDate.Year;

        // Tính thu nhập theo tháng, năm và mức tăng trưởng
        var monthlyIncome = await GetMonthlyIncomeAsync(currentMonth, currentYear);
        var lastMonthIncome = await GetMonthlyIncomeAsync(lastMonth, lastMonthYear);
        var yearlyIncome = await GetYearlyIncomeAsync(currentYear);
        var growth = CalculateRevenueGrowth(monthlyIncome, lastMonthIncome);

        // Lấy nhãn và doanh thu hàng tháng
        var (monthlyLabels, monthlyData) = await GetMonthlyRevenueDataAsync(currentYear);

        // Lấy nhãn và doanh thu theo thể loại sản phẩm 
        var (categoryLabels, categoryValues) = await GetCategoryRevenueDataAsync(currentYear);

        return new DashboardStatsViewModel
        {
            MonthlyIncome = monthlyIncome,
            LastMonthIncome = lastMonthIncome,
            YearlyIncome = yearlyIncome,
            MonthlyRevenueGrowthText = growth,
            MonthlyLabels = monthlyLabels,
            MonthlyData = monthlyData,
            CategoryLabels = categoryLabels,
            CategoryData = categoryValues
        };
    }
}
