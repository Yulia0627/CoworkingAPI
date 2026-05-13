using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoworkingAPIWebApp.Models;

namespace CoworkingAPIWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingStatusesController : ControllerBase
    {
        private readonly CoworkingAPIContext _context;

        public BookingStatusesController(CoworkingAPIContext context)
        {
            _context = context;
            if (!_context.BookingStatuses.Any()) {
                _context.BookingStatuses.AddRange(
                    new BookingStatus { Name = "Бронювання створено" },
                    new BookingStatus { Name = "Бронювання оновлено" },
                    new BookingStatus { Name = "Бронювання скасовано" }
                    );
                _context.SaveChanges();
            }
        }

        // GET: api/BookingStatuses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingStatus>>> GetBookingStatuses()
        {
            return await _context.BookingStatuses.ToListAsync();
        }

        // GET: api/BookingStatuses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingStatus>> GetBookingStatus(int id)
        {
            var bookingStatus = await _context.BookingStatuses.FindAsync(id);

            if (bookingStatus == null)
            {
                return NotFound();
            }

            return bookingStatus;
        }

        // PUT: api/BookingStatuses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookingStatus(int id, BookingStatus bookingStatus)
        {
            if (id != bookingStatus.Id)
            {
                return BadRequest();
            }

            _context.Entry(bookingStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingStatusExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BookingStatuses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookingStatus>> PostBookingStatus(BookingStatus bookingStatus)
        {
            _context.BookingStatuses.Add(bookingStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookingStatus", new { id = bookingStatus.Id }, bookingStatus);
        }

        // DELETE: api/BookingStatuses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookingStatus(int id)
        {
            var bookingStatus = await _context.BookingStatuses.FindAsync(id);
            if (bookingStatus == null)
            {
                return NotFound();
            }

            _context.BookingStatuses.Remove(bookingStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingStatusExists(int id)
        {
            return _context.BookingStatuses.Any(e => e.Id == id);
        }
    }
}
