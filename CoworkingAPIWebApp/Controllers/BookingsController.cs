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
    public class BookingsController : ControllerBase
    {
        private readonly CoworkingAPIContext _context;

        public BookingsController(CoworkingAPIContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings
                .Include(b => b.Services)   
                .Include(b => b.Equipment)  
                .Include(b => b.Place)      
                .ToListAsync();
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Services)
                .Include(b => b.Equipment)
                .Include(b => b.Place)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        // PUT: api/Bookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            if (id != booking.Id)
            {
                return BadRequest();
            }

            var existingBooking = await _context.Bookings
                .Include(b => b.Services)
                .Include(b => b.Equipment)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBooking == null) return NotFound();

            bool timeChanged = existingBooking.StartTime != booking.StartTime || existingBooking.EndTime != booking.EndTime;
            bool placeChanged = existingBooking.PlaceId != booking.PlaceId;

            var oldServiceIds = existingBooking.Services.Select(s => s.Id).OrderBy(x => x);
            var newServiceIds = (booking.Services ?? new List<Service>()).Select(s => s.Id).OrderBy(x => x);
            bool serviceChanged = !oldServiceIds.SequenceEqual(newServiceIds);

            var oldEquipIds = existingBooking.Equipment.Select(e => e.Id).OrderBy(x => x);
            var newEquipIds = (booking.Equipment ?? new List<Equipment>()).Select(e => e.Id).OrderBy(x => x);
            bool equipChanged = !oldEquipIds.SequenceEqual(newEquipIds);

            var overlap = await _context.Bookings.AnyAsync(b => b.PlaceId == booking.PlaceId && b.StatusId != 3 &&
                                                               b.Id != id &&
                                                               b.StartTime < booking.EndTime && b.EndTime > booking.StartTime);
            if (overlap) return BadRequest("Це місце вже зайняте на цей час.");

            var dbPlace = await _context.Places.FindAsync(booking.PlaceId);
            if (dbPlace == null || dbPlace.IsAvailable == false)
            {
                return BadRequest("Місце недоступне для бронювання!");
            }
            if (dbPlace.Capacity < booking.ParticipantsCount)
            {
                return BadRequest($"Це місце розраховане максимум на {dbPlace.Capacity} осіб.");
            }

            if (timeChanged || placeChanged || serviceChanged || equipChanged)
            {
                var hours = (decimal)(booking.EndTime - booking.StartTime).TotalHours;
                if (hours <= 0)
                {
                    return BadRequest("Час завершення має бути пізніше часу початку.");
                }

                if (booking.StartTime < DateTime.Now)
                {
                    return BadRequest("Не можна створювати бронювання на минулий час!");
                }

                decimal newTotalPrice = hours * dbPlace.PricePerHour;

                existingBooking.Services.Clear();
                if (booking.Services != null)
                {
                    foreach (var service in booking.Services)
                    {
                        var dbService = await _context.Services.FindAsync(service.Id);
                        if (dbService != null)
                        {
                            newTotalPrice += dbService.Price;
                            existingBooking.Services.Add(dbService);
                        }
                    }
                }

                existingBooking.Equipment.Clear();
                if (booking.Equipment != null)
                {
                    foreach (var equip in booking.Equipment)
                    {
                        var dbEquip = await _context.Equipment.FindAsync(equip.Id);
                        if (dbEquip != null)
                        {
                            newTotalPrice += (hours * dbEquip.PricePerHour);
                            existingBooking.Equipment.Add(dbEquip);
                        }
                    }
                }


                _context.Entry(existingBooking).CurrentValues.SetValues(booking);


                existingBooking.TotalPrice = newTotalPrice;
                existingBooking.StatusId = 2;
            }


            try
            {

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
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

        // POST: api/Bookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            var overlap = await _context.Bookings.AnyAsync(b => b.PlaceId == booking.PlaceId && b.StatusId != 3 &&
                                                           b.StartTime < booking.EndTime && b.EndTime > booking.StartTime);
            if (overlap) return BadRequest("Це місце вже зайняте на цей час.");
            var dbPlace = await _context.Places.FindAsync(booking.PlaceId);
            if (dbPlace == null || dbPlace.IsAvailable == false)
            {
                return BadRequest("Місце недоступне для бронювання!");
            }
            if (dbPlace.Capacity<booking.ParticipantsCount)
            {
                return BadRequest($"Це місце розраховане максимум на {dbPlace.Capacity} осіб.");
            }
            var hours = (decimal)(booking.EndTime - booking.StartTime).TotalHours;

            if (hours <= 0)
            {
                return BadRequest("Час завершення має бути пізніше часу початку.");
            }

            if (booking.StartTime < DateTime.Now)
            {
                return BadRequest("Не можна створювати бронювання на минулий час!");
            }

            booking.TotalPrice = hours * dbPlace.PricePerHour;
            
            if (booking.Services != null)
            {
                var servicesFromDb = new List<Service>();
                foreach (var service in booking.Services)
                {
                    var dbService = await _context.Services.FindAsync(service.Id);
                    if (dbService != null)
                    {
                        booking.TotalPrice += dbService.Price;
                        servicesFromDb.Add(dbService); 
                    }
                }
                booking.Services = servicesFromDb; 
            }

        
            if (booking.Equipment != null)
            {
                var equipmentFromDb = new List<Equipment>();
                foreach (var equip in booking.Equipment)
                {
                    var dbEquip = await _context.Equipment.FindAsync(equip.Id);
                    if (dbEquip != null)
                    {
                        booking.TotalPrice += (hours * dbEquip.PricePerHour);
                        equipmentFromDb.Add(dbEquip); 
                    }
                }
                booking.Equipment = equipmentFromDb; 
            }
            booking.StatusId = 1;
            booking.CreatedAt = DateTime.Now;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.StatusId = 3;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
