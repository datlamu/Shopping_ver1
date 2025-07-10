using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping_ver1.Repository.Components
{
    public class SliderViewComponent : ViewComponent
    {
        // Gọi db chỉ đọc và dùng trong class này
        private readonly DataContext _dataContext;
        public SliderViewComponent(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Sliders.Where(s => s.Status == 1).ToListAsync());
    }
}
