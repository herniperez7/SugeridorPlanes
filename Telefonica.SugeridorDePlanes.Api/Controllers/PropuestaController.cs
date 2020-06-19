using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
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
        private readonly IPropuestalLogic _propuestaLogic;
        private readonly IMapper _mapper;

        public PropuestaController(IPropuestalLogic propuestaLogic, IMapper mapper)
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
        public async Task<ActionResult<bool>> AddPropuesta(Propuesta propuesta)
        {
            try
            {
                var propuestaDTO = _mapper.Map<PropuestaDTO>(propuesta);
                await _propuestaLogic.AddPropuesta(propuestaDTO);
                var propuestaDB = await _propuestaLogic.GetPropuestaByDoc(propuesta.RutCliente);

                var lineasDTO = new List<LineaPropuestaDTO>();
                foreach(LineaPropuesta linea in propuesta.Lineas)
                {
                    lineasDTO.Add(new LineaPropuestaDTO() { IdPropuesta = propuestaDB.Id, NumeroLinea = linea.Numero, Plan = linea.Plan.Plan});
                }
                await _propuestaLogic.AddLineasPropuesta(lineasDTO);
                var equiposDTO = new List<EquipoPropuestaDTO>();
                foreach (EquipoPymes equipo in propuesta.Equipos)
                {
                    equiposDTO.Add(new EquipoPropuestaDTO() { IdPropuesta = propuestaDB.Id, CODIGO_EQUIPO = equipo.CodigoEquipo});
                }

                await _propuestaLogic.AddEquiposPropuesta(equiposDTO);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}