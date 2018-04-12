using Microsoft.EntityFrameworkCore;

namespace VehiklParkingGarageApi.Models
{
    public class TicketContext : DbContext
    { 
   
        #region Properties

        /// <summary>
        /// Gets or sets the tickets database set.
        /// </summary>
        public DbSet<Ticket> Tickets { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new ticket context.
        /// </summary>
        /// <param name="options"><see cref="DbContextOptions"/> representing the database options.</param>
        public TicketContext(DbContextOptions<TicketContext> options) : base(options) { }

        #endregion
    }
}
