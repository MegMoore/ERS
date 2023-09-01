using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERS.Data;
using ERS.Models;

namespace ERS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpoyeesController : ControllerBase
    {
        private readonly ERSContext _context;

        public EmpoyeesController(ERSContext context)
        {
            _context = context;
        }

        // GET: api/Empoyees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmpoyee()
        {
          if (_context.Employees == null)
          {
              return NotFound();
          }
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Empoyees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmpoyee(int id)
        {
          if (_context.Employees == null)
          {
              return NotFound();
          }
            var empoyee = await _context.Employees.FindAsync(id);

            if (empoyee == null)
            {
                return NotFound();
            }

            return empoyee;
        }

        // PUT: api/Empoyees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpoyee(int id, Employee empoyee)
        {
            if (id != empoyee.Id)
            {
                return BadRequest();
            }

            _context.Entry(empoyee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpoyeeExists(id))
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

        // POST: api/Empoyees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmpoyee(Employee empoyee)
        {
          if (_context.Employees == null)
          {
              return Problem("Entity set 'ERSContext.Empoyee'  is null.");
          }
            _context.Employees.Add(empoyee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmpoyee", new { id = empoyee.Id }, empoyee);
        }

        // DELETE: api/Empoyees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpoyee(int id)
        {
            if (_context.Employees == null)
            {
                return NotFound();
            }
            var empoyee = await _context.Employees.FindAsync(id);
            if (empoyee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(empoyee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmpoyeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
