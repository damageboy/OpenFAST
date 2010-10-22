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
using OpenFAST;
using OpenFAST.Codec;
using OpenFAST.Template;
using OpenFAST.Template.Type;
using UnitTest.Test;
using NUnit.Framework;

namespace UnitTest.Template.Operator
{
    [TestFixture]
    public class DefaultOperatorTest : OpenFastTestCase
    {
        [Test]
        public void TestNullsNoInitialValue()
        {
            var field = new Scalar(new QName("mostlyNull"), FASTType.I32, OpenFAST.Template.Operator.Operator.Default, ScalarValue.Undefined, true);
            MessageTemplate template = Template(field);
            FastEncoder encoder = Encoder(template);

            var message = (Message)template.CreateValue(null);
            TestUtil.AssertBitVectorEquals("11000000 10000001", encoder.Encode(message));
            TestUtil.AssertBitVectorEquals("10000000", encoder.Encode(message));
        }
        [Test]
        public void TestNullsWithInitialValue()
        {
            var field = new Scalar(new QName("sometimesNull"), FASTType.I32, OpenFAST.Template.Operator.Operator.Default, new IntegerValue(10), true);
            MessageTemplate template = Template(field);
            FastEncoder encoder = Encoder(template);

            var message = (Message)template.CreateValue(null);
            TestUtil.AssertBitVectorEquals("11100000 10000001 10000000", encoder.Encode(message));
            TestUtil.AssertBitVectorEquals("10100000 10000000", encoder.Encode(message));
            message.SetInteger(1, 10);
            TestUtil.AssertBitVectorEquals("10000000", encoder.Encode(message));
        }
        [Test]
        public void TestNullValueDoesntAlterDictionary()
        {
            var copyField = new Scalar(new QName("value"), FASTType.I32, OpenFAST.Template.Operator.Operator.Copy, new IntegerValue(10), true);
            var field = new Scalar(new QName("value"), FASTType.I32, OpenFAST.Template.Operator.Operator.Default, new IntegerValue(10), true);
            MessageTemplate copyTemplate = Template(copyField);
            MessageTemplate template = Template(field);
            var context = new Context();
            var encoder = new FastEncoder(context);
            encoder.RegisterTemplate(1, template);
            encoder.RegisterTemplate(2, copyTemplate);
            var message = (Message)copyTemplate.CreateValue(null);
            message.SetInteger(1, 11);
            encoder.Encode(message);
            Assert.AreEqual(11, context.Lookup("global", copyTemplate, new QName("value")).ToInt());
            message = (Message)template.CreateValue(null);
            encoder.Encode(message);
            Assert.AreEqual(11, context.Lookup("global", copyTemplate, new QName("value")).ToInt());
        }
    }
}
