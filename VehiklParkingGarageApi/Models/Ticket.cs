using System;
using System.ComponentModel.DataAnnotations;

namespace VehiklParkingGarageApi.Models
{
    public class Ticket
    {
        #region Properties

        /// <summary>
        /// A <see cref="long"/> representing the unique ticket number (id) of the ticket.
        /// </summary>
        [Key]
        public long TicketNumber { get; set; }

        /// <summary>
        /// A <see cref="DateTime"/> representing the time the vehicle entered the lot.
        /// </summary>
        public DateTime EntryTime { get; set; }

        #endregion
    }
}
