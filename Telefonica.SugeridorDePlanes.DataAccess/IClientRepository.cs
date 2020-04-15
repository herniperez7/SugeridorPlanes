using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess
{
    public interface IClientRepository
    {
        Task<List<SugeridorClientesDTO>> GetClientes();
    }
}
