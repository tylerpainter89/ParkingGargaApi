using System;

namespace VehiklParkingGarageApi.Models
{
    public class Bill
    {
        #region Member Variables

        private const int _startingRate = 3;
        private const double _multiplier = 1.5;

        #endregion

        #region Properties

        /// <summary>
        /// A <see cref="long"/> representing the unique ticket number (id) of the ticket the bill is for.
        /// </summary>
        public long TicketNumber { get; set; }

        /// <summary>
        /// A <see cref="DateTime"/> representing the time the vehicle entered the lot.
        /// </summary>
        public DateTime EntryTime { get; set; }

        /// <summary>
        /// A <see cref="DateTime"/> representing the time the vehicle left the lot.
        /// </summary>
        public DateTime ExitTime { get; set; }

        /// <summary>
        /// A <see cref="TimeSpan"/> representing the amount of time the car was in the lot.
        /// </summary>
        public TimeSpan TimeParked { get; set; }

        /// <summary>
        /// A <see cref="double"/> representing the amount owing on the ticket.
        /// </summary>
        public double AmountOwing { get; set; }

        #endregion

        #region Constructors

        public Bill(long ticketNumber, DateTime entryTime)
        {
            this.TicketNumber = ticketNumber;
            this.EntryTime = entryTime;
            this.ExitTime = DateTime.Now;
            this.TimeParked = this.ExitTime - this.EntryTime;
            this.AmountOwing = CalculateAmountOwing();
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Calculates the amount owing on a ticket as of when the function is called.
        /// </summary>
        /// <returns>A <see cref="double"/> representing the amount owing.</returns>
        private double CalculateAmountOwing()
        {
            double amount = 0.0;

            if (this.EntryTime <= DateTime.Now.AddHours(-6))
            {
                // All day rate
                amount = _startingRate * Math.Pow(_multiplier, 3);
            }
            else if (this.EntryTime <= DateTime.Now.AddHours(-3))
            {
                // 6 hour rate
                amount = _startingRate * Math.Pow(_multiplier, 2);
            }
            else if (this.EntryTime <= DateTime.Now.AddHours(-1))
            {
                //3 hour rate
                amount = _startingRate * _multiplier;
            }
            else
            {
                //hour rate
                amount = _startingRate;
            }

            return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        }

        #endregion
    }
}
