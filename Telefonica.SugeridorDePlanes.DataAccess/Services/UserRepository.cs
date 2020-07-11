using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.DataAccess.Context;
using Telefonica.SugeridorDePlanes.DataAccess.Interfaces;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess.Services
{
    public class UserRepository : IUserRepository
    {
        protected readonly TelefonicaSugeridorDePlanesContext _context;
        public UserRepository(TelefonicaSugeridorDePlanesContext context)
        {
            _context = context;
        }

        public async Task<UserDTO> GetUserByEmail(string userEmail)
        {
            try
            {
                var client = await _context.User.Where(x => x.Email == userEmail).FirstOrDefaultAsync();
                return client;
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
                int id = int.Parse(userId);
                return await _context.User.Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
