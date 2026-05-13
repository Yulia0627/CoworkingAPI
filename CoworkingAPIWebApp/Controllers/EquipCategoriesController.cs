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
    public class EquipCategoriesController : ControllerBase
    {
        private readonly CoworkingAPIContext _context;

        public EquipCategoriesController(CoworkingAPIContext context)
        {
            _context = context;

            if (!_context.EquipCategories.Any())
            {
                _context.EquipCategories.AddRange(
                    new EquipCategory { Name = "Комп'ютерна периферія" },
                    new EquipCategory { Name = "Мультимедіа" },
                    new EquipCategory { Name = "Оргтехніка" },
                    new EquipCategory { Name = "Зарядні пристрої" },
                    new EquipCategory { Name = "Освітлення" }
                );
                _context.SaveChanges();
            }
        }

        // GET: api/EquipCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipCategory>>> GetEquipCategories()
        {
            return await _context.EquipCategories.ToListAsync();
        }

        // GET: api/EquipCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EquipCategory>> GetEquipCategory(int id)
        {
            var equipCategory = await _context.EquipCategories.FindAsync(id);

            if (equipCategory == null)
            {
                return NotFound();
            }

            return equipCategory;
        }

        // PUT: api/EquipCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipCategory(int id, EquipCategory equipCategory)
        {
            if (id != equipCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(equipCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipCategoryExists(id))
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

        // POST: api/EquipCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EquipCategory>> PostEquipCategory(EquipCategory equipCategory)
        {
            _context.EquipCategories.Add(equipCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEquipCategory", new { id = equipCategory.Id }, equipCategory);
        }

        // DELETE: api/EquipCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipCategory(int id)
        {
            var equipCategory = await _context.EquipCategories.FindAsync(id);
            if (equipCategory == null)
            {
                return NotFound();
            }

            _context.EquipCategories.Remove(equipCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquipCategoryExists(int id)
        {
            return _context.EquipCategories.Any(e => e.Id == id);
        }
    }
}
