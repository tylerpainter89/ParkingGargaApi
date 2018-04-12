using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VehiklParkingGarageApi.Models;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VehiklParkingGarageApi.Controllers
{
    [Route("/tickets")]
    public class TicketsController : Controller
    {
        #region Member Variables

        private const int _lotSize = 10;
        private readonly TicketContext _context;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new TicketsController.
        /// </summary>
        /// <param name="context">An <see cref="TicketDbContext"/> representing the ticket database.</param>
        public TicketsController(TicketContext context)
        {
            _context = (TicketContext)context;
        }

        #endregion

        #region Http Requests

        /// <summary>
        /// Gets the tickets for all vehicles remaining in the lot.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the response.</returns>
        /// <remarks>Returns an <see cref="ObjectResult"/> of type <see cref="List{Ticket}<"/> representing the tickets for all cars in the lot.
        /// If the lot is empty a <see cref="NotFoundResult"/> is returned.</remarks>
        [HttpGet]
        public IActionResult GetAll()
        {
            IActionResult response = null;

            if (_context.Tickets.Count() == 0)
            {
                response = NotFound("Lot is empty.");
            }
            else
            {
                response = new ObjectResult(_context.Tickets.ToList());
            }
            return response;
        }

        /// <summary>
        /// Gets the bill for a ticket with the supplied <paramref name="ticketNumber"/>.
        /// </summary>
        /// <param name="ticketNumber">A <see cref="long"/> representing the ticket number to get.</param>
        /// <returns>An <see cref="IActionResult"/> representing the response.</returns>
        /// <remarks>Returns an <see cref="ObjectResult"/> of type <see cref="Bill"/> representing the bill for the requested ticket.
        /// If the ticket number can't be found a <see cref="NotFoundResult"/> is returned.</remarks>
        [HttpGet("{ticketNumber}")]
        public IActionResult GetBill(long ticketNumber)
        {
            IActionResult response = null;

            Ticket requestedTicket = _context.Tickets.FirstOrDefault(t => t.TicketNumber == ticketNumber);

            if (requestedTicket == null)
            {
                response = NotFound("Ticket number not found.");
            }
            else
            {
                Bill bill = new Bill(requestedTicket.TicketNumber, requestedTicket.EntryTime);

                response = new ObjectResult(bill);
            }

            return response;
        }

        /// <summary>
        /// Creates a new ticket.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the response.</returns>
        /// <remarks>
        /// Returns a <see cref="ObjectResult"/> or type <see cref="Ticket"/> is returned. 
        /// If the lot is at capacity, a <see cref="BadRequestResult"/> will be returned indicating so.
        /// </remarks>
        [HttpPost]
        public IActionResult PrintTicket()
        {
            IActionResult response = null;

            if (_context.Tickets.Count() == _lotSize)
            {
                response = BadRequest("Lot full. Please come back later.");
            }
            else
            {
                Ticket printedTicket = new Ticket { EntryTime = DateTime.Now };

                _context.Tickets.Add(printedTicket);
                _context.SaveChanges();

                response = new ObjectResult(printedTicket);
            }
            return response;
        }

        /// <summary>
        /// Pays the specified ticket.
        /// </summary>
        /// <param name="ticketNumber">A <see cref="long"/> representing the ticket number (id) of the ticket.</param>
        /// <param name="payment">A <see cref="TicketPayment"/> parsed from the JSON in the body of the POST request.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the request.</returns>
        [HttpPost("{ticketNumber}")]
        public IActionResult PayTicket(long ticketNumber, [FromBody] TicketPayment payment)
        {
            IActionResult response = null;

            Ticket paidTicket = _context.Tickets.FirstOrDefault(t => t.TicketNumber == ticketNumber);

            if (paidTicket == null)
            {
                response = NotFound("Ticket not found. Please check your ticket number and try again.");
            }
            else if (paidTicket.TicketNumber != ticketNumber || paidTicket.TicketNumber != payment.TicketNumber)
            {
                response = BadRequest("Could not process your ticket. Please ensure the ticket number is correct.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(payment.CreditCardNumber))
                {
                    response = BadRequest("Please provide a credit card number");
                }
                else
                {
                    if (IsCreditCardValid(payment.CreditCardNumber))
                    {
                        PayTicket(paidTicket);
                        response = Ok("Your ticket has been paid.");
                    }
                    else
                    {
                        response = BadRequest("Credit card could not be verified. Please try again.");
                    }
                }
            }

                return response;
            }

            #endregion

            #region Private Functions

            /// <summary>
            /// Determines whether a <paramref name="cardNumber"/> is a valid credit card number or not.
            /// </summary>
            /// <param name="cardNumber">A <see cref="string"/> representing the credit card number.</param>
            /// <returns>A <see cref="bool"/> indicating if the card is valid or not.</returns>
            private bool IsCreditCardValid(string cardNumber)
            {
                cardNumber = cardNumber.Replace("-", string.Empty).Replace(" ", string.Empty);
                Regex validationExpression = new Regex(@"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$");

                return validationExpression.IsMatch(cardNumber);
            }

            /// <summary>
            /// Reomves the paid ticket from the list of Tickets.
            /// </summary>
            /// <param name="paidTicket">A <see cref="Ticket"/>representing the ticket that has been paid.</param>
            private void PayTicket(Ticket paidTicket)
            {
                _context.Tickets.Remove(paidTicket);
                _context.SaveChanges();
            }

            #endregion
        }
    }
