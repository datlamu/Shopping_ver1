namespace Shopping_ver1.Helpers
{
    public class Paginate
    {
        public int TotalItems { get; private set; }     // Tổng số item
        public int PageSize { get; private set; }       // Số lượng item trên 1 trang
        public int TotalPages { get; private set; }     // Tổng số trang
        public int CurrentPage { get; private set; }    // Trang hiện tại
        public int StartPage { get; private set; }      // Trang đầu
        public int EndPage { get; private set; }        // Trang cuối
        public int Skip => (CurrentPage - 1) * PageSize;// Số lượng item bị bỏ qua

        public Paginate() { }

        public Paginate(int totalItems, int pageInput, int pageSize = 10)
        {
            // Tổng số Item
            TotalItems = totalItems;

            // Số lượng item trên 1 trang
            PageSize = pageSize;

            // Tổng số trang ( làm tròn lên nếu không chia hết VD: 3.1 = 4 )
            TotalPages = (int)Math.Ceiling((decimal)totalItems / PageSize);

            // Trang hiện tại
            CurrentPage = Math.Clamp(pageInput, 1, TotalPages);

            // Trang đầu
            StartPage = CurrentPage - 5;

            // Trang cuối
            EndPage = CurrentPage + 4;

            // Không cho xuống trang âm và giúp xử lý trang cuối ở code sau luôn nằm đúng trong khoảng hợp lý
            // * Sinh ra từ việc tránh trang âm, thường dành riêng cho 5 trang đầu tiên
            if (StartPage <= 0)
            {
                EndPage = EndPage - (StartPage - 1);
                StartPage = 1;
            }

            // Giữ trang cuối luôn <= tổng trang 
            // * Sinh ra từ việc điều chỉnh trang khoảng đầu và cuối, giữ cho trang cuối không tràn ra khỏi tổng trang
            if (EndPage > TotalPages)
            {
                EndPage = TotalPages;
                // Khi qua 10 trang đầu sẽ xử lý để trang đầu và trang cuối luôn nằm khoảng 10 trang hiện ra
                if (EndPage > 10)
                    StartPage = EndPage - 9;
            }
        }
    }
}
