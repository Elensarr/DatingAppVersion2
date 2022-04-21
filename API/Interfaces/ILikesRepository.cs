using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        //get individual like
        Task<UserLike> GetUserLike(int sourceUSerId, int likedUserId);

        //get user with all likes
        Task<AppUser> GetUserWithLikes(int userId);

        // get likes from one user
        Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId);
    }
}
