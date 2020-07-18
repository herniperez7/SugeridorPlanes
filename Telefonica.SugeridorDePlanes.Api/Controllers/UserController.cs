using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.Email;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.PDF;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.Users;
using Telefonica.SugeridorDePlanes.BusinessLogic;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;

namespace Telefonica.SugeridorDePlanes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserLogic _userLogic;
        private readonly IMapper _mapper;
        public UserController(IUserLogic userLogic, IMapper mapper)
        {
            _userLogic = userLogic;
            _mapper = mapper;
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