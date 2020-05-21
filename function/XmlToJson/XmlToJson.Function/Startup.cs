using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Storage.Net;
using Storage.Net.Blobs;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(XmlToJson.Function.Startup))]

namespace XmlToJson.Function
{
    
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var dataLakeAccount = Environment.GetEnvironmentVariable("DataLakeAccount");
            var dataLakeSecrect = Environment.GetEnvironmentVariable("DataLakeSecret");

            IBlobStorage storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(dataLakeAccount, dataLakeSecrect);

            builder.Services.Add(new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(typeof(IBlobStorage), storage));
        }

    }
}
