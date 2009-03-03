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

*/
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
