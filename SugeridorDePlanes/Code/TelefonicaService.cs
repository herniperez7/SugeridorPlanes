using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using Telefonica.SugeridorDePlanes.Models.Data;
using Telefonica.SugeridorDePlanes.Resources.Enums;

namespace Telefonica.SugeridorDePlanes.Code
{
    public class TelefonicaService : ITelefonicaService
    {
        private readonly IClient _client;
        private readonly IMapper _mapper;
        private List<SuggestorB2b> _currentPlans;
        private List<DefinitivePlanModel> _curretDefinitvePlans;
        private SuggestorClient _currentClient;
        private List<SuggestorClient> _currentClients;        

        //Lista total de moviles
        private List<DevicePymesModel> _equiposPymes;
        //Lista de moviles seleccionados para incorporar en la Proposal
        private List<DevicePymesModel> _currentEquiposPymes;

        public TelefonicaService(IClient client, IMapper mapper)
        {
            _client = client;
            _currentPlans = new List<SuggestorB2b>();
            _mapper = mapper;
            PopulateEquiposPymesList(); //---> provisorio, ubicar el metodo cuando se leguea
            _currentEquiposPymes = new List<DevicePymesModel>();
        }


        public async Task<List<SuggestorClient>> GetClientes()
        {
            try
            {
                var clients = await _client.GetClientsAsync();
                List<SuggestorClient> clientList = clients.ToList();
                _currentClients = clientList;

                return clientList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SuggestorClient> GetCurrentClients()
        {
            return _currentClients;
        }

        public SuggestorClient GetCurrentClient()
        {
            return _currentClient;
        }

        public void UpdateCurrentClient(string document)
        {
            var currentClient = _currentClients.Where(x => x.Documento == document).FirstOrDefault();
            _currentClient = currentClient;
        }

        public async Task<List<SuggestorB2b>> GetSuggestedPlans()
        {
            try
            {
                var plans = await _client.GetPlansAsync();
                List<SuggestorB2b> planList = plans.ToList();

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<List<OfertPlan>> GetActualPlansAsync()
        {
            try
            {
                var plans = await _client.GetActualPlansAsync();
                List<OfertPlan> planList = plans.ToList();
                PopulateEquiposPymesList();

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<List<SuggestorB2b>> GetSuggestedPlansByClientNumber(string clientNumber)
        {
            try
            {
                var plans = await _client.GetPlansByClientNumberAsync(clientNumber);
                List<SuggestorB2b> planList = plans.ToList();

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<SuggestorB2b>> GetSuggestedPlansByRut(string rut)
        {
            try
            {
                var plans = await _client.GetPlansByRutAsync(rut);
                List<SuggestorB2b> planList = plans.ToList();
                _currentPlans = planList;
                UpdateDefinitivePlans(planList);

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SuggestorB2b> GetCurrentPlans()
        {
            return _currentPlans;
        }

        public List<DefinitivePlanModel> GetCurrentDefinitivePlans()
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
            new DefinitivePlanModel
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

        private void UpdateDefinitivePlans(List<SuggestorB2b> planList)
        {
            _curretDefinitvePlans = new List<DefinitivePlanModel>();
            foreach (SuggestorB2b reco in planList)
            {
                var bono1024 = Convert.ToInt64(reco.BonoPlanSugerido) / 1024;

                DefinitivePlanModel planDef = new DefinitivePlanModel() { RecomendadorId = reco.Id, Plan = reco.PlanSugerido, Bono = bono1024, Roaming = reco.RoamingPlanSugerido, TMM_s_iva = (Decimal)reco.TmmPlanSugerido };
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


        public List<DefinitivePlanModel> UpdateDefinitivePlanList(List<SuggestorB2b> planList)
        {
            _curretDefinitvePlans = new List<DefinitivePlanModel>();
            foreach (SuggestorB2b reco in planList)
            {
                //var bono1024 = Convert.ToInt64(reco.BonoPlanSugerido) / 1024;

                DefinitivePlanModel planDef = new DefinitivePlanModel() { RecomendadorId = reco.Id, Plan = reco.PlanSugerido, Bono = Convert.ToInt64(reco.BonoPlanSugerido), Roaming = reco.RoamingPlanSugerido, TMM_s_iva = (decimal)reco.TmmPlanSugerido };
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
                _equiposPymes = _mapper.Map<List<DevicePymesModel>>(mobileDevices);

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

        public List<DevicePymesModel> GetEquiposPymesList()
        {            
            return _equiposPymes.ToList();
        }

        public List<DevicePymesModel> GetCurrentEquiposPymesList()
        {
            return _currentEquiposPymes;
        }

        public void SetCurrentEquiposPymesList(List<DevicePymesModel> mobileList) 
        {
            _currentEquiposPymes = mobileList;
        }

        /// <summary>
        /// Actualiza la lista actual de moviles de la prupuesta
        /// </summary>
        public void UpdateCurrentEquiposPymesList(string code, bool delete)
        {
            DevicePymesModel mobile = null;
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
                    var result = _client.AddProposalAsync(proposal).Result;
                    return result;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Proposal> GetProposalsByUser(string idUsuario)
        {
            try
            {
                if (idUsuario != null && idUsuario != String.Empty)
                {
                    var Proposals = _client.GetProposalsUsuarioAsync(idUsuario).Result;
                    var ProposalsList = Proposals.ToList();

                    return ProposalsList;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Proposal GetProposalsById(string idProposal)
        {
            try
            {
                if (idProposal != null && idProposal != String.Empty)
                {
                    var Proposal = _client.GetProposalAsync(idProposal).Result;

                    return Proposal;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Proposal> GetProposals()
        {
            try
            {

                    var Proposals = _client.GetProposalsAsync().Result;
                    var ProposalsList = Proposals.ToList();

                    return ProposalsList;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Proposal GetProposalById(string idProposal)
        {
            try
            {
                Proposal proposal = null;

                if (idProposal != null && idProposal != String.Empty)
                {
                    proposal = _client.GetProposalAsync(idProposal).Result; 
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
            List<SuggestorB2b> plansList = _currentPlans;
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
