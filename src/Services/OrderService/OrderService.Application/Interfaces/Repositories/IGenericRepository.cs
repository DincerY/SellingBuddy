using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OrderService.Domain.SeedWork;

namespace OrderService.Application.Interfaces.Repositories;

public interface IGenericRepository<T> : IRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAll();
    Task<List<T>> Get(Expression<Func<T, bool>> filter,Func<IQueryable<T>, IOrderedQueryable<T>> func);
    Task<List<T>> Get(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);

    Task<T> GetById(Guid id);

    Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);

    IQueryable<T> GetSingleAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);


    Task<T> AddAsync(T entity);

    T Update(T entity);

}