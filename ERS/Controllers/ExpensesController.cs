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
    public class ExpensesController : ControllerBase
    {
        private readonly ERSContext _context;

        public ExpensesController(ERSContext context)
        {
            _context = context;
        }

        // GET: api/Expenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
        {
          if (_context.Expenses == null)
          {
              return NotFound();
          }
            return await _context.Expenses.ToListAsync();
        }

        // GET: api/Expenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
          if (_context.Expenses == null)
          {
              return NotFound();
          }
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null)
            {
                return NotFound();
            }

            return expense;
        }

        //Added Method
        //************************************************//

        //[GEt: /api/expenses/approved]

        [HttpGet("APPROVED")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetApprovedExpenses()
        {
            if (_context.Expenses == null)
            {
                return NotFound();
            }
            return await _context.Expenses
                          .Where(x => x.Status == "APPROVED")
                          .Include(x => x.Employee)
                          .ToListAsync();
        }


        //[PUT: /api/expenses/pay/{expenseId}]
        [HttpPut("{expenseId}")]
        public async Task<IActionResult> PayExpense(int id, Expense expense)
        {
            expense.Status = "PAID";
            return await PutExpense(id, expense);

            var paid = 

        }



        // Update Employees Expenses Due and Paid
        private async Task UpdateEmployeeExpenseDueAndPaid(int id)
        {
            var total = (from e in _context.Expenses
                         join el in _context.Expenselines
                             on e.ID equals el.ExpenseId
                         join i in _context.Items
                             on el.ItemId equals i.Id
                         where e.ID == id
                         select new
                         {
                             ExpenseTotal = el.Quantity * i.Price
                         }).Sum(x => x.ExpenseTotal);
            var order = await _context.Expenses.FindAsync(id);
            order!.Total = total;
            await _context.SaveChangesAsync();


        }

        //************************************************//

        // PUT: api/Expenses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpense(int id, Expense expense)
        {
            if (id != expense.ID)
            {
                return BadRequest();
            }

            _context.Entry(expense).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseExists(id))
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

        // POST: api/Expenses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(Expense expense)
        {
          if (_context.Expenses == null)
          {
              return Problem("Entity set 'ERSContext.Expenses'  is null.");
          }
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExpense", new { id = expense.ID }, expense);
        }

        // DELETE: api/Expenses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            if (_context.Expenses == null)
            {
                return NotFound();
            }
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExpenseExists(int id)
        {
            return (_context.Expenses?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
