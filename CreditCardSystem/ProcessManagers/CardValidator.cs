using CreditCardSystem.Interfaces;
using CreditCardSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CreditCardSystem.ProcessManagers
{
    public class CardValidator : ICardValidator
    {
        private readonly CreditCardSystemContext _context;
        public CardValidator(CreditCardSystemContext context) 
        {
            _context = context;
        }
        public (bool, CardType?) ValidateCardNumber(string cardNumber)
        {
            var providers = _context.CardType.Where(x => x.IsActive).Include("ValidationRegex");

            CardType? matchedProvider = null;
            bool isValid = false;

            foreach (var provider in providers)
            {
                var regexResult = Regex.IsMatch(cardNumber, provider.ValidationRegex.ValidationRegexString);

                if (regexResult)
                {
                    isValid = true;
                    matchedProvider = provider;
                    break;
                }
            }
            return (isValid, matchedProvider);
        }

        public bool ValidateCVV(string cvv)
        {
            return Regex.IsMatch(cvv, "^[0-9]{3,4}$");
        }

        public bool ValidateDate(DateTime date)
        {
            var dateNow = DateTime.Now;
            if(date.Month >= dateNow.Month && date.Year >= dateNow.Year)
                return true;
            
            return false;

        }
    }
}
