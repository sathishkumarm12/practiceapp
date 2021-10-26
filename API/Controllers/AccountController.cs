
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using API.data;
using API.DTO;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using AutoMapper;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly DataContext context;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(DataContext context, Interfaces.ITokenService tokenService,
        IMapper mapper)
        {
            this.context = context;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<DTO.UserDTO>> Register(RegisterDTO registerDTO)
        {

            if (await UserExists(registerDTO.username))
                return BadRequest("USERNAME_EXISTS");

            var user = mapper.Map<AppUser>(registerDTO);

            using var hmac = new HMACSHA512();

            user.UserName = registerDTO.username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.password));
            user.PasswordKey = hmac.Key;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new UserDTO()
            {
                Username = user.UserName,
                Token = tokenService.CreateToken(user),
                PhotoUrl = user.Photos != null ? user.Photos.FirstOrDefault(x => x.IsMain == true)?.Url : null,
                KnowAs = user.KnowAs
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDTO.username.ToLower());

            if (user == null)
                return Unauthorized("Invalid user name");

            using var hmac = new HMACSHA512(user.PasswordKey);
            var passwordHase = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.password));

            for (int i = 0; i < passwordHase.Length; i++)
            {
                if (passwordHase[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid password");
            }

            return new UserDTO()
            {
                Username = user.UserName,
                Token = tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain == true)?.Url
            };
        }
    }
}