using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Shopping_ver1.Models
{
    public class OrderDetailModel
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        [Range(0.01, double.MaxValue)]
        [Precision(18, 0)]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public ProductModel Product { get; set; }

        public OrderDetailModel() { }
        public OrderDetailModel(string userName, string orderCode, CartItemModel cart)
        {
            this.UserName = userName;
            this.OrderCode = orderCode;
            this.ProductId = cart.ProductId;
            this.Price = cart.Price;
            this.Quantity = cart.Quantity;
        }
    }
}
