using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using Telefonica.SugeridorDePlanes.Models.Data;
using Telefonica.SugeridorDePlanes.Resources.Enums;
using Telefonica.SugeridorDePlanes.Resources.helpers;

namespace Telefonica.SugeridorDePlanes.Code
{
    public class TelefonicaService : ITelefonicaService
    {
        private readonly IClient _client;
        private readonly IMapper _mapper;
        private static List<OfertActualPlanModel> _ofertPlanList;
        private static List<SuggestorB2b> _currentPlans;
        private static List<DefinitivePlanModel> _curretDefinitvePlans;
        private static SuggestorClientModel _currentClient;
        private static List<SuggestorClientModel> _currentClients;

        //Lista total de moviles
        private static List<DevicePymesModel> _equiposPymes;
        //Lista de moviles seleccionados para incorporar en la Proposal
        private static List<DevicePymesModel> _currentEquiposPymes;

        //Lista de moviles que se confirmaron para la propuesta
        private static List<DevicePymesModel> _confirmedEquiposPymes;
        private static Proposal _CurrentProposal;

        public TelefonicaService(IClient client, IMapper mapper)
        {
            _client = client;
           // _currentPlans = new List<SuggestorB2b>();
            _mapper = mapper;

            if (_confirmedEquiposPymes == null) 
            {
                _confirmedEquiposPymes = new List<DevicePymesModel>();
            }            
        }

        public async Task<bool> PopulateData() 
        {
            try
            {
                await PopulateEquiposPymesList();
                await PopulateClientList();
                await PopulateOfertPlanList();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }        
        }

