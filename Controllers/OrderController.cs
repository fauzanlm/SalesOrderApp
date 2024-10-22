using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using SalesOrderApp.Data;
using SalesOrderApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SalesOrderApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchQuery, string orderDate, int pageNumber = 1, int pageSize = 10)
        {
            ViewBag.SearchQuery = searchQuery;
            ViewBag.OrderDate = orderDate;

            var orders = _context.Orders.Include(o => o.Customer).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                orders = orders.Where(o =>
                    o.SalesOrderNumber.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    o.OrderDate.ToString("dd/MM/yyyy").Contains(searchQuery) ||
                    o.Customer.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            if (DateTime.TryParse(orderDate, out DateTime parsedDate))
            {
                orders = orders.Where(o => o.OrderDate.Date == parsedDate.Date);
            }

            var totalOrders = await orders.CountAsync();

            var ordersPaged = await orders.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalOrders = totalOrders;

            return View(ordersPaged);
        }

        public IActionResult OrderForm(string orderId = "")
        {
            var customers = _context.Customers.ToList();
            ViewBag.Customers = new SelectList(customers, "Id", "Name");

            if (!string.IsNullOrEmpty(orderId))
            {
                var order = _context.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == int.Parse(orderId));
                if (order != null)
                {
                    return View(order);
                }
            }

            return View(new Order()); 
        }


        [HttpPost]
        public async Task<IActionResult> SaveOrder(Order order, string orderItemsJson)
        {
            if (ModelState.IsValid)
            {
                var customer = await _context.Customers.FindAsync(order.CustomerId);
                if (customer == null)
                {
                    ModelState.AddModelError("CustomerId", "Customer not found");
                    return View("OrderForm", order);
                }
                
                var orderItems = JsonConvert.DeserializeObject<List<OrderItem>>(orderItemsJson);

                if (orderItems == null || !orderItems.Any())
                {
                    ModelState.AddModelError("", "Order items not valid.");
                    return View("OrderForm", order); 
                }

                if (order.Id == 0)
                {
                    order.Customer = customer; 
                    order.OrderItems = orderItems;
                    _context.Orders.Add(order); 
                }
                else
                {
                    var existingOrder = await _context.Orders
                        .Include(o => o.OrderItems)
                        .FirstOrDefaultAsync(o => o.Id == order.Id);
                    if (existingOrder != null)
                    {
                        existingOrder.SalesOrderNumber = order.SalesOrderNumber;
                        existingOrder.OrderDate = order.OrderDate;
                        existingOrder.Address = order.Address;
                        existingOrder.CustomerId = order.CustomerId; 
                        existingOrder.Customer = customer; 

                        existingOrder.OrderItems.Clear();

                        foreach (var item in orderItems)
                        {
                            item.OrderId = existingOrder.Id; 
                            existingOrder.OrderItems.Add(item); 
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var customers = _context.Customers.ToList();
            ViewBag.Customers = new SelectList(customers, "Id", "Name");
            return View("OrderForm", order);
        }

        public async Task<IActionResult> ExportToExcel(string searchQuery, string orderDate)
        {
            
            var orders = await _context.Orders.Include(o => o.Customer).ToListAsync();

            
            if (!string.IsNullOrEmpty(searchQuery))
            {
                orders = orders.Where(o =>
                    o.SalesOrderNumber.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    o.OrderDate.ToString("dd/MM/yyyy").Contains(searchQuery) ||
                    o.Customer.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            
            if (DateTime.TryParse(orderDate, out DateTime parsedDate))
            {
                orders = orders.Where(o => o.OrderDate.Date == parsedDate.Date).ToList();
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");
                worksheet.Cell(1, 1).Value = "No";
                worksheet.Cell(1, 2).Value = "Sales Order";
                worksheet.Cell(1, 3).Value = "Order Date";
                worksheet.Cell(1, 4).Value = "Customer";

                int row = 2;
                for (int i = 0; i < orders.Count; i++)
                {
                    worksheet.Cell(row, 1).Value = i + 1;
                    worksheet.Cell(row, 2).Value = orders[i].SalesOrderNumber;
                    worksheet.Cell(row, 3).Value = orders[i].OrderDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 4).Value = orders[i].Customer.Name;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var fileName = $"Orders_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

    }
}
