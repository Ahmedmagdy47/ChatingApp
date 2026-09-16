using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AppUsers>>> GetMembers()
        {
            var members = await context.Users.ToListAsync();
            return Ok(members);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUsers>> GetMember(string id)
        {
            var member = await context.Users.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return Ok(member);
        }
    }
}