        private async Task<bool> PopulateOfertPlanList()
        {
            try
            {
                var plans = await _client.GetActualPlansAsync();

               /* foreach (var plan in plans)
                {
                    plan.Bono_ /= 1024;
                }*/

                _ofertPlanList = _mapper.Map<List<OfertActualPlanModel>>(plans);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        private async Task<bool> PopulateClientList()
        {
            try
            {
                _currentClients = new List<SuggestorClientModel>();
                var clients = await _client.GetClientsAsync();
                _currentClients = _mapper.Map<List<SuggestorClientModel>>(clients);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public List<DevicePymesModel> GetConfirmedEquiposPymes()
        {
            var equiposPymes = _confirmedEquiposPymes;
            return equiposPymes;
        }

        public void SetConfirmedEquiposPymes(List<DevicePymesModel> currentList)
        {
            _confirmedEquiposPymes = currentList;
        }

        public Proposal GetCurrentProposal()
        {
            return _CurrentProposal;
        }

        public void SetCurrentProposal(Proposal proposal)
        {
            _CurrentProposal = proposal;
        }     

        
        public List<SuggestorClientModel> GetCurrentClients()
        {
            return _currentClients;
        }

        
        public SuggestorClientModel GetCurrentClient()
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

        public List<OfertActualPlanModel> GetActualPlans()
        {
            return _ofertPlanList;
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

        public void SetCurrentDefinitivePlans(List<DefinitivePlanModel> currentPlans)
        {
            _curretDefinitvePlans = currentPlans;
        }

        public void UpdateCurrentDefinitivePlans(UpdateSuggestedPlanModel updatePlan)
        {
            var defPlansList = _curretDefinitvePlans;

            var ofertPlan = _ofertPlanList.Where(x => x.Plan.Equals(updatePlan.Plan)).FirstOrDefault();

            _curretDefinitvePlans = defPlansList.Select(x =>
            new DefinitivePlanModel
            {
                RecomendadorId = x.RecomendadorId,
                Plan = x.Plan,
                Bono = x.Bono,
                Roaming = x.Roaming,
                TMM_s_iva = x.TMM_s_iva,
                TmmString = TelefonicaHelper.FormatCultureNumber(x.TMM_s_iva)
            }).ToList();
            //            

            
            foreach (var plan in _curretDefinitvePlans)
            {
                if (plan.RecomendadorId == updatePlan.PlanToEdit)
                {
                    plan.Plan = ofertPlan.Plan;
                    plan.Bono = ofertPlan.Bono_ / 1024;
                    plan.Roaming = ofertPlan.Roaming;
                    plan.TMM_s_iva = ofertPlan.TMM_s_iva;
                    plan.TmmString = TelefonicaHelper.FormatCultureNumber(ofertPlan.TMM_s_iva);
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

        private async Task<bool> PopulateEquiposPymesList()
        {
            try
            {
                var id = 1;
                var mobileDevices = await _client.GetMobileDevicesAsync();
                _equiposPymes = _mapper.Map<List<DevicePymesModel>>(mobileDevices);

                foreach (var mobile in _equiposPymes)
                {
                    mobile.Id = id.ToString();
                    mobile.PrecioSinIva = Math.Round(mobile.PrecioSinIva, 1);
                    id++;
                }
                return true;
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
            var mobileList = _currentEquiposPymes;
            return mobileList;
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

            if (delete)
            {
                mobile = _currentEquiposPymes.Where(x => x.Id == code).FirstOrDefault();
                if (mobile != null)
                {
                    _currentEquiposPymes.Remove(mobile);
                }
            }
            else
            {
                mobile = _equiposPymes.Where(x => x.Id == code).FirstOrDefault();
                if (mobile != null)
                {
                    _currentEquiposPymes.Add(mobile);
                }
            }
        }

        public byte[] GeneratePdfFromHtml(string devicePayment)
        {
            try
            {
                var mobileList = GetConfirmedEquiposPymes();
                var client = GetCurrentClient();
                var planesDefList = GetCurrentDefinitivePlans();
                var devicePaymentDouble = Convert.ToDouble(devicePayment);
                var planesDef = _mapper.Map<List<OfertPlan>>(planesDefList);
                var mobileDevicesList = _mapper.Map<List<DevicePymes>>(mobileList);

                var proposalPdf = new ProposalPdf
                {
                    MobileList = mobileDevicesList,
                    PlanList = planesDef,
                    CompanyName = client.Titular,
                    DevicePayment = devicePaymentDouble
                };

                var pdfByteArray = _client.GeneratePdfAsync(proposalPdf).Result;

                return pdfByteArray;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Proposal> AddProposal(ProposalData proposal)
        {
            try
            {
                Proposal proposalResult = null;
                Proposal currentProposal = _CurrentProposal;
                if (proposal != null)
                {
                    if (_CurrentProposal != null)
                    {
                        await SaveProposal(proposal.DevicePayment.ToString(), proposal.Finalizada, proposal.IdUsuario);
                    }
                    else
                    {
                        proposalResult = await _client.AddProposalAsync(proposal);
                    }
                }
                return proposalResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Proposal>> GetProposalsByUser(string idUsuario)
        {
            try
            {
                if (idUsuario != null && idUsuario != String.Empty)
                {
                    var Proposals = await _client.GetProposalsUsuarioAsync(idUsuario);
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

        public async Task<Proposal> GetProposalsById(string idProposal)
        {
            try
            {
                if (idProposal != null && idProposal != String.Empty)
                {
                    var Proposal = await _client.GetProposalAsync(idProposal);

                    return Proposal;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Proposal>> GetProposals()
        {
            try
            {

                var Proposals = await _client.GetProposalsAsync();
                var ProposalsList = Proposals.ToList();

                return ProposalsList;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Proposal> GetProposalById(string idProposal)
        {
            try
            {
                Proposal proposal = null;

                if (idProposal != null && idProposal != String.Empty)
                {
                    proposal = await _client.GetProposalAsync(idProposal);
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

            foreach (var m in _confirmedEquiposPymes)
            {
                subsidy += m.PrecioSinIva;
            }

            return subsidy;
        }

        public decimal GetPayback(string devicePayment)
        {

            devicePayment = !string.IsNullOrEmpty(devicePayment) ? devicePayment : "0";
            var devicePaymentNumber = int.Parse(devicePayment);
            decimal subsidy = GetSubsidy();
            subsidy -= devicePaymentNumber;

            decimal income = 0;
            decimal payback = 0;

            foreach (var plan in _curretDefinitvePlans)
            {
                income += plan.TMM_s_iva;
            }

            if (income > 0)
            {
                payback = subsidy / income;
                payback = Math.Round(payback, 1);
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

        public User GetUserByEmail(string userEmail)
        {
            if (userEmail != String.Empty)
            {
                try
                {
                    var user = _client.GetUserByEmailAsync(userEmail).Result;
                    return user;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return null;
            }

        }

        public User GetUserById(string userId)
        {
            if (userId != String.Empty)
            {
                try
                {

                    return _client.GetUserByIdAsync(userId).Result;


                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return null;
            }

        }

        public void EmptyEquipoPymesCurrentList()
        {
            _currentEquiposPymes = new List<DevicePymesModel>();
            _confirmedEquiposPymes = new List<DevicePymesModel>();
        }

        /// <summary>
        /// Metodo para actualiar toda la propuesta con sus planes y equipos
        /// </summary>
        public async Task<bool> UpdateTotalProposal(ProposalData proposal)
        {
            try
            {
                await _client.UpdateTotalProposalAsync(proposal);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ProposalData GetProposalData(string devicePayment, bool isCreated, int idUsuario)
        {
            var planesDefList = GetCurrentDefinitivePlans();
            var mobileList = GetConfirmedEquiposPymes();
            var subsidy = GetSubsidy();
            var payback = GetPayback(devicePayment);
            var clientModel = GetCurrentClient();
            var client = _mapper.Map<SuggestorClient>(clientModel);
            var suggestorList = GetCurrentPlans();
            var devicePaymentDouble = Convert.ToDouble(devicePayment);
            var subsidioDouble = Convert.ToDouble(subsidy);
            var paybackDouble = Convert.ToDouble(payback);
            var planesDef = _mapper.Map<List<OfertPlan>>(planesDefList);
            var mobileDevicesList = _mapper.Map<List<DevicePymes>>(mobileList);

            ProposalData proposalData = new ProposalData()
            {
                Client = client,
                SuggestorList = suggestorList,
                PlanesDefList = planesDef,
                DevicePayment = devicePaymentDouble,
                Payback = paybackDouble,
                Subsidio = subsidioDouble,
                MobileDevicesList = mobileDevicesList,
                Finalizada = isCreated,
                IdUsuario = idUsuario,
                Activa = true
            };

            return proposalData;
        }

        public async Task<bool> SaveProposal(string devicePayment, bool isFinalized, int userId)
        {
            try
            {
                var currentProposal = GetCurrentProposal();
                //Si no hay una propuesta activa, entonces se esta creando una nueva, de lo contrario se actualizando la actual
                bool isCreated = currentProposal == null;
                var proposalData = GetProposalData(devicePayment, isFinalized, userId);

                //si no hay una propuesta activa, se crea
                if (isCreated)
                {
                    var proposalRecuest = await AddProposal(proposalData);
                    SetCurrentProposal(proposalRecuest);
                }
                else
                {
                    proposalData.Id = currentProposal.Id;
                    proposalData.Finalizada = isFinalized;
                    await UpdateTotalProposal(proposalData);
                    isCreated = false;
                }

                return isCreated;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<DefinitivePlanModel> PopulateDefinitivePlanList(Proposal proposal)
        {
            List<DefinitivePlanModel> planDefinitveList = new List<DefinitivePlanModel>();
            var idPlan = 1;
            foreach (var linea in proposal.Lineas)
            {
                var planModel = new DefinitivePlanModel()
                {
                    RecomendadorId = idPlan,
                    Plan = linea.Plan.Plan,
                    Bono = linea.Plan?.Bono_ != null ? linea.Plan.Bono_ / 1024 : 0,
                    Roaming = linea.Plan.Roaming,
                    TMM_s_iva = (decimal)linea.Plan.TmM_s_iva
                };

                planDefinitveList.Add(planModel);
                idPlan++;
            }

            return planDefinitveList;
        }

        public async Task<bool> DeleteProposal(int proposalId)
        {
            try
            {
                await _client.DeleteProposalAsync(proposalId);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
