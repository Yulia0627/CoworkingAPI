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
    public class ZoneCategoriesController : ControllerBase
    {
        private readonly CoworkingAPIContext _context;

        public ZoneCategoriesController(CoworkingAPIContext context)
        {
            _context = context;
            if (!_context.ZoneCategories.Any()) {
                _context.ZoneCategories.AddRange(
                    new ZoneCategory { Name = "Silent Zone", 
                                       Description = "Зона абсолютної тиші. Ідеально підходить для глибокого " +
                                                     "навчання, написання коду або зосередженої роботи без дзвінків." },
                    new ZoneCategory { Name = "Open Space", 
                                       Description = "Простора зона з великими столами. Чудове місце для тих, " +
                                       "              хто любить працювати в динамічній атмосфері офісу." },
                    new ZoneCategory { Name = "Meeting Room", 
                                       Description = "Закриті кімнати, обладнані екранами та фліпчартами для " +
                                                      "проведення командних нарад, брейнштормів або зум-колів." },
                    new ZoneCategory { Name = "Lounge Area", 
                                       Description = "Зона з диванами та кріслами-мішками. Підходить для неформального " +
                                                      "спілкування, читання або короткого відпочинку за кавою." },
                    new ZoneCategory { Name = "Private Office", 
                                       Description = "Окремі кабінети для команд або осіб, які потребують повної приватності " +
                                                     "та власного закритого простору." },
                    new ZoneCategory { Name = "Skype Booth", 
                                       Description = "Маленькі звукоізольовані кабінки для приватних телефонних розмов або " +
                                                      "відеодзвінків." }

                    );
                _context.SaveChanges();
            }
        }

        // GET: api/ZoneCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ZoneCategory>>> GetZoneCategories()
        {
            return await _context.ZoneCategories.ToListAsync();
        }

        // GET: api/ZoneCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ZoneCategory>> GetZoneCategory(int id)
        {
            var zoneCategory = await _context.ZoneCategories.FindAsync(id);

            if (zoneCategory == null)
            {
                return NotFound();
            }

            return zoneCategory;
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchZoneCategory(int id, ZoneCategory zoneCategory)
        {
            if (id != zoneCategory.Id)
            {
                return BadRequest();
            }

           var existingCat = await _context.ZoneCategories.FindAsync(id);
            if (existingCat == null)
            {
                return NotFound();
            }

            if (zoneCategory.Name != null)
            {
                existingCat.Name = zoneCategory.Name;
            }

            if (zoneCategory.Description != null)
                {
                    existingCat.Description = zoneCategory.Description;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZoneCategoryExists(id))
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

        // PUT: api/ZoneCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutZoneCategory(int id, ZoneCategory zoneCategory)
        {
            if (id != zoneCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(zoneCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZoneCategoryExists(id))
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

        // POST: api/ZoneCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ZoneCategory>> PostZoneCategory(ZoneCategory zoneCategory)
        {
            _context.ZoneCategories.Add(zoneCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetZoneCategory", new { id = zoneCategory.Id }, zoneCategory);
        }

        // DELETE: api/ZoneCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteZoneCategory(int id)
        {
            var zoneCategory = await _context.ZoneCategories.FindAsync(id);
            if (zoneCategory == null)
            {
                return NotFound();
            }

            _context.ZoneCategories.Remove(zoneCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ZoneCategoryExists(int id)
        {
            return _context.ZoneCategories.Any(e => e.Id == id);
        }
    }
}
