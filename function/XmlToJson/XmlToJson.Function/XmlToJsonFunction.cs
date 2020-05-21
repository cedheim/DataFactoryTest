using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Storage.Net;
using Storage.Net.Blobs;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace XmlToJson.Function
{
    public class XmlToJsonFunction
    {
        private readonly IBlobStorage _blobStorage;
        private readonly string _container;

        public XmlToJsonFunction(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
            _container = Environment.GetEnvironmentVariable("RawContainer");
        }

        [FunctionName(nameof(XmlToJsonFunction))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] XmlToJsonInputModel model, ILogger log)
        {
            if (string.IsNullOrEmpty(model.File))
            {
                return new BadRequestObjectResult(new { Message = "Missing file name" });
            }

            var pathToRawFile = $"/{_container}/{model.File}";

            var exists = await _blobStorage.ExistsAsync(pathToRawFile);

            if (!exists)
            {
                return new NotFoundObjectResult(new { Message = "File not found." });
            }

            string xml;
            using (var rawFile = await _blobStorage.OpenReadAsync(pathToRawFile))
            using (var rawReader = new StreamReader(rawFile))
            {
                xml = await rawReader.ReadToEndAsync();
            }

            var start = xml.IndexOf("<?");
            if(start < 0)
            {
                return new BadRequestObjectResult(new { Message = "Unable to find start." });
            }

            xml = xml.Substring(start);

            var end = xml.IndexOf("</MEDELANDE>");

            if (end < 0)
            {
                return new BadRequestObjectResult(new { Message = "Unable to find end." });
            }

            xml = xml.Substring(0, end);
            
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            string json = JsonConvert.SerializeXmlNode(doc);

            return new OkObjectResult(JObject.Parse(json));

        }
    }
}
