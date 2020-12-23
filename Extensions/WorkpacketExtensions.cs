using P360WebReference;
using System;
using System.Linq;

namespace CreditOne.P360FormSubmissionService.Extensions
{
    public static class WorkpacketExtensions
    {
        public static object GetProperty(this Workpacket workpacket, string propertyName)
        {
            return workpacket.WorklistAttributes.Properties
                .Where(p => p.Name == propertyName)
                .Select(p => p.Value)
                .FirstOrDefault();
        }

        public static T GetProperty<T>(this Workpacket workpacket, string propertyName)
        {
            var result = workpacket.GetProperty(propertyName);
            return (T)Convert.ChangeType(result, typeof(T));
        }
    }
}
