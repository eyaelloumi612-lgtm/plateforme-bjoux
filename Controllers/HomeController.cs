using Microsoft.AspNetCore.Mvc;
using JewelryManagementPlatform.Data;
using Microsoft.EntityFrameworkCore;

namespace JewelryManagementPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stats = new DashboardStats
            {
                TotalClients = await _context.Clients.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalCommandes = await _context.Commandes.CountAsync(),
                TotalRevenue = await _context.Factures.SumAsync(f => f.MontantTTC)
            };

            return View(stats);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }

    public class DashboardStats
    {
        public int TotalClients { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCommandes { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}