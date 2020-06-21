using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.RequestModels;
using Telefonica.SugeridorDePlanes.BusinessLogic;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.DataAccess;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropuestaController : ControllerBase
    {
        private readonly IPropuestaLogic _propuestaLogic;
        private readonly IMapper _mapper;

        public PropuestaController(IPropuestaLogic propuestaLogic, IMapper mapper)
        {
            _propuestaLogic = propuestaLogic;
            _mapper = mapper;
        }

        [HttpGet("getPropuestas")]
        public async Task<ActionResult<List<Propuesta>>> GetPropuestas()
        {
            try
            {
               
                var propuestaDTO = await _propuestaLogic.GetPropuestas();
                var propuestaList = _mapper.Map<List<Propuesta>>(propuestaDTO);

                return propuestaList;
            }
            catch (Exception ex)
            {
                throw ex; 
            }           

        }

        [HttpGet("getPropuestasUsuario")]
        public async Task<ActionResult<List<Propuesta>>> GetPropuestasUsuario(string idProposal)
        {
            try
            {
                var propuestasDTO = await _propuestaLogic.GetPropuestasUsuario(idProposal);
                var propuestas = _mapper.Map<List<Propuesta>>(propuestasDTO);


                return propuestas;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet("getPropuesta")]
        public async Task<ActionResult<Propuesta>> GetPropuesta(string idProposal)
        {
            try
            {
                var propuestaDto = await _propuestaLogic.GetPropuesta(idProposal);
                var propuesta = _mapper.Map<Propuesta>(propuestaDto);

                return propuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost("addPropuesta")]
        public async Task<ActionResult<bool>> AddPropuesta([FromBody]ProposalData proposal)
        {
            try
            {
                PropuestaDTO propuestaDTO = new PropuestaDTO()
                {
                    Documento = proposal.Client.Documento,
                    Guid = Guid.NewGuid().ToString(),
                    Estado = "Pendiente"
                };
                if (proposal.Finalizada) propuestaDTO.Estado = "Finalizada";
                var resultProposalAdd = await _propuestaLogic.AddPropuesta(propuestaDTO);
                if (resultProposalAdd)
                {
                    var propuestaDB = await _propuestaLogic.GetPropuestaByGuid(propuestaDTO.Guid);
                    bool listAdded = false;
                    if (propuestaDB != null)
                    {
                        if (proposal.SuggestorList.Count > 0)
                        {
                            var lineasDTO = new List<LineaPropuestaDTO>();
                            for (var i = 0; i < proposal.SuggestorList.Count; i++)
                            {
                                lineasDTO.Add(new LineaPropuestaDTO() { NumeroLinea = proposal.SuggestorList[i].Movil.ToString(), Plan = proposal.PlanesDefList[i].Plan, IdPropuesta = propuestaDB.Id });

                            }
                            listAdded = await _propuestaLogic.AddLineasPropuesta(lineasDTO);
                        }
                        var equiposDTO = new List<EquipoPropuestaDTO>();
                        if (proposal.MobileDevicesList.Count > 0)
                        {
                            foreach (EquipoPymes equipo in proposal.MobileDevicesList)
                            {
                                equiposDTO.Add(new EquipoPropuestaDTO() { IdPropuesta = propuestaDB.Id, CODIGO_EQUIPO = equipo.CodigoEquipo });
                            }

                            var equipAdded = await _propuestaLogic.AddEquiposPropuesta(equiposDTO);
                            if (!equipAdded)
                            {
                                await _propuestaLogic.DeletePropuestaByGuid(propuestaDTO.Guid);
                                return false;
                            }
                        }
                        if (listAdded)
                        {
                            return true;
                        }
                        else
                        {
                            await _propuestaLogic.DeletePropuestaByGuid(propuestaDTO.Guid);
                            return false;
                        }
                    }
                                       
                }
                               
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}