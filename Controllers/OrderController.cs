using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using StanaGO.Data;
using StanaGO.Models;
using StanaGO.Enums;
using StanaGO.ViewModels;

namespace StanaGO.Controllers
{
    public class OrderController : Controller
    {
        private readonly StanaGOContext _context;
        private readonly UserManager<User> _userManager;


        public OrderController(StanaGOContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        //metode petru ciobani(vanzatori)

        [Authorize(Roles = "Shepherd")]
        [HttpGet]
        public async Task<IActionResult> FromClients()
        {
            var sellerId = _userManager.GetUserId(User);

            var orders = await _context.Orders
                .Include(o => o.Product)
                .Include(o => o.Customer)
                .Where(o => o.SellerId == sellerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var model = orders.Select(o => new OrdersViewModel
            {
                Id = o.Id,
                ProductName = o.Product.Name,
                CustomerName = o.Customer.UserName!,
                CreatedAt = o.CreatedAt,
                Status = o.Status,
                CustomerConfirmed = o.CustomerConfirmation,
                SellerConfirmed = o.SellerConfirmation

            }).ToList();
            return View(model);
        }


        [Authorize(Roles = "Shepherd")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ConfirmSeller(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            order.SellerConfirmation = true;

            if (order.CustomerConfirmation)
            {
                order.Status = ProductStatus.Sold;
                order.Product.Status = ProductStatus.Sold;
            }

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(FromClients));
        }

        //metode pentru clienti

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var customerId = _userManager.GetUserId(User);

            var orders = await _context.Orders
                .Include(o => o.Product)
                .Include(o => o.Seller)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var model = orders.Select(o => new OrdersViewModel
            {
                Id = o.Id,
                ProductName = o.Product.Name,
                CreatedAt = o.CreatedAt,
                Status = o.Status,
                CustomerConfirmed = o.CustomerConfirmation,
                SellerConfirmed = o.SellerConfirmation
            }).ToList();

            return View(model);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddOrder(int productId)
        {
            var customerId = _userManager.GetUserId(User);
            var product = await _context.Products.Include(p => p.Farm)
                    .FirstOrDefaultAsync(p => p.Id == productId && p.Status == ProductStatus.Available); if (product == null)
            {
                return NotFound();
            }
            var order = new Order
            {
                CustomerId = customerId,
                SellerId = product.Farm.OwnerId,
                ProductId = product.Id,
                CreatedAt = DateTime.UtcNow,
                Status = ProductStatus.Queued
            };

            product.Status = ProductStatus.Queued;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyOrders));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ConfirmCustomer(int id)
        {
            var customerId = _userManager.GetUserId(User);

            var order = await _context.Orders.Include(o => o.Product).FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == customerId);

            if (order == null)
            {
                return NotFound();
            }

            order.CustomerConfirmation = true;

            if (order.SellerConfirmation)
            {
                order.Status = ProductStatus.Sold;
                order.Product.Status = ProductStatus.Sold;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyOrders));
        }


    }
}
