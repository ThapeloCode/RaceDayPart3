using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/enrolments
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Enrolment>>> GetEnrolments()
        {
            return await _context.Enrolments
                .Include(e => e.User)
                .Include(e => e.Event)
                .Include(e => e.Category)
                .ToListAsync();
        }

        // GET: api/enrolments/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Enrolment>> GetEnrolment(int id)
        {
            var enrolment = await _context.Enrolments
                .Include(e => e.User)
                .Include(e => e.Event)
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.EnrolmentId == id);

            if (enrolment == null) return NotFound();
            return enrolment;
        }

        // POST: api/enrolments (Participant or Organiser)
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Enrolment>> CreateEnrolment([FromBody] Enrolment enrolment)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null)
            {
                enrolment.UserId = int.Parse(userIdClaim);
            }

            enrolment.EnrolmentDate = DateTime.UtcNow;

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEnrolment), new { id = enrolment.EnrolmentId }, enrolment);
        }

        // DELETE: api/enrolments/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEnrolment(int id)
        {
            var enrolment = await _context.Enrolments.FindAsync(id);
            if (enrolment == null) return NotFound();

            _context.Enrolments.Remove(enrolment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}