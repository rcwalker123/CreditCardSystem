using CreditCardSystem.Interfaces;
using System.Text.RegularExpressions;

namespace CreditCardSystem.ProcessManagers
{
    public class CardValidator : ICardValidator
    {
        public bool ValidateCardNumber(string cardNumber)
        {
            throw new NotImplementedException();
        }

        public bool ValidateCVV(string cvv)
        {
            return Regex.IsMatch(cvv, "^[0-9][3,4]$");
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
