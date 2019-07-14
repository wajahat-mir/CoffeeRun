using CoffeeRun.Data;
using CoffeeRun.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeRun.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> FindOrdersByRunForUserAsync(int runId, string UserId);
        Task<IEnumerable<Order>> FindOrdersByRunAsync(int runId);
        Task<Order> FindOrderAsync(int orderId);
        Task CreateOrderAsync(Order order);
        Task EditOrderAsync(Order order);
        Task RemoveOrderAsync(int id);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Order>> FindOrdersByRunForUserAsync(int runId, string UserId)
        {
            var orders = await _db.Orders.Where(o => o.RunId == runId && o.OrderUserId == UserId).ToListAsync();
            return orders;
        }

        public async Task<IEnumerable<Order>> FindOrdersByRunAsync(int runId)
        {
            var orders = await _db.Orders.Where(o => o.RunId == runId).ToListAsync();
            return orders;
        }

        public async Task<Order> FindOrderAsync (int orderId)
        {
            return await _db.Orders.FindAsync(orderId);
        }

        public async Task CreateOrderAsync(Order order)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
        }

        public async Task EditOrderAsync(Order order)
        {
            _db.Entry(order).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
        public async Task RemoveOrderAsync(int id)
        {
            var order = await FindOrderAsync(id);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
        }
    }
}
