using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.DataAccess.Context;
using Telefonica.SugeridorDePlanes.DataAccess.Interfaces;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess.Services
{
    public class ClientRepository : IClientRepository
    {
        protected readonly TelefonicaSugeridorDePlanesContext _context;
        public ClientRepository(TelefonicaSugeridorDePlanesContext context)
        {
            _context = context;
        }

        public async Task<List<SuggestorClientDTO>> GetClientes()
        {
            try
            {
                var clients = await _context.SuggestorClient.ToListAsync();                

                return clients;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

    }
}
