using CreditOne.P360FormSubmissionService.Infra.Sync;
using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormSubmissionService.Services.Contracts;
using Microsoft.Extensions.Options;
using P360WebReference;
using System;
using System.Threading.Tasks;
using static P360WebReference.ViewStarServiceSoapClient;

namespace CreditOne.P360FormSubmissionService.Services
{
    public class P360AuthenticationService : IP360AuthenticationService
    {
        private static readonly SemaphoreLocker _locker = new SemaphoreLocker();
        private readonly IOptionsMonitor<P360ServiceData> _p360ServiceData;

        public P360AuthenticationService(IOptionsMonitor<P360ServiceData> p360ServiceData)
        {
            this._p360ServiceData = p360ServiceData;
        }

        public async Task<LoginResponse> LoginAsync(P360LoginData p360LoginData)
        {
            LoginResponse loginResponse = null;
            if (Enum.TryParse<LoginMode>(p360LoginData.LoginMode, out LoginMode loginMode))
            {
                var service = new P360WebReference.ViewStarServiceSoapClient(EndpointConfiguration.ViewStarServiceSoap,p360LoginData.Url);

                await _locker.LockAsync(async () =>
                {
                    loginResponse = await service.LoginAsync(
                        p360LoginData.Username,
                        p360LoginData.Password,
                        p360LoginData.Group,
                        loginMode);
                    // This is a work-around due to P360 issue whereas simultaneous logins to not release licenses
                    await Task.Delay(TimeSpan.FromMilliseconds(_p360ServiceData.CurrentValue.TimeBetweenP360LoginsInMs)); 
                });

            }

            return loginResponse;
        }

        public async Task<LogoutResponse> LogoutAsync(SessionTokenHeader sessionTokenHeader, P360LoginData p360LoginData)
        {
            var service = new P360WebReference.ViewStarServiceSoapClient(EndpointConfiguration.ViewStarServiceSoap,p360LoginData.Url);
            return await service.LogoutAsync(sessionTokenHeader);
        }
    }
}
