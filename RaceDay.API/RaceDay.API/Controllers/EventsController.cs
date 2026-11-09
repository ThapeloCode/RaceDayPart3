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
            return await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .Include(e => e.Enrolments)
                .ToListAsync();
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .Include(e => e.Enrolments)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventItem == null) return NotFound();
            return eventItem;
        }

        // GET: api/events/organiser/{organiserId}
        [HttpGet("organiser/{organiserId}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetOrganiserEvents(int organiserId)
        {
            var events = await _context.Events
                .Where(e => e.OrganiserId == organiserId)
                .Include(e => e.Categories)
                .Include(e => e.Enrolments)
                .ToListAsync();

            return Ok(events);
        }

        // POST: api/events
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event eventModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify that the Organiser exists in the Users table
            var organiserExists = await _context.Users.AnyAsync(u => u.UserId == eventModel.OrganiserId);
            if (!organiserExists)
            {
                return BadRequest(new { message = "Invalid Organiser ID. User does not exist." });
            }

            _context.Events.Add(eventModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = eventModel.EventId }, eventModel);
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
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

        // GET: api/events/{id}/categories
        [HttpGet("{id}/categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetEventCategories(int id)
        {
            var categories = await _context.Categories
                .Where(c => c.EventId == id)
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/events/{id}/categories
        [HttpPost("{id}/categories")]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Category>> AddCategory(int id, [FromBody] Category category)
        {
            var eventExists = await _context.Events.AnyAsync(e => e.EventId == id);
            if (!eventExists) return NotFound(new { message = "Event not found." });

            category.EventId = id;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // GET: api/events/{id}/enrolments
        [HttpGet("{id}/enrolments")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Enrolment>>> GetEventEnrolments(int id)
        {
            var enrolments = await _context.Enrolments
                .Where(e => e.EventId == id)
                .Include(e => e.User)
                .Include(e => e.Category)
                .ToListAsync();

            return Ok(enrolments);
        }
    }
}