using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Extensions
{
    public static class IFormFileExtensions
    {
        public static bool IsImage(this IFormFile formFile)
        {
            return formFile.ContentType.Contains("image");
        }

        public async static Task<byte[]> ToByteArrayAsync(this IFormFile formFile)
        {
            using (var ms = new MemoryStream())
            {
                await formFile.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}
