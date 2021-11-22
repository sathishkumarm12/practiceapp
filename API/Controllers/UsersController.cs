using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseAPIController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
        {
            var user = await unitOfWork.UserRepository.GetUserByNameAsync(User.GetUsername());
            userParams.CurrentUserName = user.UserName;
            //userParams.Gender = user.Gender;

            var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser"),]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            var user = await unitOfWork.UserRepository.GetMemberAsync(username);
            //var userMapped = mapper.Map<MemberDTO>(user);
            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            var username = User.GetUsername();
            var user = await unitOfWork.UserRepository.GetUserByNameAsync(username);

            mapper.Map(memberUpdateDTO, user);

            unitOfWork.UserRepository.Update(user);

            if (await unitOfWork.Complete())
                return NoContent();
            else
                return BadRequest("User update failed");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var username = User.GetUsername();
            var user = await unitOfWork.UserRepository.GetUserByNameAsync(username);

            var result = await photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo()
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                IsMain = (user.Photos.Count == 0),

            };

            user.Photos.Add(photo);

            if (await unitOfWork.Complete())
            {
                //return mapper.Map<PhotoDTO>(photo);
                return CreatedAtRoute("GetUser",
                new { username },
                mapper.Map<PhotoDTO>(photo));
            }
            else
                return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoid}")]
        public async Task<ActionResult> SetMainPhoto(int photoid)
        {
            var user = await unitOfWork.UserRepository.GetUserByNameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoid);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.Where(p => p.IsMain == true).ToList();
            foreach (var main in currentMain)
            {
                main.IsMain = false;
            }

            photo.IsMain = true;

            unitOfWork.UserRepository.Update(user);
            if (await unitOfWork.Complete())
                return NoContent();
            else
            {
                return BadRequest("Set as main photo failed");
            }
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await unitOfWork.UserRepository.GetUserByNameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null)
            {
                return NotFound();
            }

            if (photo.IsMain)
            {
                return BadRequest("You cannot delete the mail photo");
            }

            if (photo.PublicId != null)
            {
                var delResult = await photoService.DeletePhotoAsync(photo.PublicId);

                if (delResult.Error != null) return BadRequest(delResult.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await unitOfWork.Complete())
                return NoContent();
            else
            {
                return BadRequest("Delete photo failed");
            }
        }
    }
}