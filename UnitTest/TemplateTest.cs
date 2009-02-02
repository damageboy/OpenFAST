using System;
using System.Collections.Generic;
using System.Text;
using UnitTest.Test;
using OpenFAST.Template.Loader;
using NUnit.Framework;
using System.IO;
using OpenFAST.Template;
using OpenFAST;

namespace UnitTest
{
    [TestFixture]
    public class TemplateTest : OpenFastTestCase
    {
        private static readonly String SCP_1_1_NS = "http://www.fixprotocol.org/ns/fast/scp/1.1";
        private static readonly String PRE_TRADE_NS = "http://www.openfast.org/fix44/preTrade";
        private static readonly String SESSION_NS = "http://www.openfast.org/fix44/session";
        private static readonly String COMPONENTS_NS = "http://www.openfast.org/fix44/components";
        private static readonly String FIX_44_NS = "http://www.openfast.org/fix44";
        private static readonly String EXT_NS = "http://www.openfast.org/ext";
        private MessageTemplateLoader loader;
        [SetUp]
        protected void SetUp()
        {
            loader = new XMLMessageTemplateLoader(true);
            loader.Load(new StreamReader("components.xml").BaseStream);
            loader.Load(new StreamReader("preTrade.xml").BaseStream);
            loader.Load(new StreamReader("session.xml").BaseStream);
        }
        [Test]
        public void TestTemplates()
        {
            MessageTemplate quote = loader.TemplateRegistry.get_Renamed(new QName("Quote", PRE_TRADE_NS));

            Assert.AreEqual(FIX_44_NS, quote.GetField("QuoteID").QName.Namespace);
            Assert.IsNotNull(quote.GetField(new QName("Group", EXT_NS)));

            Assert.AreEqual(1, quote.StaticTemplateReferences.Length);
            Assert.IsNotNull(quote.GetStaticTemplateReference(new QName("Instrument", COMPONENTS_NS)));
        }

        public void TestTemplateExtension()
        {
            MessageTemplate logon = loader.TemplateRegistry.get_Renamed(new QName("Logon", SESSION_NS));
            Assert.IsTrue(logon.HasAttribute(new QName("reset", SCP_1_1_NS)));
        }
    }

}
