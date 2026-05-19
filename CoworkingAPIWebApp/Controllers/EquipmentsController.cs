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
    public class EquipmentsController : ControllerBase
    {
        private readonly CoworkingAPIContext _context;

        public EquipmentsController(CoworkingAPIContext context)
        {
            _context = context;
            if (!_context.Equipment.Any())
            {
                _context.Equipment.AddRange(
                    new Equipment
                    {
                        Name = "Монітор Dell 27\"",
                        Article = "DELL-U2723QE",
                        PricePerHour = 45.00m,
                        CategoryId = 1,
                        Description = "4K монітор для професійної роботи."
                    },
                    new Equipment
                    {
                        Name = "Проектор Epson",
                        Article = "PROJ-EPS-720",
                        PricePerHour = 120.00m,
                        CategoryId = 2,
                        Description = "Для презентацій та кінопоказів."
                    },
                    new Equipment
                    {
                        Name = "Кільцева лампа",
                        Article = "LED-RING-14",
                        PricePerHour = 30.00m,
                        CategoryId = 5,
                        IsAvailable = true
                    }
                );
                _context.SaveChanges();
            }
        }

        // GET: api/Equipments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipment()
        {
            return await _context.Equipment.ToListAsync();
        }

        // GET: api/Equipments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Equipment>> GetEquipment(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);

            if (equipment == null)
            {
                return NotFound();
            }

            return equipment;
        }

        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipment(string? article)
        {
            IQueryable<Equipment> query = _context.Equipment;
            if (article != null)
            {
                query = query.Where(e => e.Article == article);
            }
            return await query.ToListAsync();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchEquipment(int id, Equipment equip)
        {
            if (id != equip.Id)
            {
                return BadRequest();
            }

            var existingEquip = await _context.Equipment.FindAsync(id);
            if (existingEquip == null)
            {
                return NotFound();
            }

            if (equip.Article != null)
            {
                existingEquip.Article = equip.Article;
            }

            if (equip.Name != null)
            {
                existingEquip.Name = equip.Name;
            }

            if (equip.PricePerHour != null)
            {
                existingEquip.PricePerHour = equip.PricePerHour;
            }

            if (equip.Description != null)
            {
                existingEquip.Description = equip.Description;
            }
            if (equip.IsAvailable != null)
            {
                existingEquip.IsAvailable = equip.IsAvailable;
            }

            if (equip.CategoryId != null)
            {
                existingEquip.CategoryId = equip.CategoryId;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipmentExists(id))
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

        // PUT: api/Equipments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipment(int id, Equipment equipment)
        {
            if (id != equipment.Id)
            {
                return BadRequest();
            }

            _context.Entry(equipment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipmentExists(id))
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

        // POST: api/Equipments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Equipment>> PostEquipment(Equipment equipment)
        {
            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEquipment", new { id = equipment.Id }, equipment);
        }

        // DELETE: api/Equipments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquipmentExists(int id)
        {
            return _context.Equipment.Any(e => e.Id == id);
        }
    }
}
