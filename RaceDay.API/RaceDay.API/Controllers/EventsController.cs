using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var events = await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .Include(e => e.Enrolments)
                .AsSplitQuery() // Eliminates Cartesian explosion warning & optimizes performance
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .Include(e => e.Enrolments)
                .Include(e => e.Results)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return Ok(eventItem);
        }

        // GET: api/events/organiser/5
        [HttpGet("organiser/{organiserId}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetOrganiserEvents(int organiserId)
        {
            var events = await _context.Events
                .Where(e => e.OrganiserId == organiserId)
                .Include(e => e.Categories)
                .Include(e => e.Enrolments)
                .AsSplitQuery()
                .ToListAsync();

            return Ok(events);
        }

        // POST: api/events
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event eventItem)
        {
            // Verify organiser exists
            var organiser = await _context.Users.FindAsync(eventItem.OrganiserId);
            if (organiser == null)
            {
                return BadRequest(new { message = "Invalid OrganiserId. User does not exist." });
            }

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = eventItem.EventId }, eventItem);
        }

        // DELETE: api/events/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/events/5/categories
        [HttpPost("{id}/categories")]
        [Authorize]
        public async Task<ActionResult<Category>> AddCategory(int id, [FromBody] Category category)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            category.EventId = id;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }
    }
}