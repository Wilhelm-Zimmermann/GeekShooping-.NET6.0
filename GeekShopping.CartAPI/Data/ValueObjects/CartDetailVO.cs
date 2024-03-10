﻿using GeekShopping.CartApi.Model;
using GeekShopping.CartAPI.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.CartAPI.Data.ValueObjects
{
    public class CartDetailVO
    {
        public long Id { get; set; }
        public long CartHeaderId { get; set; }
        public CartHeaderVO CartHeader { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public int Count { get; set; }
    }
}