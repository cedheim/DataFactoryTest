using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Storage.Net.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace XmlToJson.Function
{
    public class WriteJsonFunction
    {
        private readonly IBlobStorage _blobStorage;
        private readonly string _container;

        public WriteJsonFunction(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
            _container = Environment.GetEnvironmentVariable("FilteredContainer");
        }

        [FunctionName(nameof(WriteJsonFunction))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] WriteJsonInputModel model, ILogger log)
        {
            if(model == null || string.IsNullOrEmpty(model.File) || model.Payload == null)
            {
                return new BadRequestObjectResult(new { Message = "Input model badly formatted" });
            }

            var fileName = $"{Path.GetFileNameWithoutExtension(model.File)}.json";
            var creationDate = model.Payload.SelectToken("HarvestedProduction.HarvestedProductionHeader.CreationDate").ToObject<DateTime>();

            var pathToFile = $"/{_container}/xml/{creationDate.Year}/{creationDate.Month}/{creationDate.Day}/{fileName}";

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model.Payload);
            await _blobStorage.WriteAsync(pathToFile, Encoding.UTF8.GetBytes(json), false);

            return new OkResult();
        }
    }
}
