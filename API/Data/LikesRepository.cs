using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext context;
        public LikesRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserID)
        {
            return await context.Likes.FindAsync(sourceUserId, likedUserID);
        }

        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var users = context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = context.Likes.AsQueryable();

            if (likesParams.predicate == "liked")
            {
                likes = likes.Where(l => l.SourceUserId == likesParams.userId);
                users = likes.Select(s => s.LikedUser);
            }
            else if (likesParams.predicate == "likedBy")
            {
                likes = likes.Where(l => l.LikedUserID == likesParams.userId);
                users = likes.Select(s => s.SourceUser);
            }
            else
            {
                return null;
            }

            var likedUsers = users.Select(u => new LikeDTO
            {
                Username = u.UserName,
                City = u.City,
                KnowAs = u.KnowAs,
                Id = u.Id,
                Age = u.DateOfBirth.CalculateAge(),
                PhotoUrl = u.Photos.FirstOrDefault(p => p.IsMain).Url
            });

            return await PagedList<LikeDTO>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await context.Users
                                .Include(x => x.LikedUsers)
                                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}