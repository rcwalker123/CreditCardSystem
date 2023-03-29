using CreditCardSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CreditCardSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CreditCardSystemContext _dbContext;

        public HomeController(ILogger<HomeController> logger, CreditCardSystemContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            _dbContext.CardType.Add(new CardType { CardTypeId = Guid.NewGuid(), CardTypeName = "VISA" });
            _dbContext.CardType.Add(new CardType { CardTypeId = Guid.NewGuid(), CardTypeName = "AMEX" });
            _dbContext.CardType.Add(new CardType { CardTypeId = Guid.NewGuid(), CardTypeName = "MasterCard" });
            _dbContext.CardType.Add(new CardType { CardTypeId = Guid.NewGuid(), CardTypeName = "Discover" });
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}