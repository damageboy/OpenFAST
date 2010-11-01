using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using OpenFAST.Error;
using OpenFAST.Template.Loader;
using OpenFAST.Codec;

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
                try
                {
                    var xml = new XmlDocument();
                    xml.Load(reader);

                    foreach (XmlElement test in xml.GetElementsByTagName("test"))
                    {
                        XmlElement desc = test.GetElement("desc");
                        Console.WriteLine("  Test {0}{1}", test.GetAttribute("name"),
                                          desc == null || string.IsNullOrEmpty(desc.Value) ? "" : " - " + desc.Value);

                        var tmpl = new XmlMessageTemplateLoader();

                        XmlElement templFile = test.GetElement("templatesfile");
                        tmpl.LoadTemplateIdFromAuxId = true;
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
                                string binStream = binstr.InnerText.Trim();
                                binData = ByteUtil.ConvertBitStringToFastByteArray(binStream);
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
                        Message msg;
                        XmlElement target = test.GetElement("data");
                        XmlNode msgString = target.FirstChild;
                        //MessageOutputStream mout = new MessageOutputStream(new MemoryStream());
                        //mout.TemplateRegistry = tmpl.TemplateRegistry;
                        while ((msg = mis.ReadMessage()) != null)
                        {
                            //TODO: Introduce FIX decoding/encoding Scheme);
                            if (msgString != null)
                            {
                                //try
                                //{
                                //    mout.WriteMessage(msg);
                                //}
                                //catch(Exception ex)
                                //{
                                //    Console.WriteLine(ex.ToString());
                                //}
                                Assert.AreEqual(msgString.InnerText.Trim(), msg.ToString());
                                msgString = msgString.NextSibling;
                            }
                            else
                            {
                                Console.WriteLine(msg.ToString());
                            }
                        }
                    }
                }
                finally
                {
                    reader.Close();
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