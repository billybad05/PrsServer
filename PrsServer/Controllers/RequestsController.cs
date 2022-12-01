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
    public class RequestsController : ControllerBase {

        public static string NEW = "NEW";
        public static string MODIFIED = "MODIFIED";
        public static string APPROVED = "APPROVED";
        public static string REJECTED = "REJECTED";
        public static string REVIEW = "REVIEW";
        public static string PAID = "PAID";

        private readonly PrsDbContext _context;

        public RequestsController(PrsDbContext context) {
            _context = context;
        }

        [HttpGet("review/{userId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsInReview(int userId) {
            var requests = await _context.Requests
                 .Where(y => (y.Status == REVIEW) && y.UserId != userId)
                 .Include(x => x.User)
                 .ToListAsync();
            return requests;
        }
        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests() {

            return await _context.Requests.Include(x => x.User)
                                          .Include(x => x.Requestlines)
                                          .ThenInclude(x => x.Product)
                                          .ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id) {

            var request = await _context.Requests.Include(x => x.User)
                                                 .Include(x => x.Requestlines)
                                                 .ThenInclude(x => x.Product)
                                                 .SingleOrDefaultAsync(x => x.Id == id);

            if (request == null) {
                return NotFound();
            }

            return request;
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request) {
            if (id != request.Id) {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!RequestExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("review")]
        public async Task<IActionResult> ReviewRequest(Request request) {

            request.Status = (request.Total <= 50) ? APPROVED : REVIEW;
            return await PutRequest(request.Id, request);
        }

        [HttpPut("approve")]
        public async Task<IActionResult> ApproveRequest(Request request) {

            request.Status = APPROVED;
            return await PutRequest(request.Id, request);

        }

        [HttpPut("reject")]
        public async Task<IActionResult> RejectRequest(Request request) {

            request.Status = REJECTED;
            return await PutRequest(request.Id, request);

        }

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request) {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id) {
            var request = await _context.Requests.FindAsync(id);
            if (request == null) {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RequestExists(int id) {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
