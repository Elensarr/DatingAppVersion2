using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UsersController : BaseApiController

    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        //constructor
        public UsersController(IUserRepository userRepository,
             IMapper mapper, // injecting mapper
             IPhotoService photoService) // inject photoservice
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        //api/users
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();

            return Ok(users);
        }

        // api/users/{name}
        // GetUser  --> name of the route
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        //uploading changes
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //whoever logged in gives name  from claim (in token)
            var username = User.GetUserName();
            // gets user from the database
            var user = await _userRepository.GetUserByUserNameAsync(username);

            //check if user is a default
            //if (user)
            //{
            //    return BadRequest("No user");
            //}

            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        //upload photo
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto([FromForm] IFormFile file)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());

            if (user == null)
            {
                return BadRequest("User is null");
            }

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                // this return 200
                // return _mapper.Map<PhotoDto>(photo);

                //to return 201 result
                // "GetUser"  -> route's name
                // new { username = user.UserName } - route parameter
                // this returns photo location https://localhost:5001/api/Users/mia in header
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            //user name from token
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());

            if (user == null)
            {
                return BadRequest("User is null");
            }

            var photo = user.Photos.FirstOrDefault(
                x => x.Id == photoId);
            if (photo.IsMain)
            {
                return BadRequest("This is already your main photo");
            }

            var currentMain = user.Photos.FirstOrDefault(
                x => x.IsMain);
            if (currentMain != null)
            {
                currentMain.IsMain = false;
            }

            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            //user name from token
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());            

            if (user == null)
            {
                return BadRequest("User is null");
            }

            //publicId from photoId
            var photoToDelete = user.Photos.FirstOrDefault(
                x => x.Id == photoId);

            if (photoToDelete == null)
            {
                return NotFound();
            }

            if(photoToDelete.IsMain)
            {
                return BadRequest("Can't delete amin photo");
            }

            if (photoToDelete.PublicId != null)
            {
                var deletionResult = await _photoService.DeletePhotoAsync(photoToDelete.PublicId);

                if(deletionResult.Error != null)
                {
                    return BadRequest(deletionResult.Error.Message);
                }
            }            

            user.Photos.Remove(photoToDelete);

            if (await _userRepository.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("An error deleting the photo occured");
        }

    }
}