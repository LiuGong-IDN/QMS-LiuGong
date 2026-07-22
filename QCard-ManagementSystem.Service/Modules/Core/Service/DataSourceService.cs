using System.Data;
using Dapper;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using Kendo.Mvc.UI;
using Modules.MyMachines.Model;
using Newtonsoft.Json;
using OBS;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Session;

namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class DataSourceService(IDbHelper db) : DbService(db)
    {
        
        public List<DropDownModel> GetCustomerList(string customer, string filter)
        {
            string methodName = "GetCustomerList";
            var result = new List<DropDownModel>();

            try
            {
                var customerData = Db.DataSourceRepository.GetCustomerList(customer, filter);
                if (customerData.Count > 0)
                {
                    foreach (CustomerCode customerCode in customerData)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = customerCode.ID,
                            Text = customerCode.COMPANY_NAME.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public List<DropDownModel> GetRoleList(string filter)
        {
            string methodName = "GetRoleList";
            var result = new List<DropDownModel>();

            try
            {
                var roleData = Db.DataSourceRepository.GetRoleList(filter);
                if (roleData.Count > 0)
                {
                    foreach (Role roleCode in roleData)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = roleCode.ID,
                            Text = roleCode.ROLE_NAME.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public List<DropDownModel> GetClaimStatusList(string filter)
        {
            string methodName = "GetClaimStatusList";
            var result = new List<DropDownModel>();

            try
            {
                var claimStatus = Db.DataSourceRepository.GetClaimStatusList(filter);
                if (claimStatus.Count > 0)
                {
                    foreach (ClaimStatus status in claimStatus)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = status.ID,
                            Text = status.STATUS_NAME.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public List<DropDownModel> GetClaimProcessStatusList(string filter, int status)
        {
            string methodName = "GetClaimStatusList";
            var result = new List<DropDownModel>();

            try
            {
                var claimStatus = Db.DataSourceRepository.GetClaimProcessStatusList(filter, status);
                if (claimStatus.Count > 0)
                {
                    foreach (ClaimProcessStatus statusDdl in claimStatus)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = statusDdl.STATUS_TO,
                            Text = statusDdl.STATUS_TO_NAME.Trim(),
                            Name = statusDdl.ACTION_NAME.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }

        public List<DropDownModel> GetAssetType(string filter)
        {
            string methodName = "GetAssetType";
            var result = new List<DropDownModel>();

            try
            {
                var assetTypes = Db.DataSourceRepository.GetAssetType(filter);
                if (assetTypes.Count > 0)
                {
                    foreach (AssetType assetType in assetTypes)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = assetType.ID,
                            Text = assetType.ASSET_TYPE_NAME.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public List<DropDownModel> GetModelList(string filter)
        {
            string methodName = "GetModelList";
            var result = new List<DropDownModel>();

            try
            {
                var ModelCode = Db.DataSourceRepository.GetModelList(filter);
                if (ModelCode.Count > 0)
                {
                    foreach (ModelMachine modelCode in ModelCode)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = modelCode.ID,
                            Text = modelCode.MODEL_NAME.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public List<DropDownModel> GetPartNoList(string filter)
        {
            string methodName = "GetPartNoList";
            var result = new List<DropDownModel>();

            try
            {
                var PartCode = Db.DataSourceRepository.GetPartNoList(filter);
                if (PartCode.Count > 0)
                {
                    foreach (PartNo partCode in PartCode)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = partCode.ID,
                            Text = partCode.PART_NO.Trim(),
                            Decimal = partCode.RETAIL_PRICE,
                            Name = partCode.PART_NAME.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public List<DropDownModel> GetPartNoById(string partId)
        {
            var result = new List<DropDownModel>();
            try
            {
                var PartCode = Db.DataSourceRepository.GetPartNoById(partId);
                if (PartCode.Count > 0)
                {
                    foreach (PartNo partCode in PartCode)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = partCode.ID,
                            Text = partCode.PART_NO.Trim(),
                            Decimal = partCode.RETAIL_PRICE,
                            Name = partCode.PART_NAME.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog("GetPartNoById", this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public List<DropDownModel> GetModelbyAssetTypeList(long assetTypeId, string filter)
        {
            string methodName = "GetModelbyAssetTypeList";
            var result = new List<DropDownModel>();

            try
            {
                var ModelCode = Db.DataSourceRepository.GetModelbyAssetTypeList(assetTypeId, filter);
                if (ModelCode.Count > 0)
                {
                    foreach (ModelMachine modelCode in ModelCode)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = modelCode.ID,
                            Text = modelCode.MODEL_NAME.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public List<DropDownModel> GetPriorityList(string filter)
        {
            string methodName = "GetPriorityList";
            var result = new List<DropDownModel>();

            try
            {
                var PriorityCode = Db.DataSourceRepository.GetPriorityList(filter);
                if (PriorityCode.Count > 0)
                {
                    foreach (Priority priorityCode in PriorityCode)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = priorityCode.ID,
                            Text = priorityCode.CODE.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        
        public List<DropDownModel> GetServicePartyList(string filter)
        {
            string methodName = "GetServicePartyList";
            var result = new List<DropDownModel>();

            try
            {
                var ServiceCode = Db.DataSourceRepository.GetServicePartyList(filter);
                if (ServiceCode.Count > 0)
                {
                    foreach (ServiceParty serviceCode in ServiceCode)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = serviceCode.ID,
                            Text = serviceCode.SERVICE_PARTY_NAME.Trim(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public List<DropDownModel> GetProgramList(string filter)
        {
            string methodName = "GetProgramList";
            var result = new List<DropDownModel>();

            try
            {
                var ProgramCode = Db.DataSourceRepository.GetProgramList(filter);
                if (ProgramCode.Count > 0)
                {
                    foreach (ProgramModel programCode in ProgramCode)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = programCode.ID,
                            Text = programCode.PROGRAM_CODE.Trim(),
                            Name = programCode.PROGRAM_NAME.Trim(),
                            Description = programCode.PRIORITY
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public List<DropDownModel> GetProgramListByModelId(string filter, long modelId)
        {
            string methodName = "GetProgramListByModelId";
            var result = new List<DropDownModel>();

            try
            {
                var ProgramCode = Db.DataSourceRepository.GetProgramListByModelId(filter, modelId);
                if (ProgramCode.Count > 0)
                {
                    foreach (ProgramModel programCode in ProgramCode)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = programCode.ID,
                            Text = programCode.PROGRAM_CODE.Trim(),
                            Name = programCode.PROGRAM_NAME.Trim(),
                            Description = programCode.PRIORITY
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public List<DropDownModel> GetPDC(string filter)
        {
            string methodName = "GetPDC";
            var result = new List<DropDownModel>();

            try
            {
                var pdcList = Db.DataSourceRepository.GetPDC(filter);
                if (pdcList.Count > 0)
                {
                    foreach (PDC pdcData in pdcList)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = pdcData.ID,
                            Text = pdcData.PDC_NAME.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public List<DropDownModel> GetProgramCategory(string filter)
        {
            string methodName = "GetPDC";
            var result = new List<DropDownModel>();

            try
            {
                var programCategoryList = Db.DataSourceRepository.GetProgramCategory(filter);
                if (programCategoryList.Count > 0)
                {
                    foreach (Config programCategory in programCategoryList)
                    {
                        result.Add(new DropDownModel
                        {
                            Name = programCategory.SYSTEM_ID,
                            Text = programCategory.SYSTEM_VALUE_TEXT.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public List<DropDownModel> GetShippingAgent(string filter)
        {
            string methodName = "GetShippingAgent";
            var result = new List<DropDownModel>();

            try
            {
                var shippingAgentList = Db.DataSourceRepository.GetShippingAgent(filter);
                if (shippingAgentList.Count > 0)
                {
                    foreach (ShippingAgent shippingAgent in shippingAgentList)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = shippingAgent.ID,
                            Text = shippingAgent.SHIPPING_AGENT_NAME.Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }

        
        public List<DropDownModel> GetSerialNoQCard(string filter, int customerId, string serialno)
        {
            string methodName = "GetSerialNoQCard";
            var result = new List<DropDownModel>();
            int customerIdSession = customerId != 0
            ? customerId
            : (SessionManager.CompanyId > int.MaxValue ? int.MaxValue : (int)SessionManager.CompanyId);
            try
            {
                var SerialNo = Db.DataSourceRepository.GetSerialNoQCard(filter, customerIdSession, serialno);
                if (SerialNo.Count > 0)
                {
                    foreach (Machines serialNo in SerialNo)
                    {
                        result.Add(new DropDownModel
                        {
                            Value = serialNo.ID,
                            Text = serialNo.SERIAL_NO?.Trim(),
                            Name = serialNo.MACHINE_TYPE?.ToString(),
                            Description = serialNo.PRODUCT_MODEL?.ToString(),
                            Date = serialNo.PURCHASE_DATE,
                            Category = serialNo.CATEGORY_CLAIM.ToString()
                        });
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Deteksi error RAISERROR dari SQL
                if (sqlEx.Number == 50000)
                {
                    // Lempar kembali ke Controller agar bisa dikembalikan ke JS
                    throw new Exception(sqlEx.Message);
                }

                // Log error lain
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, sqlEx.Message, sqlEx.StackTrace);
                throw;
            }

            return result;
        }


        public HttpClient GetHttpClientWithToken(string baseUrl)
        {
            string methodName = "GetHttpClientWithToken";
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetTokenApi(baseUrl));

                return client;
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string GetTokenApi(string baseUrl)
        {
            string methodName = "GetTokenApi";
            try
            {
                var tokenAuthenticatedRequest = new TokenAuthenticatedRequest();
                tokenAuthenticatedRequest.Username = new ConfigService(Db).GetConfigByIdAndType("API_SETTING", "API_BASE_USERNAME").SYSTEM_VALUE_TEXT;
                tokenAuthenticatedRequest.Password = new ConfigService(Db).GetConfigByIdAndType("API_SETTING", "API_BASE_PASSWORD").SYSTEM_VALUE_TEXT;

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var uri = string.Format("/User/GetApiToken");
                    var serializeModel = JsonConvert.SerializeObject(tokenAuthenticatedRequest);
                    StringContent content = new StringContent(serializeModel, System.Text.Encoding.UTF8, "application/json");
                    var response = client.PostAsync(uri, content).Result;

                    TokenAuthenticatedResponse result = JsonConvert.DeserializeObject<TokenAuthenticatedResponse>(response.Content.ReadAsStringAsync().Result);
                    return result.Token;
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public (string? PhoneNumber, string? Email) ParseLoginInput(string phoneOrEmail)
        {
            if (string.IsNullOrWhiteSpace(phoneOrEmail))
                return (null, null);

            // cek apakah email
            if (phoneOrEmail.Contains('@'))
            {
                return (null, phoneOrEmail.Trim());
            }

            // anggap sisanya adalah phone number
            return (phoneOrEmail.Trim(), null);
        }

        public string GetFileImage(string fileCategory, string fileCategoryId)
        {
            string methodName = "GetFileImage";
            var result = string.Empty;
            var fileType = "IMAGE";

            try
            {
                result = Db.DataSourceRepository.GetFileImage(fileType, fileCategory, fileCategoryId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
        public DataSourceResult GetClaimStatusHistory(DataSourceRequest request, int claimId)
        {
            string methodName = "GetClaimStatusHistory";
            var result = new DataSourceResult();
            try
            {
                result = Db.DataSourceRepository.GetClaimStatusHistory(request, claimId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public ActionName GetActionName(string systemType)
        {
            string methodName = "GetActionName";
            ActionName result = null;

            try
            {
                result = Db.DataSourceRepository.GetActionName(systemType);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }
    }
}
