﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.DataAccess;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public class PropuestaLogic : IPropuestaLogic
    {
        private readonly IPropuestaRepository _proposalRepository;

        public PropuestaLogic(IPropuestaRepository proposalRepository)
        {
            _proposalRepository = proposalRepository;
        }

        public async Task<List<PropuestaDTO>> GetPropuestas()
        {
            try
            {                 
                return await _proposalRepository.GetPropuestas();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<PropuestaDTO>> GetPropuestasUsuario(string idUsuario)
        {
            try
            {
                return await _proposalRepository.GetPropuestasUsuario(idUsuario);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PropuestaDTO> GetPropuesta(string idPropuesta)
        {
            try
            {
                var proposal = await _proposalRepository.GetPropuesta(idPropuesta);
                return proposal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PropuestaDTO> GetPropuestaByDoc(string doc)
        {
            try
            {
                return await _proposalRepository.GetPropuestaByDoc(doc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        

        public async Task<List<LineaPropuestaDTO>> GetLineasPropuesta(int idPropuesta)
        {
            try
            {
                return await _proposalRepository.GetLineasPropuesta(idPropuesta);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<EquipoPropuestaDTO>> GetEquiposPropuesta(int idPropuesta)
        {
            try
            {
                return await _proposalRepository.GetEquiposPropuesta(idPropuesta);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateProposal(PropuestaDTO proposal) 
        {
            try
            {
                 _proposalRepository.UpdatePropsal(proposal);
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }

        public async Task<bool> UpdateTotalProposal(TransactionProposalDTO transactionProposal) 
        {
            try
            {
                await _proposalRepository.UpdateTotalProposal(transactionProposal);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }        
        }

        public Task<int> InsertProposal(TransactionProposalDTO proposaTransaction)
        {
            try
            {
               var proposalId = _proposalRepository.InsertProposal(proposaTransaction);
                return proposalId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
