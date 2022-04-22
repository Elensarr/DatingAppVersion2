using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        //get individual like
        Task<UserLike> GetUserLike(int sourceUSerId, int likedUserId);

        //get user with all likes
        Task<AppUser> GetUserWithLikes(int userId);

        // get likes from one user
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}
