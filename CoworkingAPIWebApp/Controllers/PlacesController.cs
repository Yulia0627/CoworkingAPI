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
    public class PlacesController : ControllerBase
    {
        private readonly CoworkingAPIContext _context;

        public PlacesController(CoworkingAPIContext context)
        {
            _context = context;
            if (!_context.Places.Any())
            {
                _context.Places.AddRange(
        new Place
        {
            Name = "Стіл біля вікна",
            Article = "OS-WIN-01",
            PricePerHour = 50.00m,
            Capacity = 1,
            ZoneId = 2,
            Description = "Робоче місце з гарним освітленням."
        },
        new Place
        {
            Name = "Кімната для переговорів",
            Article = "MR-KYIV-01",
            PricePerHour = 450.00m,
            Capacity = 10,
            ZoneId = 3,
            Description = "Для великих командних зустрічей."
        }
    );
                _context.SaveChanges();
            }
        }

        // GET: api/Places
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Place>>> GetPlaces(int? zoneId)
        {
            IQueryable<Place> query = _context.Places;
            query = query.Where(p => p.IsAvailable == true);
            if (zoneId.HasValue)
            {
                query = query.Where(p => p.ZoneId == zoneId);
            }
            return await query.ToListAsync();
        }

        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<Place>>> GetPlaces(string? article)
        {
            IQueryable<Place> query = _context.Places;
            if (article != null)
            {
                query = query.Where(p => p.Article == article);
            }
            return await query.ToListAsync();
        }

        // GET: api/Places/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Place>> GetPlace(int id)
        {
            var place = await _context.Places.FindAsync(id);

            if (place == null)
            {
                return NotFound();
            }

            return place;
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchPlace(int id, Place place)
        {
            if (id != place.Id)
            {
                return BadRequest();
            }

            var existingPlace = await _context.Places.FindAsync(id);
            if (existingPlace == null)
            {
                return NotFound();
            }

            if (place.Article != null)
            {
                existingPlace.Article = place.Article;
            }

            if (place.Name != null)
            {
                existingPlace.Name = place.Name;
            }

            if(place.PricePerHour != null)
            {
                existingPlace.PricePerHour = place.PricePerHour;
            }

            if (place.Capacity != null)
            {
                existingPlace.Capacity = place.Capacity;
            }

            if (place.Description != null)
            {
                existingPlace.Description = place.Description;
            }
            if (place.IsAvailable != null)
            {
                existingPlace.IsAvailable = place.IsAvailable;
            }
            if(place.ZoneId != null)
            {
                existingPlace.ZoneId = place.ZoneId;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaceExists(id))
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

        // PUT: api/Places/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlace(int id, Place place)
        {
            if (id != place.Id)
            {
                return BadRequest();
            }

            _context.Entry(place).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaceExists(id))
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

        // POST: api/Places
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Place>> PostPlace(Place place)
        {
            _context.Places.Add(place);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlace", new { id = place.Id }, place);
        }

        // DELETE: api/Places/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlace(int id)
        {
            var place = await _context.Places.FindAsync(id);
            if (place == null)
            {
                return NotFound();
            }

            _context.Places.Remove(place);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlaceExists(int id)
        {
            return _context.Places.Any(e => e.Id == id);
        }
    }
}
