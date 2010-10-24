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
using System.IO;
using NUnit.Framework;
using OpenFAST.Template;
using OpenFAST.Template.Loader;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests
{
    [TestFixture]
    public class TemplateTest : OpenFastTestCase
    {
        #region Setup/Teardown

        [SetUp]
        protected void SetUp()
        {
            _loader = new XmlMessageTemplateLoader();
            _loader.Load(new StreamReader("components.xml").BaseStream);
            _loader.Load(new StreamReader("preTrade.xml").BaseStream);
            _loader.Load(new StreamReader("session.xml").BaseStream);
        }

        #endregion

        private const string Scp11Ns = "http://www.fixprotocol.org/ns/fast/scp/1.1";
        private const string PreTradeNs = "http://www.openfast.org/fix44/preTrade";
        private const string SessionNs = "http://www.openfast.org/fix44/session";
        private const string ComponentsNs = "http://www.openfast.org/fix44/components";
        private const string Fix44Ns = "http://www.openfast.org/fix44";
        private const string ExtNs = "http://www.openfast.org/ext";

        private IMessageTemplateLoader _loader;

        [Test]
        public void TestTemplateExtension()
        {
            MessageTemplate logon = _loader.TemplateRegistry.GetTemplate(new QName("Logon", SessionNs));
            Assert.IsTrue(logon.HasAttribute(new QName("reset", Scp11Ns)));
        }

        [Test]
        public void TestTemplates()
        {
            MessageTemplate quote = _loader.TemplateRegistry.GetTemplate(new QName("Quote", PreTradeNs));

            Assert.AreEqual(Fix44Ns, quote.GetField("QuoteID").QName.Namespace);
            Assert.IsNotNull(quote.GetField(new QName("Group", ExtNs)));

            Assert.AreEqual(1, quote.StaticTemplateReferences.Length);
            Assert.IsNotNull(quote.GetStaticTemplateReference(new QName("Instrument", ComponentsNs)));
        }
    }
}