namespace BarangaySkillExchangePlaform.Server.Controllers
{
    using BarangaySkillExchangePlaform.Server.Data;
    using BarangaySkillExchangePlaform.Server.DTOs;
    using BarangaySkillExchangePlaform.Server.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
    [Route("api/skill-offers")]
    public class SkillOffersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SkillOffersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<SkillOffer>>> GetSkillOffers(
            [FromQuery] string? search,
            [FromQuery] string? category)
        {
            var query = _context.SkillOffers
                .Include(skillOffer => skillOffer.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(skillOffer =>
                    skillOffer.Title.ToLower().Contains(search.ToLower()) ||
                    skillOffer.Description.ToLower().Contains(search.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(skillOffer => skillOffer.Category.ToLower() == category.ToLower());
            }

            return await query
                .OrderByDescending(skillOffer => skillOffer.CreatedAt)
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SkillOffer>> GetSkillOffer(int id)
        {
            var skillOffer = await _context.SkillOffers
                .Include(skillOffer => skillOffer.User)
                .FirstOrDefaultAsync(skillOffer => skillOffer.Id == id);

            if (skillOffer is null)
            {
                return NotFound(new { message = "Skill offer not found." });
            }

            return skillOffer;
        }

        [HttpPost]
        public async Task<ActionResult<SkillOffer>> CreateSkillOffer(CreateSkillOfferRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);

            if (user is null)
            {
                return BadRequest(new { message = "User does not exist." });
            }

            if (user.Status == "Suspended")
            {
                return BadRequest(new { message = "Suspended users cannot create skill offers." });
            }

            var skillOffer = new SkillOffer
            {
                UserId = request.UserId,
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                Availability = request.Availability,
                LocationPreference = request.LocationPreference,
                Status = "Active"
            };

            _context.SkillOffers.Add(skillOffer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSkillOffer), new { id = skillOffer.Id }, skillOffer);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSkillOffer(int id, UpdateSkillOfferRequest request)
        {
            var skillOffer = await _context.SkillOffers.FindAsync(id);

            if (skillOffer is null)
            {
                return NotFound(new { message = "Skill offer not found." });
            }

            skillOffer.Title = request.Title;
            skillOffer.Description = request.Description;
            skillOffer.Category = request.Category;
            skillOffer.Availability = request.Availability;
            skillOffer.LocationPreference = request.LocationPreference;
            skillOffer.Status = request.Status;

            await _context.SaveChangesAsync();

            return Ok(skillOffer);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSkillOffer(int id)
        {
            var skillOffer = await _context.SkillOffers.FindAsync(id);

            if (skillOffer is null)
            {
                return NotFound(new { message = "Skill offer not found." });
            }

            _context.SkillOffers.Remove(skillOffer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Skill offer deleted successfully." });
        }
    }
}
