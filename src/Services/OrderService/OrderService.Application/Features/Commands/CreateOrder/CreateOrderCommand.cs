﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using OrderService.Application.Features.Queries.GetOrderDetailById;
using OrderService.Application.Features.Queries.ViewModels;
using OrderService.Domain.Models;

namespace OrderService.Application.Features.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<bool>
{
    private readonly List<OrderItemDTO> _orderItems;

    public CreateOrderCommand()
    {
        _orderItems = new List<OrderItemDTO>();
    }

    public CreateOrderCommand(List<BasketItem> basketItems, string userId, string userName, string city,string street, string state,string country,string zipCode,string cardNumber, string cardHolderName, DateTime cardExpiration, string cardSecurityNumber, int cardTypeId) : this()
    {
        var dtoList = basketItems.Select(item => new OrderItemDTO()
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            PictureUrl = item.PictureUrl,
            UnitPrice = item.UnitPrice,
            Units = item.Quantity
        });
        _orderItems = dtoList.ToList();
        UserName = userName;
        City = city;
        Street = street;
        State = state;
        Country = country;
        ZipCode = zipCode;
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        CardExpiration = cardExpiration;
        CardSecurityNumber = cardSecurityNumber;
        CardTypeId = cardTypeId;
    }

    public string UserName { get; private set; }
    public string City { get; private set; }
    public string Street { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string ZipCode { get; private set; }
    public string CardNumber { get; private set; }
    public string CardHolderName { get; private set; }
    public DateTime CardExpiration { get; private set; }
    public string CardSecurityNumber { get; private set; }
    public int CardTypeId { get; set; }
    public IEnumerable<OrderItemDTO> OrderItems => _orderItems;

    public string CorrelationId { get; set; }

    

}


public class OrderItemDTO
{
    public int ProductId { get; init; }
    public string ProductName { get; init; }
    public decimal UnitPrice { get; init; }
    public int Units { get; init; }
    public string PictureUrl { get; init; }
}