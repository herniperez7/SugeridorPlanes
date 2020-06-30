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
    public class ProposalController : ControllerBase
    {
        private readonly IProposalLogic _ProposalLogic;
        private readonly ISuggestorLogic _suggestorLogic;
        private readonly IClientLogic _clientLogic;
        private readonly IMapper _mapper;

        public ProposalController(ISuggestorLogic suggestorLogic, IProposalLogic ProposalLogic, IMapper mapper, IClientLogic clientLogic)
        {
            _ProposalLogic = ProposalLogic;
            _suggestorLogic = suggestorLogic;
            _mapper = mapper;
            _clientLogic = clientLogic;
        }

        [HttpGet("getProposals")]
        public async Task<ActionResult<List<Proposal>>> GetProposals()
        {
            try
            {          
                var ProposalsDTO = await _ProposalLogic.GetProposals();
                var clientList = _clientLogic.GetClientes().Result;
                List<Proposal> Proposals = new List<Proposal>();
                foreach(ProposalDTO Proposal in ProposalsDTO)
                {
                    var lineasDTO = await _ProposalLogic.GetLineasProposal(Proposal.Id);
                    var equiposDTO = await _ProposalLogic.GetEquiposProposal(Proposal.Id);
                    var lineasList = new List<ProposalLine>();
                    if (lineasDTO.Count > 0)
                    {
                        foreach(ProposalLineDTO linea in lineasDTO)
                        {
                            var plan = await _suggestorLogic.GetPlanByCode(linea.Plan);
                            var planModel = _mapper.Map<OfertPlan>(plan);
                            lineasList.Add(new ProposalLine() { Numero = linea.NumeroLinea, Plan = planModel});
                        }
                    }
                    var equiposList = new List<DevicePymes>();
                    if (equiposDTO.Count > 0)
                    {
                        foreach (ProposalDeviceDTO equipo in equiposDTO)
                        {
                            var movilDevice = await _suggestorLogic.GetEquiposPymesByCode(equipo.CODIGO_EQUIPO);
                            equiposList.Add(new DevicePymes() { CodigoEquipo = equipo.CODIGO_EQUIPO, Marca = movilDevice.Marca,Nombre =movilDevice.Nombre,PrecioSinIva = movilDevice.PrecioSinIva,Stock = movilDevice.Stock });
                        }
                    }
                    var item = _mapper.Map<Proposal>(Proposal);
                    item.Equipos = equiposList;
                    item.Lineas = lineasList;
                    Proposals.Add(item);
                }

                foreach (var p in Proposals)
                {
                    PopulateClientName(p, clientList);
                }

                return Proposals;
            }
            catch (Exception ex)
            {
                throw ex; 
            }           

        }

        [HttpGet("getProposalsUsuario")]
        public async Task<ActionResult<List<Proposal>>> GetProposalsUsuario(string idProposal)
        {
            try
            {
                var ProposalsDTO = await _ProposalLogic.GetProposalsUsuario(idProposal);
                var Proposals = _mapper.Map<List<Proposal>>(ProposalsDTO);


                return Proposals;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        [HttpGet("getProposal")]
        public async Task<ActionResult<Proposal>> GetProposal(string idProposal)
        {
            try
            {
                var plansDto = await _suggestorLogic.GetActualPlans();
                var mobileListDto = await _suggestorLogic.GetEquiposPymes();
                var ProposalDto = await _ProposalLogic.GetProposal(idProposal);
                var proposalLinesDTO = await _ProposalLogic.GetLineasProposal(ProposalDto.Id);
                var mobileDevicesDTO = await _ProposalLogic.GetEquiposProposal(ProposalDto.Id);
                var clientList = _clientLogic.GetClientes().Result;

                var proposal = _mapper.Map<Proposal>(ProposalDto);
                PopulateProposalLines(proposal, plansDto, proposalLinesDTO);
                PopulateProposalMobileList(proposal, mobileListDto, mobileDevicesDTO);
                PopulateClientName(proposal, clientList);

                return proposal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("addProposal")]
        public async Task<ActionResult<bool>> AddProposal([FromBody]ProposalData proposal)
        {
            try
            {
                ProposalDTO ProposalDTO = new ProposalDTO()
                {
                    Documento = proposal.Client.Documento,
                    Payback = proposal.Payback,
                    DevicePayment = proposal.DevicePayment,
                    Subsidio = proposal.Subsidio,
                    Guid = Guid.NewGuid().ToString(),
                    Estado = "Pendiente"
                };
                if (proposal.Finalizada) ProposalDTO.Estado = "Finalizada";
                var resultProposalAdd = await _ProposalLogic.AddProposal(ProposalDTO);
                if (resultProposalAdd)
                {
                    var ProposalDB = await _ProposalLogic.GetProposalByGuid(ProposalDTO.Guid);
                    bool listAdded = false;
                    if (ProposalDB != null)
                    {
                        if (proposal.SuggestorList.Count > 0)
                        {
                            var lineasDTO = new List<ProposalLineDTO>();
                            for (var i = 0; i < proposal.SuggestorList.Count; i++)
                            {
                                lineasDTO.Add(new ProposalLineDTO() { NumeroLinea = proposal.SuggestorList[i].Movil.ToString(), Plan = proposal.PlanesDefList[i].Plan, IdPropuesta = ProposalDB.Id });

                            }
                            listAdded = await _ProposalLogic.AddLineasProposal(lineasDTO);
                        }
                        var equiposDTO = new List<ProposalDeviceDTO>();
                        if (proposal.MobileDevicesList.Count > 0)
                        {
                            foreach (DevicePymes equipo in proposal.MobileDevicesList)
                            {
                                equiposDTO.Add(new ProposalDeviceDTO() { IdPropuesta = ProposalDB.Id, CODIGO_EQUIPO = equipo.CodigoEquipo });
                            }

                            var equipAdded = await _ProposalLogic.AddEquiposProposal(equiposDTO);
                            if (!equipAdded)
                            {
                                await _ProposalLogic.DeleteProposalByGuid(ProposalDTO.Guid);
                                return false;
                            }
                        }
                        if (listAdded)
                        {
                            return true;
                        }
                        else
                        {
                            await _ProposalLogic.DeleteProposalByGuid(ProposalDTO.Guid);
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

        private void PopulateProposalLines(Proposal proposal, List<OfertActualPlanDTO> plansDto, List<ProposalLineDTO> linesDto) 
        {
            proposal.Lineas = new List<ProposalLine>();

            foreach (var p in linesDto)
            {
                var plan = plansDto.Where(x => x.Plan == p.Plan).FirstOrDefault();
                var planModel = _mapper.Map<OfertPlan>(plan);
                var proposalLine = new ProposalLine() { Numero = p.NumeroLinea, Plan = planModel };
                proposal.Lineas.Add(proposalLine);
            }        
        }

        private void PopulateProposalMobileList(Proposal proposal, List<DevicePymes> equipoPymesList, List<ProposalDeviceDTO> equipoLinesDto ) 
        {
            proposal.Equipos = new List<DevicePymes>();

            foreach (var m in equipoLinesDto)
            {
                var equipoPymes = equipoPymesList.Where(x => x.CodigoEquipo.Equals(m.CODIGO_EQUIPO)).FirstOrDefault();
                proposal.Equipos.Add(equipoPymes);
            }
        }

        private void PopulateClientName(Proposal proposal, List<SuggestorClientDTO> clients) 
        {
            var clientList = _clientLogic.GetClientes().Result;
            var client = clientList.Where(c => c.Documento.Equals(proposal.RutCliente)).FirstOrDefault();
            proposal.ClientName = client != null ? client.Titular : proposal.RutCliente;
        }        
    }
}