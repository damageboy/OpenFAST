/*

The contents of this file are subject to the Mozilla Public License
Version 1.1 (the "License"); you may not use this file except in
compliance with the License. You may obtain a copy of the License at
http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS"
basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
License for the specific language governing rights and limitations
under the License.

The Original Code is OpenFAST.

The Initial Developer of the Original Code is The LaSalle Technology
Group, LLC.  Portions created by Shariq Muhammad
are Copyright (C) Shariq Muhammad. All Rights Reserved.

Contributor(s): Shariq Muhammad <shariq.muhammad@gmail.com>
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
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
                        byte[] binData;
                        if (binFile != null)
                        {
                            using (FileStream stream = File.OpenRead(binFile.GetAttribute("path")))
                            {
                                mis = new MessageInputStream(stream) {TemplateRegistry = tmpl.TemplateRegistry};
                                binData = File.ReadAllBytes(binFile.GetAttribute("path"));
                            }
                        }
                        else
                        {
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
                        var msgStream = new MemoryStream();
                        var mout = new MessageOutputStream(msgStream) {TemplateRegistry = tmpl.TemplateRegistry};
                        while ((msg = mis.ReadMessage()) != null)
                        {
                            //TODO: Introduce FIX decoding/encoding Scheme);
                            if (msgString != null)
                            {
                                mout.WriteMessage(msg);
                                Assert.AreEqual(msgString.InnerText.Trim(), msg.ToString());
                                msgString = msgString.NextSibling;
                                
                            }
                            else
                            {
                                Console.WriteLine(msg);
                            }
                        }
                        //Verifying Encoding
                        if (binData != null)
                        {
                            byte[] outStrem = msgStream.ToArray();
                            if (outStrem.Length > 0)
                            {
                                for (int i = 0; i < binData.Length; i++)
                                {
                                    Assert.AreEqual(binData[i], outStrem[i]);
                                }
                            }
                            else
                            {
                                Console.WriteLine("WARNING: No data emitted during encoding.");
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