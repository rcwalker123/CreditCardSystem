namespace CreditCardSystem.Interfaces
{
    public interface ICardValidator
    {
        public bool ValidateDate(DateTime date);
        public bool ValidateCVV(string cvv);
        public bool ValidateCardNumber(string cardNumber);

    }
}
