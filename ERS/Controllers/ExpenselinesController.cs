﻿using System;
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

    public class ExpenselinesController : ControllerBase
    {
        private readonly ERSContext _context;


        public ExpenselinesController(ERSContext context)
        {
            _context = context;
        }
        private async Task RecalculateExpenseTotal(int expenseId)
        {
         var result = (from e in _context.Expenses
                          join el in _context.Expenselines
                          on e.ID equals el.ExpenseId
                          join i in _context.Items
                          on el.ItemId equals i.Id
                          where e.ID == expenseId
                          select new
                          {
                              lineTotals = e.Total + el.Quantity
                          }).Sum(x => x.lineTotals);
            var expense = await _context.Expenses.FindAsync(expenseId);
            expense!.Total = result;
            await _context.SaveChangesAsync(); 
    }

        // GET: api/Expenselines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expenseline>>> GetExpenselines()
        {
          if (_context.Expenselines == null)
          {
              return NotFound();
          }
            return await _context.Expenselines.ToListAsync();
        }

        // GET: api/Expenselines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expenseline>> GetExpenselines(int id)
        {
          if (_context.Expenselines == null)
          {
              return NotFound();
          }
            var expenselines = await _context.Expenselines.FindAsync(id);
            

            if (expenselines == null)
            {
                return NotFound();
            }
            return expenselines;
        }

        // PUT: api/Expenselines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpenselines(int id, Expenseline expenselines)
        {
            if (id != expenselines.Id)
            {
                return BadRequest();
            }

            _context.Entry(expenselines).State = EntityState.Modified;
            if(expenselines.Quantity <= 0)
            {
                return Problem("Quantity Cannot be less than 1!");
            }
            try
            {
                await _context.SaveChangesAsync();
                await RecalculateExpenseTotal(expenselines.ExpenseId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenselinesExists(id))
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

        // POST: api/Expenselines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Expenseline>> PostExpenselines(Expenseline expenselines)
        {
          if (_context.Expenselines == null)
          {
              return Problem("Entity set 'ERSContext.Expenselines'  is null.");
          }
          if (expenselines.Quantity <= 0)
          {
              return Problem("Quantity Cannot be less than 1!");
          }
            _context.Expenselines.Add(expenselines);
            await _context.SaveChangesAsync();
            await RecalculateExpenseTotal(expenselines.ExpenseId);
            return CreatedAtAction("GetExpenselines", new { id = expenselines.Id }, expenselines);
        }

        // DELETE: api/Expenselines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpenselines(int id)
        {
            if (_context.Expenselines == null)
            {
                return NotFound();
            }
            var expenselines = await _context.Expenselines.FindAsync(id);
            if (expenselines == null)
            {
                return NotFound();
            }
            var expenseId = expenselines.ExpenseId;

            _context.Expenselines.Remove(expenselines);
            await _context.SaveChangesAsync();
            await RecalculateExpenseTotal(expenseId);
            return NoContent();
        }

        private bool ExpenselinesExists(int id)
        {
            return (_context.Expenselines?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
