using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoffeeRun.Data;
using CoffeeRun.Models;
using CoffeeRun.Repositories;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using CoffeeRun.ViewModels;
using Microsoft.AspNetCore.Http;

namespace CoffeeRun.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public OrdersController(IOrderRepository orderRepository, 
            UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int Id)
        {
            string userId = _userManager.GetUserId(User);
            int runId;

            if (HttpContext.Session.GetInt32("runId") != null)
                runId = Convert.ToInt32(HttpContext.Session.GetInt32("runId"));
            else
            {
                runId = Id;
                HttpContext.Session.SetInt32("runId", runId);
            }
                
            var orders = await _orderRepository.FindOrdersByRunAsync(runId);
            var ordersViewModels = _mapper.Map<IEnumerable<OrderViewModel>>(orders);

            foreach (var orderViewModel in ordersViewModels)
            {
                if (orderViewModel.OrderUserId == userId)
                    orderViewModel.ableToModify = true;
                else
                    orderViewModel.ableToModify = false;
            }

            return View(ordersViewModels);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderRepository.FindOrderAsync(Convert.ToInt32(id));
            if (order == null)
            {
                return NotFound();
            }

            var orderViewModel = _mapper.Map<OrderViewModel>(order);
            return View(orderViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("size,HowWouldYouLikeIt,Quantity")] OrderViewModel orderViewModel)
        {
            if (ModelState.IsValid)
            {
                var order = _mapper.Map<Order>(orderViewModel);
                int? runId = HttpContext.Session.GetInt32("runId");
                if (runId == null)
                   return BadRequest();

                order.RunId = Convert.ToInt32(runId);
                order.OrderUserId = _userManager.GetUserId(User);
                await _orderRepository.CreateOrderAsync(order);

                return RedirectToAction(nameof(Index), new { id = runId });
            }
            return View(orderViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderRepository.FindOrderAsync(Convert.ToInt32(id));
            if (order == null)
            {
                return NotFound();
            }

            var orderViewModel = _mapper.Map<OrderViewModel>(order);
            return View(orderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,size,HowWouldYouLikeIt,Quantity")] OrderViewModel orderViewModel)
        {
            if (id != orderViewModel.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var order = await _orderRepository.FindOrderAsync(orderViewModel.OrderId);
                    if (order == null)
                        NotFound();

                    order.size = orderViewModel.size;
                    order.HowWouldYouLikeIt = orderViewModel.HowWouldYouLikeIt;
                    order.Quantity = orderViewModel.Quantity;

                    await _orderRepository.EditOrderAsync(order);
                    return RedirectToAction(nameof(Index), new { id = order.RunId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                
            }
            return View(orderViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderRepository.FindOrderAsync(Convert.ToInt32(id));
            if (order == null)
            {
                return NotFound();
            }

            var orderViewModel = _mapper.Map<OrderViewModel>(order);
            return View(orderViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var runId = Convert.ToInt32(HttpContext.Session.GetInt32("runId"));

            await _orderRepository.RemoveOrderAsync(id);
            return RedirectToAction(nameof(Index), new { id = runId});
        }
    }
}
