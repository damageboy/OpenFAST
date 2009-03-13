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
            var templateSource = new StreamReader("CME/templates.xml");
            var templateLoader = new XMLMessageTemplateLoader {LoadTemplateIdFromAuxId = true};
            templateLoader.Load(templateSource.BaseStream);



            var is1 = new StreamReader("CME/messages.fast");
            var mis = new MessageInputStream(is1.BaseStream);
            mis.SetTemplateRegistry(templateLoader.TemplateRegistry);
            Message md = mis.ReadMessage();
            Assert.AreEqual(-5025.0, md.GetSequence("MDEntries")[0].GetDouble("NetChgPrevDay"), .1);
        }
    }
}
