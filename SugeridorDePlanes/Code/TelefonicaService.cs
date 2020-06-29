using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.PDF;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.RequestModels;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using Telefonica.SugeridorDePlanes.Models.Data;
using Telefonica.SugeridorDePlanes.Resources.Enums;

namespace Telefonica.SugeridorDePlanes.Code
{
    public class TelefonicaService : ITelefonicaService
    {
        private readonly IClient _client;
        private readonly IMapper _mapper;
        private List<RecomendadorB2b> _currentPlans;
        private List<PlanDefinitivolModel> _curretDefinitvePlans;
        private SugeridorClientes _currentClient;
        private List<SugeridorClientes> _currentClients;        

        //Lista total de moviles
        private List<EquipoPymesModel> _equiposPymes;
        //Lista de moviles seleccionados para incorporar en la propuesta
        private List<EquipoPymesModel> _currentEquiposPymes;

        public TelefonicaService(IClient client, IMapper mapper)
        {
            _client = client;
            _currentPlans = new List<RecomendadorB2b>();
            _mapper = mapper;
            PopulateEquiposPymesList(); //---> provisorio, ubicar el metodo cuando se leguea
            _currentEquiposPymes = new List<EquipoPymesModel>();
        }


        public async Task<List<SugeridorClientes>> GetClientes()
        {
            try
            {
                var clients = await _client.GetClientsAsync();
                List<SugeridorClientes> clientList = clients.ToList();
                _currentClients = clientList;

                return clientList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SugeridorClientes> GetCurrentClients()
        {
            return _currentClients;
        }

        public SugeridorClientes GetCurrentClient()
        {
            return _currentClient;
        }

        public void UpdateCurrentClient(string document)
        {
            var currentClient = _currentClients.Where(x => x.Documento == document).FirstOrDefault();
            _currentClient = currentClient;
        }

        public async Task<List<RecomendadorB2b>> GetSuggestedPlans()
        {
            try
            {
                var plans = await _client.GetPlansAsync();
                List<RecomendadorB2b> planList = plans.ToList();

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<List<PlanesOferta>> GetActualPlansAsync()
        {
            try
            {
                var plans = await _client.GetActualPlansAsync();
                List<PlanesOferta> planList = plans.ToList();
                PopulateEquiposPymesList();

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<List<RecomendadorB2b>> GetSuggestedPlansByClientNumber(string clientNumber)
        {
            try
            {
                var plans = await _client.GetPlansByClientNumberAsync(clientNumber);
                List<RecomendadorB2b> planList = plans.ToList();

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<RecomendadorB2b>> GetSuggestedPlansByRut(string rut)
        {
            try
            {
                var plans = await _client.GetPlansByRutAsync(rut);
                List<RecomendadorB2b> planList = plans.ToList();
                _currentPlans = planList;
                UpdateDefinitivePlans(planList);

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RecomendadorB2b> GetCurrentPlans()
        {
            return _currentPlans;
        }

        public List<PlanDefinitivolModel> GetCurrentDefinitivePlans()
        {
            return _curretDefinitvePlans;
        }

        //public void UpdateCurrentDefinitivePlans(List<PlanDefinitivolModel> currentPlans)
        //{
        //    _curretDefinitvePlans = currentPlans;
        //}

        public void UpdateCurrentDefinitivePlans(UpdateSuggestedPlanModel updatePlan) 
        {
            var defPlansList = _curretDefinitvePlans;
            
            _curretDefinitvePlans = defPlansList.Select(x =>
            new PlanDefinitivolModel
            {
                RecomendadorId = x.RecomendadorId,
                Plan = x.Plan,
                Bono = x.Bono,
                Roaming = x.Roaming,
                TMM_s_iva = x.TMM_s_iva,
                TmmString = x.TMM_s_iva.ToString("n")
            }).ToList();
            //            

            foreach (var plan in _curretDefinitvePlans)
            {
                if (plan.RecomendadorId == updatePlan.PlanToEdit) {
                    plan.Plan = updatePlan.Plan;
                    plan.Bono = long.Parse(updatePlan.Bono);
                    plan.Roaming = updatePlan.Roaming;
                    plan.TMM_s_iva = decimal.Parse(updatePlan.TMM);
                    plan.TmmString = decimal.Parse(updatePlan.TMM).ToString("n");
                }
            }           
        }      

        private void UpdateDefinitivePlans(List<RecomendadorB2b> planList)
        {
            _curretDefinitvePlans = new List<PlanDefinitivolModel>();
            foreach (RecomendadorB2b reco in planList)
            {
                var bono1024 = Convert.ToInt64(reco.BonoPlanSugerido) / 1024;

                PlanDefinitivolModel planDef = new PlanDefinitivolModel() { RecomendadorId = reco.Id, Plan = reco.PlanSugerido, Bono = bono1024, Roaming = reco.RoamingPlanSugerido, TMM_s_iva = (Decimal)reco.TmmPlanSugerido };
                _curretDefinitvePlans.Add(planDef);
            }
        }

        /// <summary>
        /// retorna la suma del total de los tmm sin iva de los planes definitivos
        /// </summary>
        /// <returns></returns>
        public decimal GetDefinitivePlansIncome()
        {
            decimal income = 0;

            foreach (var plan in _curretDefinitvePlans)
            {
                income += plan.TMM_s_iva;
            }

            return income;
        }


        public List<PlanDefinitivolModel> UpdateDefinitivePlanList(List<RecomendadorB2b> planList)
        {
            _curretDefinitvePlans = new List<PlanDefinitivolModel>();
            foreach (RecomendadorB2b reco in planList)
            {
                //var bono1024 = Convert.ToInt64(reco.BonoPlanSugerido) / 1024;

                PlanDefinitivolModel planDef = new PlanDefinitivolModel() { RecomendadorId = reco.Id, Plan = reco.PlanSugerido, Bono = Convert.ToInt64(reco.BonoPlanSugerido), Roaming = reco.RoamingPlanSugerido, TMM_s_iva = (decimal)reco.TmmPlanSugerido };
                _curretDefinitvePlans.Add(planDef);
            }
            return _curretDefinitvePlans;
        }

        public async Task SendMail(Email emailData)
        {
            try
            {
                await _client.SendMailAsync(emailData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void PopulateEquiposPymesList()
        {
            try
            {
                var id = 1;
                var mobileDevices = await _client.GetMobileDevicesAsync();
                _equiposPymes = _mapper.Map<List<EquipoPymesModel>>(mobileDevices);

                foreach (var mobile in _equiposPymes)
                {
                    mobile.Id = id.ToString();
                    id++;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EquipoPymesModel> GetEquiposPymesList()
        {            
            return _equiposPymes.ToList();
        }

        public List<EquipoPymesModel> GetCurrentEquiposPymesList()
        {
            return _currentEquiposPymes;
        }

        public void SetCurrentEquiposPymesList(List<EquipoPymesModel> mobileList) 
        {
            _currentEquiposPymes = mobileList;
        }

        /// <summary>
        /// Actualiza la lista actual de moviles de la prupuesta
        /// </summary>
        public void UpdateCurrentEquiposPymesList(string code, bool delete)
        {
            EquipoPymesModel mobile = null;
            mobile = _equiposPymes.Where(x => x.CodigoEquipo == code).FirstOrDefault();
            if (delete)
            {
                if(mobile != null)
                {
                    _currentEquiposPymes.Remove(mobile);
                }                
            }
            else
            {                
                if(mobile != null)
                {
                    _currentEquiposPymes.Add(mobile);
                }
            }
        }

        public byte[] GeneratePdfFromHtml(ProposalPdf proposalPdf)
        {
            try
            {
                var pdfByteArray = _client.GeneratePdfAsync(proposalPdf).Result;

                return pdfByteArray;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AddProposal(ProposalData proposal)
        {
            try
            {
                if(proposal != null)
                {
                    var result = _client.AddPropuestaAsync(proposal).Result;
                    return result;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Propuesta> GetProposalsByUser(string idUsuario)
        {
            try
            {
                if (idUsuario != null && idUsuario != String.Empty)
                {
                    var propuestas = _client.GetPropuestasUsuarioAsync(idUsuario).Result;
                    var propuestasList = propuestas.ToList();

                    return propuestasList;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Propuesta> GetProposals()
        {
            try
            {

                    var propuestas = _client.GetPropuestasAsync().Result;
                    var propuestasList = propuestas.ToList();

                    return propuestasList;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Propuesta GetProposalById(string idProposal)
        {
            try
            {
                Propuesta proposal = null;

                if (idProposal != null && idProposal != String.Empty)
                {
                    proposal = _client.GetPropuestaAsync(idProposal).Result; 
                }

                return proposal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public decimal GetSubsidy() 
        {
            decimal subsidy = 0;

            foreach (var m in _currentEquiposPymes)
            {
                subsidy += m.PrecioSinIva;
            }

            return subsidy;
        }

        public decimal GetPayback() 
        {
            decimal subsidy = GetSubsidy();
            decimal income = 0;
            decimal payback = 0;

            foreach (var plan in _curretDefinitvePlans)
            {
                income += plan.TMM_s_iva;
            }

            if (income > 0) 
            {
                payback = subsidy / income;
            }

            return payback;
        }

        public IndexModel CalculateIndexes() 
        {
            List<RecomendadorB2b> plansList = _currentPlans;
            var defPlansList = _curretDefinitvePlans;
            BillingStatus billingStatus = BillingStatus.None;

            decimal fixedGap = 0; //Gap fijo
            decimal billingGap = 0; //Gap de facturacion
            decimal arpuProm = 0; // ARPUPROM
            decimal tmmSumatory = 0; // TMM+Prestacion
            decimal defTmmSumatory = 0;  //TMM de planes sugeridos
            decimal tmmSinIva = 0; //TMM sin iva info actual
                                   //   decimal billingDifference = 0; //diferencia entre el tmm de info actual y el tmm de sugerido

            foreach (var plan in plansList)
            {
                arpuProm += plan.ArpuProm != null ? (decimal)plan.ArpuProm : 0;
                tmmSumatory += plan.TmmPrestacion != null ? (decimal)plan.TmmPrestacion : 0;
                tmmSinIva += plan.TmmSinIva != null ? (decimal)plan.TmmSinIva : 0;
            }

            foreach (var defPlan in defPlansList)
            {
                defTmmSumatory += defPlan.TMM_s_iva;
            }

            billingGap = defTmmSumatory - arpuProm;
            fixedGap = defTmmSumatory - tmmSumatory;

            if (plansList.Count > 0)
            {
                if (billingGap > 0)
                {
                    billingStatus = BillingStatus.Higher;
                }
                else if (billingGap < 0)
                {
                     billingStatus = BillingStatus.Lower;
                }
                else if (billingGap == 0)
                {
                     billingStatus = BillingStatus.Equal;
                }
            }


            var gapModel = new IndexModel
            {
                BillingGap = billingGap,
                FixedGap = fixedGap,
                 BillingStatus = billingStatus,
                TmmPrestacion = tmmSumatory
            };
            return gapModel;
        }

    }
}
