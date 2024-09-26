﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}
