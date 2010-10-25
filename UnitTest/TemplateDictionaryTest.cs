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
using OpenFAST.Error;
using OpenFAST.Template;
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;

namespace OpenFAST.UnitTests
{
    [TestFixture]
    public class TemplateDictionaryTest
    {
        [Test]
        public void TestExistingTemplateValueLookup()
        {
            IDictionary dictionary = new TemplateDictionary();
            Group template = new MessageTemplate(
                "Position",
                new Field[]
                    {
                        new Scalar("exchange", FastType.String, Operator.Copy, ScalarValue.Undefined, false)
                    });
            ScalarValue value = new StringValue("NYSE");
            dictionary.Store(template, new QName("exchange"), FastConstants.AnyType, value);

            Assert.AreEqual(ScalarValue.Undefined, dictionary.Lookup(template, new QName("bid"), FastConstants.AnyType));
        }

        [Test]
        public void TestLookupMultipleValuesForTemplate()
        {
            IDictionary dictionary = new TemplateDictionary();
            Group template = new MessageTemplate(
                "Position",
                new Field[]
                    {
                        new Scalar("exchange", FastType.String, Operator.Copy, ScalarValue.Undefined, false)
                    });
            ScalarValue value = new StringValue("NYSE");
            ScalarValue marketValue = new DecimalValue(100000.00);
            dictionary.Store(template, new QName("exchange"), FastConstants.AnyType, value);
            dictionary.Store(template, new QName("marketValue"), FastConstants.AnyType, marketValue);

            Assert.AreNotEqual(ScalarValue.Undefined, value);
            Assert.AreEqual(value, dictionary.Lookup(template, new QName("exchange"), FastConstants.AnyType));
            Assert.AreEqual(marketValue, dictionary.Lookup(template, new QName("marketValue"), FastConstants.AnyType));
        }

        [Test]
        public void TestReset()
        {
            IDictionary dictionary = new TemplateDictionary();
            Group template = new MessageTemplate(
                "Position",
                new Field[]
                    {
                        new Scalar("exchange", FastType.String, Operator.Copy, ScalarValue.Undefined, false)
                    });
            ScalarValue value = new StringValue("NYSE");
            dictionary.Store(template, new QName("exchange"), FastConstants.AnyType, value);

            Assert.AreEqual(value, dictionary.Lookup(template, new QName("exchange"), FastConstants.AnyType));
            dictionary.Reset();
            Assert.AreEqual(ScalarValue.Undefined,
                            dictionary.Lookup(template, new QName("exchange"), FastConstants.AnyType));
        }

        [Test]
        public void TestTemplateValueLookup()
        {
            IDictionary dictionary = new TemplateDictionary();
            Group template = new MessageTemplate(
                "Position",
                new Field[]
                    {
                        new Scalar("exchange", FastType.String, Operator.Copy, ScalarValue.Undefined, false)
                    });
            ScalarValue value = new StringValue("NYSE");
            dictionary.Store(template, new QName("exchange"), FastConstants.AnyType, value);

            Assert.AreEqual(value, dictionary.Lookup(template, new QName("exchange"), FastConstants.AnyType));

            Group quoteTemplate = new MessageTemplate(
                "Quote",
                new Field[]
                    {
                        new Scalar("bid", FastType.Decimal, Operator.Delta, ScalarValue.Undefined, false)
                    });
            Assert.AreEqual(ScalarValue.Undefined,
                            dictionary.Lookup(quoteTemplate, new QName("exchange"), FastConstants.AnyType));
        }
    }
}