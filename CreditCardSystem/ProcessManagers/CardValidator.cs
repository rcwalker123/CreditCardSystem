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
            bool regexResult = false;

            if(!IsCardNumberValid(cardNumber))
            {
                return (isValid, matchedProvider);
            }

            foreach (var provider in providers)
            {
                try
                {
                    regexResult = Regex.IsMatch(cardNumber, provider.ValidationRegex.ValidationRegexString);

                }
                catch(Exception ex) 
                {
                    //This is where malformed regexes go
                    continue;
                }

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


        /// <summary>
        /// Code taken from: https://www.codeproject.com/Articles/36377/Validating-Credit-Card-Numbers
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        private static bool IsCardNumberValid(string cardNumber)
        {
            int i, checkSum = 0;

            // Compute checksum of every other digit starting from right-most digit
            for (i = cardNumber.Length - 1; i >= 0; i -= 2)
                checkSum += (cardNumber[i] - '0');

            // Now take digits not included in first checksum, multiple by two,
            // and compute checksum of resulting digits
            for (i = cardNumber.Length - 2; i >= 0; i -= 2)
            {
                int val = ((cardNumber[i] - '0') * 2);
                while (val > 0)
                {
                    checkSum += (val % 10);
                    val /= 10;
                }
            }

            // Number is valid if sum of both checksums MOD 10 equals 0
            return ((checkSum % 10) == 0);
        }
    }
}
