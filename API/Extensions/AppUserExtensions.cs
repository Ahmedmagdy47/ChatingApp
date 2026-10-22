using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Extensions
{
    public static class AppUserExtensions
    {
        public static async Task<UserDto> userDto(this AppUsers user, ITokenService tokenService)
        {
            return new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email!,
                Image = user.ImageUrl,
                Token = await tokenService.CreateToken(user)
            };

        }
    }
}