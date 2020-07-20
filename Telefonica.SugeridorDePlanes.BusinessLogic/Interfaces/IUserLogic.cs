using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces
{
    public interface IUserLogic
    {
        Task<List<UserDTO>> GetUsers();
        Task<UserDTO> GetUserById(string userId);
        Task<UserDTO> GetUserByEmail(string userEmail);
        Task<UserDTO> GetUserByUserName(string userName);
    }
}
