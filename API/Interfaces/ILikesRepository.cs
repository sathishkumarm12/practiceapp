using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserID);

        Task<AppUser> GetUserWithLikes(int userId);

        Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams);
    }
}