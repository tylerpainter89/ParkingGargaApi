namespace VehiklParkingGarageApi.Models
{
    public class TicketPayment
    {
        #region Properties

        /// <summary>
        /// A <see cref="long"/> representing the unique ticket number (id) of the ticket being paid.
        /// </summary>
        public long TicketNumber { get; set; }

        /// <summary>
        /// A <see cref="string"/> representing the credit card number being used to pay the ticket.
        /// </summary>
        public string CreditCardNumber { get; set; }
        
        #endregion
    }
}
