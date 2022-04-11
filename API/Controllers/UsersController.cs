using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}
