using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace XmlToJson.Function
{
    public class WriteJsonInputModel
    {
        public WriteJsonInputModel(string file, JObject payload)
        {
            File = file;
            Payload = payload;
        }

        public string File { get; }
        public JObject Payload { get; }
    }
}
