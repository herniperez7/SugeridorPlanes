using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.DataAccess.Interfaces;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserRepository _userRepository;

        public UserLogic(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDTO>> GetUsers()
        {
            try
            {
                return await _userRepository.GetUsers();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<UserDTO> GetUserByEmail(string userEmail)
        {
            try
            {
                return await _userRepository.GetUserByEmail(userEmail);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserDTO> GetUserById(string userId)
        {
            try
            {
                return await _userRepository.GetUserById(userId);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserDTO> GetUserByUserName(string userName)
        {
            try
            {
                return await _userRepository.GetUserByUserName(userName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
    }
}
