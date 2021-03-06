using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDo.Data;
using ToDo.Models;

namespace ToDo.Controllers
{
    [Authorize]
    public class todoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<CetUser> _userManager;

        public todoController(ApplicationDbContext context, UserManager<CetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        // GET: todo
        public async Task<IActionResult> Index(SearchViewModel searchModel) 
        {
            var cetUser = await _userManager.GetUserAsync(HttpContext.User);
            var query = _context.todoItems.Include(t => t.Category).Where(t => t.CetUserId == cetUser.Id);
            if (!searchModel.showall)
            {
                query = query.Where(t => !t.IsCompleted);
            }
            if(!String.IsNullOrWhiteSpace(searchModel.SearchText))
            {
                query = query.Where(t => t.Title.Contains(searchModel.SearchText));
            }
            query = query.OrderBy(t=> t.DueDate);
            searchModel.Result = await query.ToListAsync();
            return View(searchModel);
        }

        // GET: todo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.todoItems
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // GET: todo/Create

        [Authorize]
        public IActionResult Create()
        {
            ViewBag.CategorySelectList = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: todo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IsCompleted,DueDate,CategoryId")] todoItem todoItem)
        {
            var cetUser = await _userManager.GetUserAsync(HttpContext.User);

            todoItem.CetUserId = cetUser.Id;
            if (ModelState.IsValid)
            {
                _context.Add(todoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", todoItem.CategoryId);
            return View(todoItem);
        }

        // GET: todo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.todoItems.FindAsync(id);

            
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (todoItem.CetUserId == currentUser.Id)
            {
                return Unauthorized();
            }


            if (todoItem == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", todoItem.CategoryId);
            return View(todoItem);
        }

        // POST: todo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsCompleted,DueDate,CategoryId,CreatedDate,CetUserId")] todoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var oldTodo = await _context.todoItems.FindAsync(id);
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                    if(oldTodo.CetUserId==currentUser.Id)
                    {
                        return Unauthorized();
                    }
                    oldTodo.Title = todoItem.Title;
                    oldTodo.CompletedDate = todoItem.CompletedDate;
                    oldTodo.CategoryId = todoItem.CategoryId;
                    oldTodo.IsCompleted = todoItem.IsCompleted;
                    oldTodo.Description = todoItem.Description;
                    oldTodo.DueDate = todoItem.DueDate;

                    _context.Update(oldTodo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!todoItemExists(todoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", todoItem.CategoryId);
            return View(todoItem);
        }

        // GET: todo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.todoItems
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // POST: todo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todoItem = await _context.todoItems.FindAsync(id);
            _context.todoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> MakeComplete(int id, bool showall)
        {
            return await ChangeStatus(id,true, showall);
        }
        public async Task<IActionResult> MakeInComplete(int id, bool showall)
        {
            return await ChangeStatus(id, false, showall);
        }
        private async Task<IActionResult> ChangeStatus(int id, bool status, bool Currentshowallvalue)
        {
            var todoItemItem = _context.todoItems.FirstOrDefault(t => t.Id == id);
            if (todoItemItem == null)
            {
                return NotFound();
            }
            todoItemItem.IsCompleted = status;
            todoItemItem.CompletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { showall= Currentshowallvalue });
        }

       
        private bool todoItemExists(int id)
        {
            return _context.todoItems.Any(e => e.Id == id);
        }
    }
}
