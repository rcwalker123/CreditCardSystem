using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CreditCardSystem.Models;
using CreditCardSystem.Interfaces;
using CreditCardSystem.ProcessManagers;

namespace CreditCardSystem.Controllers
{
    public class CreditCardsController : Controller
    {
        private readonly CreditCardSystemContext _context;
        private readonly ICardValidator _validator;

        public CreditCardsController(CreditCardSystemContext context, ICardValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        // GET: CreditCards
        public async Task<IActionResult> Index()
        {
            var creditCardSystemContext = _context.CreditCard.Include(c => c.CardType);
            return View(await creditCardSystemContext.ToListAsync());
        }

        // GET: CreditCards/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.CreditCard == null)
            {
                return NotFound();
            }

            var creditCard = await _context.CreditCard
                .Include(c => c.CardType)
                .FirstOrDefaultAsync(m => m.CreditCardId == id);
            if (creditCard == null)
            {
                return NotFound();
            }

            return View(creditCard);
        }

        // GET: CreditCards/Create
        public IActionResult Create()
        {
            ViewData["CardTypeId"] = new SelectList(_context.CardType, "CardTypeId", "CardTypeName");
            return View();
        }

        // POST: CreditCards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CreditCardId,CardNumber,CardExpiry,CardCvv,CardTypeId")] CreditCard creditCard)
        {

            var cardNumberValidator = false;
            CardType? matchedProvider = null;

            var cardNumber = creditCard.CardNumber.Replace("-", "").Replace(" ", "");
            (cardNumberValidator, matchedProvider) = _validator.ValidateCardNumber(cardNumber);

            var dateValidator = _validator.ValidateDate(creditCard.CardExpiry);
            var cvvValidator = _validator.ValidateCVV(creditCard.CardCvv);

            if (!dateValidator)
            {
                ViewBag.DateError = "Please enter a date from now";
            }

            if (!cvvValidator)
            {
                ViewBag.CvvError = "Please enter a date from now";
            }

            if (!cardNumberValidator || matchedProvider == null)
            {
                ViewBag.CardNumberError = "Please enter a valid card number for an active card type";
            }

            if (!cvvValidator || !dateValidator || !cardNumberValidator)
            {
                return View(creditCard);
            }


            //Normally we would encrypt the card number to prevent storing it in plaintext

            var creditCardFromDb = await _context.CreditCard.FirstOrDefaultAsync(x => x.CardNumber == creditCard.CardNumber);

            if (creditCardFromDb != null)
            {
                ViewBag.CardNumberError = "This card has already been captured";
                ;
                return View(creditCard);
            }

            creditCard.CreditCardId = Guid.NewGuid();

            //At this point we should not have a null matchedProvider as this is caught in the above if statements
            creditCard.CardTypeId = matchedProvider.CardTypeId;
            _context.Add(creditCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: CreditCards/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.CreditCard == null)
            {
                return NotFound();
            }

            var creditCard = await _context.CreditCard.FindAsync(id);
            if (creditCard == null)
            {
                return NotFound();
            }
            ViewData["CardTypeId"] = new SelectList(_context.CardType, "CardTypeId", "CardTypeName", creditCard.CardTypeId);
            return View(creditCard);
        }

        // POST: CreditCards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CreditCardId,CardNumber,CardExpiry,CardCvv,CardTypeId")] CreditCard creditCard)
        {
            if (id != creditCard.CreditCardId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(creditCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreditCardExists(creditCard.CreditCardId))
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
            ViewData["CardTypeId"] = new SelectList(_context.CardType, "CardTypeId", "CardTypeName", creditCard.CardTypeId);
            return View(creditCard);
        }

        // GET: CreditCards/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.CreditCard == null)
            {
                return NotFound();
            }

            var creditCard = await _context.CreditCard
                .Include(c => c.CardType)
                .FirstOrDefaultAsync(m => m.CreditCardId == id);
            if (creditCard == null)
            {
                return NotFound();
            }

            return View(creditCard);
        }

        // POST: CreditCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.CreditCard == null)
            {
                return Problem("Entity set 'CreditCardSystemContext.CreditCard'  is null.");
            }
            var creditCard = await _context.CreditCard.FindAsync(id);
            if (creditCard != null)
            {
                _context.CreditCard.Remove(creditCard);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreditCardExists(Guid id)
        {
            return (_context.CreditCard?.Any(e => e.CreditCardId == id)).GetValueOrDefault();
        }
    }
}
