using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.DataAccess.Context;
using Telefonica.SugeridorDePlanes.DataAccess.Interfaces;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess.Services
{
    public class SuggestorRepository : ISuggestorRepository
    {
        protected readonly TelefonicaSugeridorDePlanesContext _context;

        public SuggestorRepository(TelefonicaSugeridorDePlanesContext context)
        {
            _context = context;
        }

        public async Task<List<OfertActualPlanDTO>> GetActualPlans()
        {
            try
            {
                var plans = await _context.OfertPlanActual.ToListAsync();

                return plans;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<OfertActualPlanDTO> GetPlanByCode(string planCode)
        {
            try
            {
                var plan = await _context.OfertPlanActual.Where(x => x.Plan == planCode).FirstOrDefaultAsync() ;

                return plan;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<SuggestorB2bDTO>> GetSuggestedPlans()
        {
            try
            {
                var plans =  await _context.SuggestorB2b.ToListAsync();

                return plans;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<SuggestorB2bDTO>> GetSuggestedPlansByClientNumer(string clientNumber)
        {
            try
            {
                var plans = await _context.SuggestorB2b.Where(x => x.CaNumber == clientNumber).ToListAsync();

                return plans;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<SuggestorB2bDTO>> GetSuggestedPlansByRut(string rut)
        {
            try
            {
                var plans = await _context.SuggestorB2b.Where(x => x.Rut == rut).ToListAsync();

                return plans;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retorna la lista de los equipos moviles
        /// </summary>
        /// <returns></returns>
        public async Task<List<DevicePymesDTO>> GetEquiposPymes()
        {
            try
            {
                var mobileDevices = await _context.EquipoPymes.ToListAsync();

                return mobileDevices;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DevicePymesDTO> GetEquiposByCode(string code)
        {
            try
            {
                return await _context.EquipoPymes.Where(x =>x.CodigoEquipo == code).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
