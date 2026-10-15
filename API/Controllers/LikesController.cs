using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController(ILikesRepository likesRepo) : ControllerBase
    {
        [HttpPost("{targetMemberId}")]
        public async Task<ActionResult> ToggleLike(string targetMemberId)
        {
            var sourceMemberId = User.GetMemberId();

            if (sourceMemberId == targetMemberId) return BadRequest("You can't like yourself");

            var existingLike = await likesRepo.GetMemberLike(sourceMemberId, targetMemberId);

            if (existingLike == null)
            {
                var like = new MemberLike
                {
                    SourceMemberId = sourceMemberId,
                    TargetMemberId = targetMemberId
                };

                likesRepo.AddLike(like);
            }
            else
            {
                likesRepo.DeleteLike(existingLike);
            }

            if (await likesRepo.SaveAllChanges()) return Ok();

            return BadRequest("Can't update like");
        }

        [HttpGet("list")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikesIds()
        {
            return Ok(await likesRepo.GetCurrentMemberLikeIds(User.GetMemberId()));
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<Member>>> GetMemberLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.MemberId = User.GetMemberId();
            var members = await likesRepo.GetMemberLikes(likesParams);

            return Ok(members);
        }
    }
}