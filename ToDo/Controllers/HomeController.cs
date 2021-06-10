using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ToDo.Data;
using ToDo.Models;

namespace ToDo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<CetUser> userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<CetUser> userManager)
        {
            _logger = logger;
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task< IActionResult> Index()
        {
            List<todoItem> result;
            if(User.Identity.IsAuthenticated)
            {
                var cetUser= await userManager.GetUserAsync(HttpContext.User);
                      var query = dbContext.todoItems
                            .Include(t=> t.Category)
                            .Where(t =>t.CetUserId ==cetUser.Id && !t.IsCompleted)
                            .OrderBy(t => t.DueDate)
                            .Take(3);
                           result  = await query.ToListAsync();

            }
            else
            {
                result = new List<todoItem>();
            }
           return View(result);
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
