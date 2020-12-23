using CreditOne.P360FormService.Models.Responses;
using CreditOne.P360FormSubmissionService.Extensions;
using CreditOne.P360FormSubmissionService.Infra.Strategies.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CreditOne.P360FormSubmissionService.Infra.Strategies.ImageExtensionExtractStrategies
{
    public class ByteExtensionExtractStrategy : IFileExtensionExtractStrategy
    {
        private readonly ImageDocument _document;

        static byte[] bmp = Encoding.ASCII.GetBytes("BM"); // BMP 
        static byte[] gif = Encoding.ASCII.GetBytes("GIF"); // GIF
        static byte[] png = new byte[] { 137, 80, 78, 71 }; // PNG 
        static byte[] tiff = new byte[] { 73, 73, 42 }; // TIFF 
        static byte[] tiff2 = new byte[] { 77, 77, 42 }; // TIFF 
        static byte[] jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg 
        static byte[] jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

        Dictionary<string, List<byte[]>> _extensionToByteMap = new Dictionary<string, List<byte[]>>
        {
            [ "bmp" ]   = new List<byte[]> { bmp },
            [ "jpeg" ]  = new List<byte[]> { jpeg, jpeg2 },
            [ "tiff" ]  = new List<byte[]> { tiff, tiff2 },
            [ "gif" ]   = new List<byte[]> { gif },
            [ "png" ]   = new List<byte[]> { png }
        };

        public ByteExtensionExtractStrategy(ImageDocument document)
        {
            this._document = document;
        }

        public string GetExtension()
        {
            return _extensionToByteMap
                .Where(d => d.Value.Any(ba => this._document.Pages.First().ImageData.StartsWith(ba)))
                .Select(d => d.Key)
                .FirstOrDefault() ?? string.Empty;
        }
    }
}
