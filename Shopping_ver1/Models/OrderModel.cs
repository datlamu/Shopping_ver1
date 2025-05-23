namespace Shopping_ver1.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int Status { get; set; } = 1;

        public OrderModel() { }
        public OrderModel(string orderCode, string userName)
        {
            this.OrderCode = orderCode;
            this.UserName = userName;
        }
    }
}
