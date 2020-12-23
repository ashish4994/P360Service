using CreditOne.P360FormSubmissionService.Models;
using P360WebReference;
using static P360WebReference.ViewStarServiceSoapClient;

namespace CreditOne.P360FormSubmissionService.Services
{
    public abstract class P360Service
    {
        private ViewStarServiceSoapClient _p360Service;
        private P360LoginData _p360LoginData;

        public P360Service(P360LoginData p360LoginData)
        {
            _p360LoginData = p360LoginData; 
        }

        public ViewStarServiceSoapClient Service
        {
            get
            {
                if(_p360Service == null)
                    _p360Service = new ViewStarServiceSoapClient(EndpointConfiguration.ViewStarServiceSoap,_p360LoginData.Url);
                return _p360Service;
            }
        }
    }
}
