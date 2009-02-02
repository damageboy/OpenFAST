using System;
using System.Collections.Generic;
using System.Text;
using UnitTest.Test;
using System.IO;
using OpenFAST.Template.Loader;
using OpenFAST;
using NUnit.Framework;

namespace UnitTest.Scenario
{
    [TestFixture]
    public class CmeTemplateTest : OpenFastTestCase
    {
        [Test]
        public void TestDeltas()
        {
            StreamReader templateSource = new StreamReader("CME/templates.xml");
            XMLMessageTemplateLoader templateLoader = new XMLMessageTemplateLoader();
            templateLoader.LoadTemplateIdFromAuxId = true;
            templateLoader.Load(templateSource.BaseStream);



            StreamReader is1 = new StreamReader("CME/messages.fast");
            MessageInputStream mis = new MessageInputStream(is1.BaseStream);
            mis.SetTemplateRegistry(templateLoader.TemplateRegistry);
            Message md = mis.ReadMessage();
            Assert.AreEqual(-5025.0, md.GetSequence("MDEntries")[0].GetDouble("NetChgPrevDay"), .1);
        }
    }
}
