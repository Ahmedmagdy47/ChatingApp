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
    public class MembersController(IMemberRepository memberRepo,
        IPhotoService photoService) : BaseApiController
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

            if (await memberRepo.SaveAllAsync()) return NoContent();

            return BadRequest("Filed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto([FromForm] IFormFile file)
        {
            var member = await memberRepo.GetMemberForUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Filed to update photo");

            var result = await photoService.UploadPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId = User.GetMemberId()
            };

            if (member.ImageUrl == null)
            {
                member.ImageUrl = photo.Url;
                member.User.ImageUrl = photo.Url;
            }

            member.photos.Add(photo);

            if (await memberRepo.SaveAllAsync()) return photo;

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var member = await memberRepo.GetMemberForUpdate(User.GetMemberId());

            if (member == null) return BadRequest("No member found");

            var photo = member.photos.SingleOrDefault(x => x.Id == photoId);

            if (member.ImageUrl == photo?.Url || photo == null)
            {
                return BadRequest("Connot set this as main image");
            }

            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;

            if (await memberRepo.SaveAllAsync()) return NoContent();

            return BadRequest("Problem Setting the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var member = await memberRepo.GetMemberForUpdate(User.GetMemberId());

            if (member == null) return BadRequest("No member found");

            var photo = member.photos.SingleOrDefault(x => x.Id == photoId);

            if (photo == null || photo.Url == member.ImageUrl)
            {
                return BadRequest("You can't delete this photo");
            }

            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            member.photos.Remove(photo);

            if (await memberRepo.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the photo");
        }
    }
}