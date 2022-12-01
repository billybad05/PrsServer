using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrsServer.Models;

namespace PrsServer.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class RequestlinesController : ControllerBase {
        private readonly PrsDbContext _context;

        public RequestlinesController(PrsDbContext context) {
            _context = context;
        }

        // GET: api/Requestlines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Requestline>>> GetRequestlines() {
            return await _context.Requestlines.ToListAsync();
        }

        // GET: api/Requestlines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Requestline>> GetRequestline(int id) {
            var requestline = await _context.Requestlines.FindAsync(id);

            if (requestline == null) {
                return NotFound();
            }

            return requestline;
        }

        // PUT: api/Requestlines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestline(int id, Requestline requestline) {
            if (id != requestline.Id) {
                return BadRequest();
            }

            _context.Entry(requestline).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
                await RecalcRequestTotal(requestline.RequestId);
            }
            catch (DbUpdateConcurrencyException) {
                if (!RequestlineExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Requestlines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Requestline>> PostRequestline(Requestline requestline) {
            _context.Requestlines.Add(requestline);
            await _context.SaveChangesAsync();
            await RecalcRequestTotal(requestline.RequestId);
            return CreatedAtAction("GetRequestline", new { id = requestline.Id }, requestline);
        }

        // DELETE: api/Requestlines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequestline(int id) {
            var requestline = await _context.Requestlines.FindAsync(id);
            if (requestline == null) {
                return NotFound();
            }

            _context.Requestlines.Remove(requestline);
            await _context.SaveChangesAsync();
            await RecalcRequestTotal(requestline.RequestId);
            return NoContent();
        }

        private async Task<IActionResult> RecalcRequestTotal(int requestId) {
            var req = await _context.Requests.FindAsync(requestId);
            if (req is null) {
                throw new Exception("Recalc failed!");
            }
            req.Total = (from rl in _context.Requestlines
                         join p in _context.Products
                         on rl.ProductId equals p.Id
                         where rl.RequestId == requestId
                         select new {
                             Subtotal = rl.Quantity * p.Price
                         }).Sum(x => x.Subtotal);

            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool RequestlineExists(int id) {
            return _context.Requestlines.Any(e => e.Id == id);
        }
    }
}