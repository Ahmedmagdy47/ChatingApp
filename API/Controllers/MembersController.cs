using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MembersController(IMemberRepository memberRepo) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            return Ok(await memberRepo.GetMembersAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            var member = await memberRepo.GetMemberByIdAsync(id);

            if (member == null) return NotFound();

            return Ok(member);
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
        {
            return Ok(await memberRepo.GetMemberPhotosAsync(id));
        }

        [HttpPut]
        public async Task<ActionResult> Update(MemberUpdateDto memberUpdateDto)
        {
            var memberId = User.GetMemberId();

            var member = await memberRepo.GetMemberForUpdate(memberId);

            if (member == null) return BadRequest("No member found");

            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDto.Description ?? member.Description;
            member.City = memberUpdateDto.City ?? member.City;
            member.Country = memberUpdateDto.Country ?? member.Country;

            member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;

            memberRepo.Update(member);

            if(await memberRepo.SaveAllAsync()) return NoContent();

            return BadRequest("Filed to update user");
        }
    }
}