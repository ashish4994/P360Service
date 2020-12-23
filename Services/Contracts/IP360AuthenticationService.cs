using CreditOne.P360FormSubmissionService.Models;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Services.Contracts
{
    public interface IP360AuthenticationService
    {
        Task<P360WebReference.LoginResponse> LoginAsync(P360LoginData p360LoginData);
        Task<P360WebReference.LogoutResponse> LogoutAsync(P360WebReference.SessionTokenHeader sessionTokenHeader,P360LoginData p360LoginData);
    }
}
