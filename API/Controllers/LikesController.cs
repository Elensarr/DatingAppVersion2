using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private ILikesRepository _likesRepository;
        private IUserRepository _userRepository;

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
                // toggle optiopn (remove like an be implemented here)
                return BadRequest("You already like this user");
            }

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);
            
            if (await _userRepository.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Failed to like user");
        }

        // api/likes
        [HttpGet]
        // predicate - taken from url: api/likes?predicate=likedBy
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate)
        {
            var users = await _likesRepository.GetUserLikes(predicate, User.GetUserId());

            return Ok(users);

        }

    }
}
