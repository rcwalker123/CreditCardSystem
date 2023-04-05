using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CreditCardSystem.Models;

namespace CreditCardSystem.Controllers
{
    public class CardTypesController : Controller
    {
        private readonly CreditCardSystemContext _context;

        public CardTypesController(CreditCardSystemContext context)
        {
            _context = context;
        }

        // GET: CardTypes
        public async Task<IActionResult> Index()
        {
            var creditCardSystemContext = _context.CardType.Include(c => c.ValidationRegex);
            return View(await creditCardSystemContext.ToListAsync());
        }

        // GET: CardTypes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.CardType == null)
            {
                return NotFound();
            }

            var cardType = await _context.CardType
                .Include(c => c.ValidationRegex)
                .FirstOrDefaultAsync(m => m.CardTypeId == id);
            if (cardType == null)
            {
                return NotFound();
            }

            return View(cardType);
        }

        // GET: CardTypes/Create
        public IActionResult Create()
        {
            //Only show non used providers
            var usedRegexes = _context.CardType.Select(s => s.ValidationRegexId).Distinct();

            var unusedRegexes = _context.ValidationRegex.Where(c => !usedRegexes.Contains(c.ValidationRegexId));

            ViewData["ValidationRegexId"] = new SelectList(unusedRegexes, "ValidationRegexId", "ValidationRegexName");
            return View();
        }

        // POST: CardTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CardTypeId,CardTypeName,IsActive,ValidationRegexId")] CardType cardType)
        {
            if (ModelState.IsValid)
            {
                cardType.CardTypeId = Guid.NewGuid();
                _context.Add(cardType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ValidationRegexId"] = new SelectList(_context.ValidationRegex, "ValidationRegexId", "ValidationRegexString", cardType.ValidationRegexId);
            return View(cardType);
        }

        // GET: CardTypes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.CardType == null)
            {
                return NotFound();
            }

            var cardType = await _context.CardType.FindAsync(id);
            if (cardType == null)
            {
                return NotFound();
            }
            ViewData["ValidationRegexId"] = new SelectList(_context.ValidationRegex, "ValidationRegexId", "ValidationRegexString", cardType.ValidationRegexId);
            return View(cardType);
        }

        // POST: CardTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CardTypeId,CardTypeName,IsActive,ValidationRegexId")] CardType cardType)
        {
            if (id != cardType.CardTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var cardTypeModel = _context.CardType.First(x => x.CardTypeId == cardType.CardTypeId);

                    cardTypeModel.IsActive = cardType.IsActive;
                    _context.Update(cardTypeModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CardTypeExists(cardType.CardTypeId))
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
            ViewData["ValidationRegexId"] = new SelectList(_context.ValidationRegex, "ValidationRegexId", "ValidationRegexString", cardType.ValidationRegexId);
            return View(cardType);
        }

        // GET: CardTypes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.CardType == null)
            {
                return NotFound();
            }

            var cardType = await _context.CardType
                .Include(c => c.ValidationRegex)
                .FirstOrDefaultAsync(m => m.CardTypeId == id);
            if (cardType == null)
            {
                return NotFound();
            }

            return View(cardType);
        }

        // POST: CardTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.CardType == null)
            {
                return Problem("Entity set 'CreditCardSystemContext.CardType'  is null.");
            }

            var inUse = await _context.CreditCard.FirstOrDefaultAsync(x => x.CardTypeId == id) is not null;

            if (inUse)
            {
                return View("DeleteError");
            }

            var cardType = await _context.CardType.FindAsync(id);
            if (cardType != null)
            {
                _context.CardType.Remove(cardType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CardTypeExists(Guid id)
        {
          return (_context.CardType?.Any(e => e.CardTypeId == id)).GetValueOrDefault();
        }
    }
}
