using System;
using System.Collections.Generic;
using System.Text;

namespace XmlToJson.Function
{
    public class XmlToJsonInputModel
    {
        public XmlToJsonInputModel(string file) 
        {
            File = file;
        }

        public string File { get; }
    }
}
