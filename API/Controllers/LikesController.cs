using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly ILikesRepository _likesRepository;
        private readonly IUserRepository _userRepository;

        public LikesController(IUserRepository userRepository,
            ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
        }

        //API/likes/{username }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId(); // from authorization
            var likedUser = await _userRepository.GetUserByUserNameAsync(username); // from context
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId); // returns user and all their likes

            // if liked user exists
            if (likedUser == null)
            {
                return NotFound();
            }

            // prevent user from liking themselves
            if(sourceUser.UserName == username)
            {
                return BadRequest("Not allowed like yourself!");
            }

            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);
            // check that user hasn't like user previously
            if (userLike != null)
            {
                sourceUser.LikedUsers.Remove(userLike);
                // toggle optiopn (remove like an be implemented here)
                return Ok("disliked");
            }

            userLike = new UserLike
            {
                SourceUserID = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);
            
            if (await _userRepository.SaveAllAsync())
            {
                return Ok("liked");
            }

            return BadRequest("Failed to like user");
        }

        // api/likes
        [HttpGet]
        // predicate - taken from url: api/likes?predicate=likedBy
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams )
        {
            likesParams.UserId = User.GetUserId();  

            var users = await _likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize,
                users.TotalCount, users.TotalPages);

            return Ok(users);

        }

    }
}
