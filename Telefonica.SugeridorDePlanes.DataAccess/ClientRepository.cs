using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.DataAccess.Context;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess
{
    public class ClientRepository : IClientRepository
    {
        protected readonly TelefonicaSugeridorDePlanesContext _context;
        public ClientRepository(TelefonicaSugeridorDePlanesContext context)
        {
            _context = context;
        }

        public async Task<List<SugeridorClientesDTO>> GetClientes()
        {
            try
            {
                var clients = await _context.SugeridorClientes.ToListAsync();                

                return clients;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

    }
}
