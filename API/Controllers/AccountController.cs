
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
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager, Interfaces.ITokenService tokenService,
        IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<DTO.UserDTO>> Register(RegisterDTO registerDTO)
        {

            if (await UserExists(registerDTO.username))
                return BadRequest("USERNAME_EXISTS");

            var user = mapper.Map<AppUser>(registerDTO);

            //using var hmac = new HMACSHA512();

            user.UserName = registerDTO.username.ToLower();
            // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.password));
            // user.PasswordKey = hmac.Key;
            // context.Users.Add(user);
            // await context.SaveChangesAsync();

            var result = await userManager.CreateAsync(user, registerDTO.password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            return new UserDTO()
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                PhotoUrl = user.Photos != null ? user.Photos.FirstOrDefault(x => x.IsMain == true)?.Url : null,
                KnowAs = user.KnowAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await userManager.Users.AnyAsync(u => u.UserName == username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDTO.username.ToLower());

            if (user == null)
                return Unauthorized("Invalid user name");

            //using var hmac = new HMACSHA512(user.PasswordKey);
            // var passwordHase = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.password));

            // for (int i = 0; i < passwordHase.Length; i++)
            // {
            //     if (passwordHase[i] != user.PasswordHash[i])
            //         return Unauthorized("Invalid password");
            // }

            var results = await signInManager.CheckPasswordSignInAsync(user, loginDTO.password, false);

            if (!results.Succeeded) return Unauthorized("Invalid username or password");

            return new UserDTO()
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain == true)?.Url,
                KnowAs = user.KnowAs,
                Gender = user.Gender
            };
        }
    }
}