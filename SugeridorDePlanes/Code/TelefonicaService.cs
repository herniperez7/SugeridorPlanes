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
        private List<RecomendadorB2b> _currentPlans;
        private List<PlanDefinitivolModel> _curretDefinitvePlans;
        private SugeridorClientes _currentClient;
        private List<SugeridorClientes> _currentClients;

        //Lista total de moviles
        private List<EquipoPymesModel> _equiposPymes;
        //Lista de moviles seleccionados para incorporar en la propuesta
        private List<EquipoPymesModel> _currentEquiposPymes;
        //Lista de moviles que se confirmaron para la propuesta
        private List<EquipoPymesModel> _confirmedEquiposPymes;

        private Propuesta _CurrentProposal;

        public TelefonicaService(IClient client, IMapper mapper)
        {
            _client = client;
            _currentPlans = new List<RecomendadorB2b>();
            _mapper = mapper;
            PopulateEquiposPymesList(); //---> provisorio, ubicar el metodo cuando se leguea
            _currentEquiposPymes = new List<EquipoPymesModel>();
            _confirmedEquiposPymes = new List<EquipoPymesModel>();
        }

        public List<EquipoPymesModel> GetConfirmedEquiposPymes()
        {
            var equiposPymes = _confirmedEquiposPymes;
            return equiposPymes;
        }

        public void SetConfirmedEquiposPymes(List<EquipoPymesModel> currentList) 
        {
            _confirmedEquiposPymes = currentList;
        }

        public Propuesta GetCurrentProposal()
        {
            return _CurrentProposal;
        }

        public void SetCurrentProposal(Propuesta proposal)
        {
            this._CurrentProposal = proposal;
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

        public void SetCurrentDefinitivePlans(List<PlanDefinitivolModel> currentPlans)
        {
            _curretDefinitvePlans = currentPlans;
        }

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
                if (plan.RecomendadorId == updatePlan.PlanToEdit)
                {
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
                    mobile.PrecioSinIva = Math.Round(mobile.PrecioSinIva, 1);
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
                var planesDef = _mapper.Map<List<PlanesOferta>>(planesDefList);
                var mobileDevicesList = _mapper.Map<List<EquipoPymes>>(mobileList);

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

        public async Task<Propuesta> AddProposal(ProposalData proposal)
        {
            try
            {
                Propuesta proposalResult = null;
                Propuesta currentProposal = _CurrentProposal;
                if (proposal != null)
                {
                    if (_CurrentProposal != null)
                    {
                        await SaveProposal(proposal.DevicePayment.ToString(), proposal.Finalizada);
                    }
                    else 
                    {
                        proposalResult = await _client.AddPropuestaAsync(proposal);
                    }                    
                }
                return proposalResult;
            }
            catch (Exception ex)
            {
                throw ex;
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

        public Propuesta GetProposalsById(string idProposal)
        {
            try
            {
                if (idProposal != null && idProposal != String.Empty)
                {
                    var propuesta = _client.GetPropuestaAsync(idProposal).Result;

                    return propuesta;
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

            foreach (var m in _confirmedEquiposPymes)
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
                payback = Math.Round(payback, 1);
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

        public void EmptyEquipoPymesCurrentList()
        {
            _currentEquiposPymes = new List<EquipoPymesModel>();
        }


        /// <summary>
        /// Metodo para actualizar el estado de la propuesta
        /// </summary>
        /// <param name="proposal"></param>
        public async void UpdateProposal(ProposalData proposal)
        {
            try
            {
                await _client.UpdateProposalAsync(proposal);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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


        public ProposalData GetProposalData(string devicePayment, bool isCreated)
        {
            var planesDefList = GetCurrentDefinitivePlans();
            var mobileList = GetConfirmedEquiposPymes();
            var subsidy = GetSubsidy();
            var payback = GetPayback();
            var client = GetCurrentClient();
            var suggestorList = GetCurrentPlans();
            var devicePaymentDouble = Convert.ToDouble(devicePayment);
            var subsidioDouble = Convert.ToDouble(subsidy);
            var paybackDouble = Convert.ToDouble(payback);
            var planesDef = _mapper.Map<List<PlanesOferta>>(planesDefList);
            var mobileDevicesList = _mapper.Map<List<EquipoPymes>>(mobileList);

            ProposalData proposalData = new ProposalData()
            {
                Client = client,
                SuggestorList = suggestorList,
                PlanesDefList = planesDef,
                DevicePayment = devicePaymentDouble,
                Payback = paybackDouble,
                Subsidio = subsidioDouble,
                MobileDevicesList = mobileDevicesList,
                Finalizada = isCreated
            };

            return proposalData;
        }

        public async Task<bool> SaveProposal(string devicePayment, bool isFinalized)
        {
            try
            {
                var currentProposal = GetCurrentProposal();
                //Si no hay una propuesta activa, entonces se esta creando una nueva, de lo contrario se actualizando la actual
                bool isCreated = currentProposal == null; 
                var proposalData = GetProposalData(devicePayment, isCreated);

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

        public List<PlanDefinitivolModel> PopulateDefinitivePlanList(Propuesta proposal)
        {
            List<PlanDefinitivolModel> planDefinitveList = new List<PlanDefinitivolModel>();
            var idPlan = 1;
            foreach (var linea in proposal.Lineas)
            {
                var planModel = new PlanDefinitivolModel()
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

    }
}
