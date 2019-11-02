using System;
using System.IO;
using System.Text;
using System.Xml;

namespace SIAMTest.Loaders
{
    class WebServiceLoader : FileLoader
    {
        public WebServiceLoader(Uri uri) : base(uri) { }

        public override Stream LoadFile()
        {
            // HARDCODE STUB
            string xml = "<?xml version=\"1.0\"?>" +
                        "<Orders><Order id=\"1\"><Customer id=\"1\"/>" +
                        "<OrderDetails><Product id=\"1\" amount=\"1\"/></OrderDetails>" +
                        "</Order></Orders>";

            var resultStream = new MemoryStream(xml.Length*2);
            var sw = new StreamWriter(resultStream) { AutoFlush = true };
            sw.Write(xml);

            return resultStream;
        }
    }
}

