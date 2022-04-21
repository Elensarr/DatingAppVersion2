using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();

            var likes = _context.Likes.AsQueryable();

            if (predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserID == userId);
                users = likes.Select(like => like.LikedUser);                    
            }

            if (predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == userId);    
                users = likes.Select(like=> like.SourceUser);
            }

            return await users.Select(user => new LikeDto {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age =  user.DateOfBirth.CalculateAge(),
                PhotoUrl=user.Photos.FirstOrDefault(p=> p.IsMain).Url,
                City = user.City,
                Id = user.Id,
            }).ToListAsync();
        }

        // retunrn user by id with all their likes
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x=> x.LikedUsers)
                .FirstOrDefaultAsync(x=> x.Id == userId);
        }
    }
}
