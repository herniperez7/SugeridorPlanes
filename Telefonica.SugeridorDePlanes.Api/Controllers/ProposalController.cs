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
                List<Proposal> Proposals = await getProposalWithData(ProposalsDTO);
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
                List<Proposal> Proposals = await getProposalWithData(ProposalsDTO);


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
        public async Task<ActionResult<Proposal>> AddProposal([FromBody]ProposalData proposal)
        {
            try
            {
                var transactionProposalDto = GetTransactionProposal(proposal);

                var proposalId = await _ProposalLogic.InsertProposal(transactionProposalDto);

                var proposalOffertLines = new List<ProposalLine>();

                foreach (var line in proposal.PlanesDefList)
                {

                    proposalOffertLines.Add(new ProposalLine()
                    {
                        Numero = "",
                        Plan = line
                    });
                }

                var proposalRequest = new Proposal()
                {
                    Id = proposalId,
                    RutCliente = proposal.Client.Documento,
                    ClientName = proposal.Client.Titular,
                    Lineas = proposalOffertLines,
                    Equipos = proposal.MobileDevicesList,
                    DevicePayment = proposal.DevicePayment,
                    Payback = proposal.Payback,
                    Subsidio = proposal.Subsidio,
                    Estado = transactionProposalDto.Proposal.Estado,
                    CreatedDate = transactionProposalDto.Proposal.CreatedDate,
                    IdUsuario = proposal.IdUsuario.ToString()
                };

                return proposalRequest;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("updateTotalProposal")]
        public async Task<IActionResult> UpdateTotalProposal([FromBody]ProposalData proposal)
        {
            try
            {
                var transactionProposal = GetTransactionProposal(proposal);

                await _ProposalLogic.UpdateTotalProposal(transactionProposal);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        private async Task<List<Proposal>> getProposalWithData(List<ProposalDTO> ProposalsDTO)
        {
            var clientList = _clientLogic.GetClientes().Result;
            List<Proposal> Proposals = new List<Proposal>();
            foreach (ProposalDTO Proposal in ProposalsDTO)
            {
                var lineasDTO = await _ProposalLogic.GetLineasProposal(Proposal.Id);
                var equiposDTO = await _ProposalLogic.GetEquiposProposal(Proposal.Id);
                var lineasList = new List<ProposalLine>();
                if (lineasDTO.Count > 0)
                {
                    foreach (ProposalLineDTO linea in lineasDTO)
                    {
                        var plan = await _suggestorLogic.GetPlanByCode(linea.Plan);
                        var planModel = _mapper.Map<OfertPlan>(plan);
                        lineasList.Add(new ProposalLine() { Numero = linea.NumeroLinea, Plan = planModel });
                    }
                }
                var equiposList = new List<DevicePymes>();
                if (equiposDTO.Count > 0)
                {
                    foreach (ProposalDeviceDTO equipo in equiposDTO)
                    {
                        var movilDevice = await _suggestorLogic.GetEquiposPymesByCode(equipo.CODIGO_EQUIPO);
                        equiposList.Add(new DevicePymes() { CodigoEquipo = equipo.CODIGO_EQUIPO, Marca = movilDevice.Marca, Nombre = movilDevice.Nombre, PrecioSinIva = movilDevice.PrecioSinIva, Stock = movilDevice.Stock });
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

        private void PopulateProposalMobileList(Proposal proposal, List<DevicePymes> equipoPymesList, List<ProposalDeviceDTO> equipoLinesDto)
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

        private TransactionProposalDTO GetTransactionProposal(ProposalData proposal)
        {
            ProposalDTO propuestaDTO = new ProposalDTO()
            {
                Id = proposal.Id,
                Documento = proposal.Client.Documento,
                Payback = proposal.Payback,
                DevicePayment = proposal.DevicePayment,
                Subsidio = proposal.Subsidio,
                CreatedDate = DateTime.Now,
                Estado = "Pendiente",
                IdUsuario = proposal.IdUsuario
            };



            if (proposal.Finalizada) propuestaDTO.Estado = "Finalizada";

            var lineasDTO = new List<ProposalLineDTO>();
            for (var i = 0; i < proposal.SuggestorList.Count; i++)
            {
                lineasDTO.Add(new ProposalLineDTO()
                {
                    NumeroLinea = proposal.SuggestorList[i].Movil.ToString(),
                    Plan = proposal.PlanesDefList[i].Plan,
                    IdPropuesta = 0
                });
            }

            var equiposDTO = new List<ProposalDeviceDTO>();

            foreach (DevicePymes equipo in proposal.MobileDevicesList)
            {
                equiposDTO.Add(new ProposalDeviceDTO()
                {
                    IdPropuesta = 0,
                    CODIGO_EQUIPO = equipo.CodigoEquipo
                });
            }

            var transactionProposalDto = new TransactionProposalDTO()
            {
                Proposal = propuestaDTO,
                Lines = lineasDTO,
                MobileDevices = equiposDTO
            };

            return transactionProposalDto;
        }



    }
}