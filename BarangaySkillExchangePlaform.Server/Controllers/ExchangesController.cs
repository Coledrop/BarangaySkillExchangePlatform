namespace BarangaySkillExchangePlaform.Server.Controllers
{
    using BarangaySkillExchangePlaform.Server.Data;
    using BarangaySkillExchangePlaform.Server.DTOs;
    using BarangaySkillExchangePlaform.Server.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    [ApiController]
    [Route("api/exchanges")]
    public class ExchangesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExchangesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Exchange>>> GetExchanges()
        {
            return await _context.Exchanges
                .Include(exchange => exchange.Requester)
                .Include(exchange => exchange.Provider)
                .Include(exchange => exchange.SkillOffer)
                .Include(exchange => exchange.ServiceRequest)
                .OrderByDescending(exchange => exchange.CreatedAt)
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Exchange>> GetExchange(int id)
        {
            var exchange = await _context.Exchanges
                .Include(exchange => exchange.Requester)
                .Include(exchange => exchange.Provider)
                .Include(exchange => exchange.SkillOffer)
                .Include(exchange => exchange.ServiceRequest)
                .FirstOrDefaultAsync(exchange => exchange.Id == id);

            if (exchange is null)
            {
                return NotFound(new { message = "Exchange not found." });
            }

            return exchange;
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<List<Exchange>>> GetUserExchanges(int userId)
        {
            return await _context.Exchanges
                .Include(exchange => exchange.Requester)
                .Include(exchange => exchange.Provider)
                .Include(exchange => exchange.SkillOffer)
                .Include(exchange => exchange.ServiceRequest)
                .Where(exchange => exchange.RequesterId == userId || exchange.ProviderId == userId)
                .OrderByDescending(exchange => exchange.CreatedAt)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Exchange>> CreateExchange(CreateExchangeRequest request)
        {
            if (request.RequesterId == request.ProviderId)
            {
                return BadRequest(new { message = "You cannot create an exchange with yourself." });
            }

            var requester = await _context.Users.FindAsync(request.RequesterId);
            var provider = await _context.Users.FindAsync(request.ProviderId);

            if (requester is null || provider is null)
            {
                return BadRequest(new { message = "Requester or provider does not exist." });
            }

            if (requester.Status == "Suspended" || provider.Status == "Suspended")
            {
                return BadRequest(new { message = "Suspended users cannot participate in exchanges." });
            }

            var exchange = new Exchange
            {
                RequesterId = request.RequesterId,
                ProviderId = request.ProviderId,
                SkillOfferId = request.SkillOfferId,
                ServiceRequestId = request.ServiceRequestId,
                Message = request.Message,
                ProposedSchedule = request.ProposedSchedule,
                Location = request.Location,
                Status = "Pending"
            };

            _context.Exchanges.Add(exchange);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExchange), new { id = exchange.Id }, exchange);
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateExchangeStatus(int id, UpdateExchangeStatusRequest request)
        {
            var exchange = await _context.Exchanges.FindAsync(id);

            if (exchange is null)
            {
                return NotFound(new { message = "Exchange not found." });
            }

            var allowedStatuses = new List<string>
        {
            "Pending",
            "Accepted",
            "Rejected",
            "Completed",
            "Cancelled"
        };

            if (!allowedStatuses.Contains(request.Status))
            {
                return BadRequest(new { message = "Invalid exchange status." });
            }

            exchange.Status = request.Status;

            if (request.Status == "Completed")
            {
                exchange.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(exchange);
        }

        [HttpPut("{id:int}/accept")]
        public async Task<IActionResult> AcceptExchange(int id)
        {
            var exchange = await _context.Exchanges.FindAsync(id);

            if (exchange is null)
            {
                return NotFound(new { message = "Exchange not found." });
            }

            exchange.Status = "Accepted";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exchange accepted successfully.", exchange });
        }

        [HttpPut("{id:int}/reject")]
        public async Task<IActionResult> RejectExchange(int id)
        {
            var exchange = await _context.Exchanges.FindAsync(id);

            if (exchange is null)
            {
                return NotFound(new { message = "Exchange not found." });
            }

            exchange.Status = "Rejected";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exchange rejected successfully.", exchange });
        }

        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> CompleteExchange(int id)
        {
            var exchange = await _context.Exchanges.FindAsync(id);

            if (exchange is null)
            {
                return NotFound(new { message = "Exchange not found." });
            }

            exchange.Status = "Completed";
            exchange.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Exchange completed successfully.", exchange });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteExchange(int id)
        {
            var exchange = await _context.Exchanges.FindAsync(id);

            if (exchange is null)
            {
                return NotFound(new { message = "Exchange not found." });
            }

            _context.Exchanges.Remove(exchange);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exchange deleted successfully." });
        }
    }
}
