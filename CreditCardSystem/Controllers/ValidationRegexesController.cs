using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CreditCardSystem.Models;
using CreditCardSystem.ProcessManagers;
using CreditCardSystem.Interfaces;

namespace CreditCardSystem.Controllers
{
    public class ValidationRegexesController : Controller
    {
        private readonly CreditCardSystemContext _context;

        public ValidationRegexesController(CreditCardSystemContext context)
        {
            _context = context;
        }

        // GET: ValidationRegexes
        public async Task<IActionResult> Index()
        {
              return _context.ValidationRegex != null ? 
                          View(await _context.ValidationRegex.ToListAsync()) :
                          Problem("Entity set 'CreditCardSystemContext.ValidationRegex'  is null.");
        }

        // GET: ValidationRegexes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ValidationRegex == null)
            {
                return NotFound();
            }

            var validationRegex = await _context.ValidationRegex
                .FirstOrDefaultAsync(m => m.ValidationRegexId == id);
            if (validationRegex == null)
            {
                return NotFound();
            }

            return View(validationRegex);
        }

        // GET: ValidationRegexes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ValidationRegexes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ValidationRegexId,ValidationRegexString,ValidationRegexName")] ValidationRegex validationRegex)
        {
            if (ModelState.IsValid)
            {
                var regexFromDb = await _context.ValidationRegex.FirstOrDefaultAsync(x => x.ValidationRegexString == validationRegex.ValidationRegexString);

                if(regexFromDb != null)
                {
                    ViewBag.ValidationRegexError = $"This Regular Expression already exists as: {regexFromDb.ValidationRegexName}";
                    return View(validationRegex);

                }
                validationRegex.ValidationRegexId = Guid.NewGuid();
                _context.Add(validationRegex);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(validationRegex);
        }

        // GET: ValidationRegexes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ValidationRegex == null)
            {
                return NotFound();
            }

            var validationRegex = await _context.ValidationRegex.FindAsync(id);
            if (validationRegex == null)
            {
                return NotFound();
            }
            return View(validationRegex);
        }

        // POST: ValidationRegexes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ValidationRegexId,ValidationRegexString,ValidationRegexName")] ValidationRegex validationRegex)
        {
            if (id != validationRegex.ValidationRegexId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(validationRegex);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ValidationRegexExists(validationRegex.ValidationRegexId))
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
            return View(validationRegex);
        }

        // GET: ValidationRegexes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ValidationRegex == null)
            {
                return NotFound();
            }

            var validationRegex = await _context.ValidationRegex
                .FirstOrDefaultAsync(m => m.ValidationRegexId == id);
            if (validationRegex == null)
            {
                return NotFound();
            }

            return View(validationRegex);
        }

        // POST: ValidationRegexes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ValidationRegex == null)
            {
                return Problem("Entity set 'CreditCardSystemContext.ValidationRegex'  is null.");
            }

            var inUse = await _context.CardType.FirstOrDefaultAsync(x => x.ValidationRegexId == id) is not null;

            if (inUse)
            {
                return View("DeleteError");
            }

            var validationRegex = await _context.ValidationRegex.FindAsync(id);
            if (validationRegex != null)
            {
                _context.ValidationRegex.Remove(validationRegex);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ValidationRegexExists(Guid id)
        {
          return (_context.ValidationRegex?.Any(e => e.ValidationRegexId == id)).GetValueOrDefault();
        }
    }
}
