﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Models;

public class CustomerBasket
{
    public string BuyerId { get; set; }
    public List<BasketItem> Items { get; set; } = new List<BasketItem>();

    public CustomerBasket()
    {

    }

    public CustomerBasket(string buyerId)
    {
        BuyerId = buyerId;
    }
}