using Application.Modules.PostalCodes.Models;
using AutoMapper;
using Domain.Common.Exceptions;
using Domain.Entities.GeneralModule;
using Domain.IContracts.IAuth;
using Domain.IContracts.IRepositories.IGenericRepositories;

namespace Application.Common.Services
{
    public class AppSettingService
    {
        #region Constructors and Locals

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public AppSettingService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        #endregion
        public async Task<List<AppSettingModel>> ListRequestAsync()
        {
            try
            {
                var appSettings = await _unitOfWork.AppSettings.GetAllAsync();
                return _mapper.Map<List<AppSettingModel>>(appSettings);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<AppSettingModel> AddRequestAsync(AppSettingModel model)
        {
            var thisUser = _mapper.Map<AppSetting>(model);
            await _unitOfWork.AppSettings.AddAsync(thisUser);
            _unitOfWork.Complete();
            return _mapper.Map<AppSettingModel>(thisUser);
        }
        public async Task<AppSettingModel> UpdateRequestAsync(AppSettingModel model)
        {
            var thisUserAppSettings = await _unitOfWork.AppSettings.GetFirstOrDefaultAsync(x => x.ID == model.ID);
            if (thisUserAppSettings == null)
            {
                throw new ClientException("Not Found", System.Net.HttpStatusCode.NotFound);
            }
            else
            {
                thisUserAppSettings.Label = model.Label;
                thisUserAppSettings.Value = model.Value;
                thisUserAppSettings.Name = model.Name;
                thisUserAppSettings.Description = model.Description;
                _unitOfWork.Complete();
            }
            return await Task.FromResult(_mapper.Map<AppSettingModel>(thisUserAppSettings));
        }

        public async Task<AppSettingModel> GetAppSettingByIDAsync(int ID)
        {
            AppSettingModel result = new AppSettingModel();
            try
            {
                var setting = await _unitOfWork.AppSettings.GetFirstOrDefaultAsync(x => x.ID == ID);
                return _mapper.Map<AppSettingModel>(setting);

            }
            catch (Exception ex)
            {
                return result;
            }
        }
        public async Task<AppSettingModel> GetAppSettingByNameAsync(string name)
        {
            AppSettingModel result = new AppSettingModel();
            try
            {
                var setting = await _unitOfWork.AppSettings.GetFirstOrDefaultAsync(x => x.Name == name);
                return _mapper.Map<AppSettingModel>(setting);
            }
            catch (Exception ex)
            {
                return result;
            }
        }
        public async Task<bool> UpdateAppSettingAsync(List<AppSettingModel> appSettings)
        {
            try
            {
                foreach (var setting in appSettings)
                {
                    var settingObj = await _unitOfWork.AppSettings.GetFirstOrDefaultAsync(x => x.ID == setting.ID);
                    if (settingObj != null)
                    {
                        settingObj.Value = setting.Value;
                        _unitOfWork.AppSettings.Update(settingObj);
                        _unitOfWork.Complete();

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<SmtpCredentialModel> GetSmtpCredentials()
        {
            try
            {
                var appSettings = await _unitOfWork.AppSettings.GetAllAsync();
                return new SmtpCredentialModel()
                {
                    FromMail = appSettings.FirstOrDefault(c => c.Name == "FromMail")?.Value ?? string.Empty,
                    SmtpClient = appSettings.FirstOrDefault(c => c.Name == "SmtpClient")?.Value ?? string.Empty,
                    SmtpPort = appSettings.FirstOrDefault(c => c.Name == "SmtpPort")?.Value ?? string.Empty,
                    SmtpUser = appSettings.FirstOrDefault(c => c.Name == "SmtpUser")?.Value ?? string.Empty,
                    SmtpPassword = appSettings.FirstOrDefault(c => c.Name == "SmtpPassword")?.Value ?? string.Empty,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<SmtpCredentialModel> UpsertRequestAsync(SmtpCredentialModel request)
        {
            var appSettings = await _unitOfWork.AppSettings.GetAllAsync();
            if (!appSettings.Any())
            {
                var appsettings = new List<AppSetting>();
                foreach (var propertyInfo in request.GetType().GetProperties())
                {
                    appsettings.Add(new AppSetting()
                    {
                        Name = propertyInfo.Name,
                        Label = propertyInfo.Name,
                        Value = (propertyInfo.GetValue(request, null) ?? string.Empty).ToString(),
                    });
                }
                await _unitOfWork.AppSettings.AddRangeAsync(appsettings);
            }
            else
            {
                foreach (var item in appSettings)
                {
                    if (item.Name == "FromMail")
                    {
                        item.Value = request.FromMail;
                    }
                    if (item.Name == "SmtpClient")
                    {
                        item.Value = request.SmtpClient;
                    }
                    if (item.Name == "SmtpPort")
                    {
                        item.Value = request.SmtpPort;
                    }
                    if (item.Name == "SmtpUser")
                    {
                        item.Value = request.SmtpUser;
                    }
                    if (item.Name == "SmtpPassword")
                    {
                        item.Value = request.SmtpPassword;
                    }
                }
            }
            _unitOfWork.Complete();
            return request;
        }
    }
}
