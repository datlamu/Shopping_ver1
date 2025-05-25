﻿namespace Shopping_ver1.Models
{
    public class OrderDetailModel
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public ProductModel Product { get; set; }

        public OrderDetailModel() { }
        public OrderDetailModel(OrderModel order, CartItemModel cart)
        {
            this.OrderCode = order.OrderCode;
            this.UserName = order.UserName;
            this.ProductId = cart.ProductId;
            this.Price = cart.Price;
            this.Quantity = cart.Quantity;
        }
    }
}
