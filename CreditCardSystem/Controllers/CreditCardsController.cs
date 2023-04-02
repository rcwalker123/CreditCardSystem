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
            if (!ModelState.IsValid)
            {
                var cardNumberValidator = false;
                var regexId = _context.CardType.FirstOrDefault(x => x.CardTypeId == creditCard.CardTypeId)?.ValidationRegexId;
                if(regexId != null)
                {
                    var regex = _context.ValidationRegex.FirstOrDefault(x => x.ValidationRegexId == regexId);
                    var cardNumber = creditCard.CardNumber.Replace("-", "").Replace(" ", "");
                    cardNumberValidator = _validator.ValidateCardNumber(cardNumber, regex.ValidationRegexString);
                }
                
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

                if (!cardNumberValidator)
                {
                    ViewBag.CardNumberError = "Please enter a valid card number";
                }
                
                if (!cvvValidator || !dateValidator || !cardNumberValidator)
                {
                    ViewData["CardTypeId"] = new SelectList(_context.CardType, "CardTypeId", "CardTypeName", creditCard.CardTypeId);
                    return View(creditCard);
                }
                   
            }
            //Encrypt
            var encryptedCardNumber = Encryption.Encrypt(creditCard.CardNumber);

            //Check if card number exists
            ModelState.AddModelError(nameof(CreditCard.CardNumber), $"{creditCard.CardNumber} already exists");
            return View(creditCard);
            //if (cardFromDb != null)
            //{
            //    ModelState.AddModelError(nameof(CreditCard.CardNumber), $"{creditCard.CardNumber} already exists");
            //    return View(creditCard);
            //}
            creditCard.CreditCardId = Guid.NewGuid();
            //_context.Add(creditCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            ViewData["CardTypeId"] = new SelectList(_context.CardType, "CardTypeId", "CardTypeName", creditCard.CardTypeId);
            return View(creditCard);
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
