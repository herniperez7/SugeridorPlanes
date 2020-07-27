using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.Users;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;

namespace Telefonica.SugeridorDePlanes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserLogic _userLogic;
        private readonly IMapper _mapper;
        public UserController(IUserLogic userLogic, IMapper mapper)
        {
            _userLogic = userLogic;
            _mapper = mapper;
        }

        [HttpGet("getUsers")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            try
            {

                var usersDTO = await _userLogic.GetUsers();
                var users = _mapper.Map<List<User>>(usersDTO);
                if (usersDTO != null)
                {
                    for(var i = 0; i< usersDTO.Count(); i++)
                    {
                        switch (usersDTO[i].Rol)
                        {
                            case Dto.Dto.UserRole.Executive:
                                users[i].Rol = new Executive();
                                break;
                            case Dto.Dto.UserRole.Administrator:
                                users[i].Rol = new Administrative();
                                break;
                        }
                    }
                    
                }

                return users;
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("getUserByEmail")]
        public async Task<ActionResult<User>> GetUserByEmail(string userEmail)
        {
            try
            {
               
                var userDTO = await _userLogic.GetUserByEmail(userEmail);
                var user = _mapper.Map<User>(userDTO);
                if (userDTO != null)
                { 
                    switch (userDTO.Rol)
                        {
                            case Dto.Dto.UserRole.Executive:
                                user.Rol = new Executive();
                                break;
                            case Dto.Dto.UserRole.Administrator:
                                user.Rol = new Administrative();
                                break;
                        }
                    }
                
                return user;
            }
            catch (Exception ex)
            {
                return BadRequest();               
            }
        }

        [HttpGet("getUserByUserName")]
        public async Task<ActionResult<User>> GetUserByUserName(string userName)
        {
            try
            {

                var userDTO = await _userLogic.GetUserByUserName(userName);
                var user = _mapper.Map<User>(userDTO);
                if (userDTO != null)
                {
                    switch (userDTO.Rol)
                    {
                        case Dto.Dto.UserRole.Executive:
                            user.Rol = new Executive();
                            break;
                        case Dto.Dto.UserRole.Administrator:
                            user.Rol = new Administrative();
                            break;
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpGet("getUserById")]
        public async Task<ActionResult<User>> GetUserById(string userId)
        {
            try
            {

                var userDTO = await _userLogic.GetUserById(userId);
                var user = _mapper.Map<User>(userDTO);

                return user;
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

    }
}