using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    

    public class UsersController: BaseApiController

    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        //constructor
        public UsersController(IUserRepository userRepository,
            IMapper mapper) // injexting mapper
        {
            _userRepository = userRepository;
            _mapper = mapper;
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
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        //uploading changes
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //whoever logged in gives name  from claim (in token)
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // gets user from the database
            var user = await _userRepository.GetUserByUserNameAsync(username);

            //check if user is a default
            //if (user)
            //{
            //    return BadRequest("No user");
            //}

            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }
    }
}
