using System;
using System.Collections.Generic;
using System.Drawing.Text;
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
        private readonly ISuggestorLogic _suggestorLogic;
        private readonly IMapper _mapper;

        public PropuestaController(ISuggestorLogic suggestorLogic, IPropuestaLogic propuestaLogic, IMapper mapper)
        {
            _propuestaLogic = propuestaLogic;
            _suggestorLogic = suggestorLogic;
            _mapper = mapper;
        }

        [HttpGet("getPropuestas")]
        public async Task<ActionResult<List<Propuesta>>> GetPropuestas()
        {
            try
            {          
                var propuestasDTO = await _propuestaLogic.GetPropuestas();
                List<Propuesta> propuestas = new List<Propuesta>();
                foreach(PropuestaDTO propuesta in propuestasDTO)
                {
                    var lineasDTO = await _propuestaLogic.GetLineasPropuesta(propuesta.Id);
                    var equiposDTO = await _propuestaLogic.GetEquiposPropuesta(propuesta.Id);
                    var lineasList = new List<LineaPropuesta>();
                    if (lineasDTO.Count > 0)
                    {
                        foreach(LineaPropuestaDTO linea in lineasDTO)
                        {
                            var plan = await _suggestorLogic.GetPlanByCode(linea.Plan);
                            var planModel = _mapper.Map<PlanesOferta>(plan);
                            lineasList.Add(new LineaPropuesta() { Numero = linea.NumeroLinea, Plan = planModel});
                        }
                    }
                    var equiposList = new List<EquipoPymes>();
                    if (equiposDTO.Count > 0)
                    {
                        foreach (EquipoPropuestaDTO equipo in equiposDTO)
                        {
                            var movilDevice = await _suggestorLogic.GetEquiposPymesByCode(equipo.CODIGO_EQUIPO);
                            equiposList.Add(new EquipoPymes() { CodigoEquipo = equipo.CODIGO_EQUIPO, Marca = movilDevice.Marca,Nombre =movilDevice.Nombre,PrecioSinIva = movilDevice.PrecioSinIva,Stock = movilDevice.Stock });
                        }
                    }
                    var item = _mapper.Map<Propuesta>(propuesta);
                    item.Equipos = equiposList;
                    item.Lineas = lineasList;
                    propuestas.Add(item);
                }

                return propuestas;
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
                var plansDto = await _suggestorLogic.GetActualPlans();
                var mobileListDto = await _suggestorLogic.GetEquiposPymes();
                var propuestaDto = await _propuestaLogic.GetPropuesta(idProposal);
                var proposalLinesDTO = await _propuestaLogic.GetLineasPropuesta(propuestaDto.Id);
                var mobileDevicesDTO = await _propuestaLogic.GetEquiposPropuesta(propuestaDto.Id);

                var proposal = _mapper.Map<Propuesta>(propuestaDto);
                PopulateProposalLines(proposal, plansDto, proposalLinesDTO);
                PopulateProposalMobileList(proposal, mobileListDto, mobileDevicesDTO);

                return proposal;
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
                    Payback = proposal.Payback,
                    DevicePayment = proposal.DevicePayment,
                    Subsidio = proposal.Subsidio,
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

        private void PopulateProposalLines(Propuesta proposal, List<PlanesOfertaActualDTO> plansDto, List<LineaPropuestaDTO> linesDto) 
        {
            proposal.Lineas = new List<LineaPropuesta>();

            foreach (var p in linesDto)
            {
                var plan = plansDto.Where(x => x.Plan == p.Plan).FirstOrDefault();
                var planModel = _mapper.Map<PlanesOferta>(plan);
                var proposalLine = new LineaPropuesta() { Numero = p.NumeroLinea, Plan = planModel };
                proposal.Lineas.Add(proposalLine);
            }        
        }

        private void PopulateProposalMobileList(Propuesta proposal, List<EquipoPymesDTO> equipoPymesList, List<EquipoPropuestaDTO> equipoLinesDto ) 
        {
            proposal.Equipos = new List<EquipoPymes>();

            foreach (var m in equipoLinesDto)
            {
                var equipoPymesDto = equipoPymesList.Where(x => x.CodigoEquipo.Equals(m.CODIGO_EQUIPO)).FirstOrDefault();
                var equipoModel = _mapper.Map<EquipoPymes>(equipoPymesDto);
                proposal.Equipos.Add(equipoModel);
            }
        }
    }
}