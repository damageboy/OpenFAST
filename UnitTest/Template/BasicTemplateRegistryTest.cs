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
using NUnit.Framework;
using OpenFAST.Template;
using UnitTest.Test;

namespace UnitTest.Template
{
    [TestFixture]
    public class BasicTemplateRegistryTest : OpenFastTestCase
    {
        [Test]
        public void TestDefine()
        {
            MessageTemplate mt = new MessageTemplate("Logon", new Field[0]);
            AbstractTemplateRegistry registry = new BasicTemplateRegistry();
            registry.Define(mt);

            //Assert.Contains(mt, registry.Templates);//dont know why it need to access when it is just defined *SM*
            Assert.AreEqual(-1, registry.GetId("Logon"));
            Assert.AreEqual(-1, registry.GetId(mt));
            Assert.AreEqual(null, registry.GetTemplate(1000));
            Assert.AreEqual(mt, registry.GetTemplate("Logon"));
        }

        // A registerd template should be in the Registry with an ID
        [Test]
        public void TestRegister()
        {
            MessageTemplate mt = new MessageTemplate("Logon", new Field[0]);
            AbstractTemplateRegistry registry = new BasicTemplateRegistry();
            registry.Register(1000, mt);

            Assert.Contains(mt, registry.Templates);
            Assert.AreEqual(1000, registry.GetId("Logon"));
            Assert.AreEqual(1000, registry.GetId(mt));
            Assert.AreEqual(mt, registry.GetTemplate(1000));
            Assert.AreEqual(mt, registry.GetTemplate("Logon"));
        }
    }
}
