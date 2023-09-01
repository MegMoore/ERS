using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERS.Data;
using ERS.Models;
using Azure.Core;

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
        private async Task UpdateEmployeeExpensesDueAndPaid(int id)
        {
            var resPaid = (from exp in _context.Expenses
                           join emp in _context.Employees
                           on exp.EmployeeID equals emp.Id
                           where emp.Id == id && exp.Status == "PAID"
                           select new
                           {
                               exp.Total
                           }).Sum(x => x.Total);
            var resDue = (from exp in _context.Expenses
                          join emp in _context.Employees
                          on exp.EmployeeID equals emp.Id
                          where emp.Id == id && exp.Status != "PAID"
                          select new
                          {
                              exp.Total
                          }).Sum(x => x.Total);
            var employee = await _context.Employees.FindAsync(id);
            employee.ExpensesPaid = resPaid;
            employee.ExpensesDue = resDue;
            await _context.SaveChangesAsync(); 
        }

        // GET: api/Expenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
        {
          if (_context.Expenses == null)
          {
              return NotFound();
          }
            return await _context.Expenses.Include(x=>x.Employee).ToListAsync();
        }

        // GET: api/Expenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
          if (_context.Expenses == null)
          {
              return NotFound();
          }
            var expense = await _context.Expenses.Include(x=>x.Employee).Where(x=>x.ID==id).FirstOrDefaultAsync();

            if (expense == null)
            {
                return NotFound();
            }

            return expense;
        }

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

        //-----------------------------------------------------------------------------------------------------------------
        [HttpPut("review/{id}")]
        public async Task<IActionResult> Review(int id, Expense E) {
            if (E.Total <= 75) {
                E.Status = "APPROVED";
                var emp = await _context.Employees.FindAsync(id);
                E.Employee = emp;
                E.Employee!.ExpensesDue += E.Total;
                return await PutExpense(id, E);
            }
            E.Status = "REVIEW";
            return await PutExpense(id, E);
        }

        [HttpPut("approve/{id}")]
        public async Task<IActionResult> Approve(int id, Expense E) {
            E.Status = "APPROVED";
            var emp = await _context.Employees.FindAsync(id);
            E.Employee = emp;
            E.Employee!.ExpensesDue += E.Total;
            return await PutExpense(id, E);
        }

        [HttpPut("reject/{id}")]
        public async Task<IActionResult> Reject(int id, Expense E) {
                E.Status = "REJECTED";
                return await PutExpense(id, E);
        }



        //-------------------------------------------------------------------------------------------------------------------
        private bool ExpenseExists(int id)
        {
            return (_context.Expenses?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
