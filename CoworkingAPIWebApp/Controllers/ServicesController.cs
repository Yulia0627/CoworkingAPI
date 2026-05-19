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
    public class ServicesController : ControllerBase
    {
        private readonly CoworkingAPIContext _context;

        public ServicesController(CoworkingAPIContext context)
        {
            _context = context;

            if (!_context.Services.Any())
            {
                _context.Services.AddRange(
                    new Service { Name = "Оренда принтера", Price = 5.00m, IsAvailable = true },
                    new Service { Name = "Безлімітна кава", Price = 150.00m, IsAvailable = true },
                    new Service { Name = "Персональний локер", Price = 40.00m, IsAvailable = true },
                    new Service { Name = "Оренда ноутбука", Price = 350.00m, IsAvailable = true }
                );
                _context.SaveChanges();
            }
        }

        // GET: api/Services
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            return await _context.Services.ToListAsync();
        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            return service;
        }

        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<Service>>> GetService(string name)
        {
           IQueryable<Service> query = _context.Services;

            if (name != null)
            {
                query = query.Where(s => s.Name == name);
            }

            return await query.ToListAsync();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchService(int id, Service service)
        {
            if (id != service.Id)
            {
                return BadRequest();
            }

            var existingService = await _context.Services.FindAsync(id);
            if (existingService == null)
            {
                return NotFound();
            }


            if (service.Name != null)
            {
                existingService.Name = service.Name;
            }

            if (service.Price != null)
            {
                existingService.Price = service.Price;
            }

            if (service.IsAvailable != null)
            {
                existingService.IsAvailable = service.IsAvailable;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
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

        // PUT: api/Services/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutService(int id, Service service)
        {
            if (id != service.Id)
            {
                return BadRequest();
            }

            _context.Entry(service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
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

        // POST: api/Services
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Service>> PostService(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetService", new { id = service.Id }, service);
        }

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
