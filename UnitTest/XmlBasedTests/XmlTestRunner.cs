using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using OpenFAST.Error;
using OpenFAST.Template.Loader;

namespace OpenFAST.UnitTests.XmlBasedTests
{
    [TestFixture]
    public class XmlTestRunner
    {
        private const string XmlTestFilesDir = @"../../../Files/Tests";

        [Test]
        public void RunAllTests()
        {
            Console.WriteLine("Initializing...");

            // throw args.Exception;
            ValidationEventHandler errorHandler =
                (sender, args) => Console.WriteLine("Error {0}\n{1}", args.Message, args.Exception);

            Type thisType = MethodBase.GetCurrentMethod().DeclaringType;
            Stream schema = thisType.Assembly.GetManifestResourceStream(thisType, "TestSchema.xsd");
            Assert.IsNotNull(schema, "Unable to load XSD from the resources");

            var settings = new XmlReaderSettings();
            settings.Schemas.Add(XmlSchema.Read(schema, errorHandler));

            //
            // BUG!  TODO! change to schema-validated input
            //
            settings.ValidationType = ValidationType.None; // ValidationType.Schema;
            settings.ValidationEventHandler += errorHandler;

            foreach (string xmlFile in Directory.GetFiles(XmlTestFilesDir, "*.xml"))
            {
                Console.WriteLine("Processing {0}...", xmlFile);

                XmlReader reader = XmlReader.Create(xmlFile, settings);

                var xml = new XmlDocument();
                xml.Load(reader);

                foreach (XmlElement test in xml.GetElementsByTagName("test"))
                {
                    XmlElement desc = test.GetElement("desc");
                    Console.WriteLine("  Test {0}{1}", test.GetAttribute("name"),
                                      desc == null || string.IsNullOrEmpty(desc.Value) ? "" : " - " + desc.Value);

                    var tmpl = new XmlMessageTemplateLoader();

                    XmlElement templFile = test.GetElement("templatesfile");
                    if (templFile != null)
                    {
                        using (FileStream stream = File.OpenRead(templFile.GetAttribute("path")))
                            tmpl.Load(stream);
                    }
                    else
                    {
                        XmlElement templXml = test.GetElement("templates", FastConstants.TemplateDefinition11);
                        if (templXml == null)
                            throw new InvalidOperationException("Expected <templates> element not found");
                        tmpl.Load(templXml);
                    }


                    XmlElement binFile = test.GetElement("binfile");
                    MessageInputStream mis;
                    if (binFile != null)
                    {
                        using (FileStream stream = File.OpenRead(binFile.GetAttribute("path")))
                            mis = new MessageInputStream(stream) {TemplateRegistry = tmpl.TemplateRegistry};
                    }
                    else
                    {
                        byte[] binData;

                        XmlElement binstr = test.GetElement("binstr");
                        if (binstr != null)
                        {
                            binData = ByteUtil.ConvertBitStringToFastByteArray(binstr.InnerText);
                        }
                        else
                        {
                            XmlElement bin64 = test.GetElement("bin64");
                            binData = Convert.FromBase64String(bin64.Value);
                        }

                        mis = new MessageInputStream(new MemoryStream(binData));

                    }

                    mis.TemplateRegistry = tmpl.TemplateRegistry;

                    //
                    // TODO - read the messages and check them against the tests
                    //
                    //Message msg;
                    //while ((msg = mis.ReadMessage()) != null)
                    //{
                    //    Console.WriteLine(msg);
                    //}
                }
            }
        }
    }

    internal static class XmlExtensions
    {
        public static XmlElement GetElement(this XmlElement element, string name)
        {
            return element.GetElementsByTagName(name).Cast<XmlElement>().SingleOrDefault();
        }

        public static XmlElement GetElement(this XmlElement element, string name, string nsUri)
        {
            return element.GetElementsByTagName(name, nsUri).Cast<XmlElement>().SingleOrDefault();
        }
    }
}