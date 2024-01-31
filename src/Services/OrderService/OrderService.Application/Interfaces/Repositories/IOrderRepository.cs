using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Application.Interfaces.Repositories;

public interface IOrderRepository : IGenericRepository<Order>
{

}