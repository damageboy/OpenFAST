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
using OpenFAST.Sessions;
using OpenFAST.Template;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Sessions
{
    [TestFixture]
    public class SessionControlProtocol11Test : OpenFastTestCase
    {
        #region Setup/Teardown

        [SetUp]
        protected void SetUp()
        {
            _scp11 = (SessionControlProtocol11) SessionConstants.Scp11;
        }

        #endregion

        private SessionControlProtocol11 _scp11;

        [Test]
        public void TestComplexCreateTemplateDefinitionMessage()
        {
            Message templateDef = _scp11.CreateTemplateDefinitionMessage(ObjectMother.AllocationInstruction);
            Assert.AreEqual("AllocInstrctn", templateDef.GetString("Name"));
        }

        [Test]
        public void TestComplexCreateTemplateFromMessage()
        {
            Message templateDef = _scp11.CreateTemplateDefinitionMessage(ObjectMother.AllocationInstruction);
            MessageTemplate template = _scp11.CreateTemplateFromMessage(templateDef, TemplateRegistryFields.Null);
            Assert.AreEqual(ObjectMother.AllocationInstruction, template);
        }

        [Test]
        public void TestCreateTemplateDeclarationMessage()
        {
            Message templateDecl = _scp11.CreateTemplateDeclarationMessage(ObjectMother.QuoteTemplate, 104);
            Assert.AreEqual("Quote", templateDecl.GetString("Name"));
            Assert.AreEqual(104, templateDecl.GetInt("TemplateId"));
        }

        [Test]
        public void TestSimpleCreateTemplateDefinitionMessage()
        {
            Message templateDef = _scp11.CreateTemplateDefinitionMessage(ObjectMother.QuoteTemplate);
            Assert.AreEqual("Quote", templateDef.GetString("Name"));
            SequenceValue instructions = templateDef.GetSequence("Instructions");
            Assert.AreEqual("bid", instructions[0].GetGroup(0).GetString("Name"));
            Assert.AreEqual("ask", templateDef.GetSequence("Instructions")[1].GetGroup(0).GetString("Name"));
        }

        [Test]
        public void TestSimpleCreateTemplateFromMessage()
        {
            Message templateDef = _scp11.CreateTemplateDefinitionMessage(ObjectMother.QuoteTemplate);
            MessageTemplate template = _scp11.CreateTemplateFromMessage(templateDef, TemplateRegistryFields.Null);
            Assert.AreEqual(ObjectMother.QuoteTemplate, template);
        }

        [Test]
        public void TestTemplateRef()
        {
            Message templateDef = _scp11.CreateTemplateDefinitionMessage(ObjectMother.BatchTemplate);
            Assert.AreEqual("Header", templateDef.GetSequence("Instructions")[0].GetGroup(0).GetString("Name"));
            Assert.AreEqual(SessionControlProtocol11.DynTempRefMessage,
                            templateDef.GetSequence("Instructions")[1].GetGroup(0)
                                .GetSequence("Instructions")[0].GetGroup(0));
            
            var registry = new BasicTemplateRegistry {{24, ObjectMother.HeaderTemplate}};
            Assert.AreEqual(ObjectMother.BatchTemplate, _scp11.CreateTemplateFromMessage(templateDef, registry));
        }
    }
}