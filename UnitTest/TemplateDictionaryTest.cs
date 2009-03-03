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
using NUnit.Framework;
using OpenFAST;
using OpenFAST.Template;
using OpenFAST.Template.Type;
using OpenFAST.Template.operator_Renamed;
using OpenFAST.Error;

namespace UnitTest
{
    [TestFixture]
    public class TemplateDictionaryTest
    {
        [Test]
        public void TestTemplateValueLookup()
        {
            Dictionary dictionary = new TemplateDictionary();
            Group template = new MessageTemplate("Position",
                    new Field[] {
                    new Scalar("exchange", FASTType.STRING, Operator.COPY, ScalarValue.UNDEFINED, false)
                });
            ScalarValue value = new StringValue("NYSE");
            dictionary.Store(template, FastConstants.ANY_TYPE, new QName("exchange"), value);

            Assert.AreEqual(value, dictionary.Lookup(template, new QName("exchange"), FastConstants.ANY_TYPE));

            Group quoteTemplate = new MessageTemplate("Quote",
                    new Field[] {
                    new Scalar("bid", FASTType.DECIMAL, Operator.DELTA, ScalarValue.UNDEFINED, false)
                });
            Assert.AreEqual(ScalarValue.UNDEFINED,
                dictionary.Lookup(quoteTemplate, new QName("exchange"), FastConstants.ANY_TYPE));
        }
        [Test]
        public void TestLookupMultipleValuesForTemplate()
        {
            Dictionary dictionary = new TemplateDictionary();
            Group template = new MessageTemplate("Position",
                    new Field[] {
                    new Scalar("exchange", FASTType.STRING, Operator.COPY, ScalarValue.UNDEFINED, false)
                });
            ScalarValue value = new StringValue("NYSE");
            ScalarValue marketValue = new DecimalValue(100000.00);
            dictionary.Store(template, FastConstants.ANY_TYPE, new QName("exchange"), value);
            dictionary.Store(template, FastConstants.ANY_TYPE, new QName("marketValue"), marketValue);

            Assert.IsFalse(value.Equals(ScalarValue.UNDEFINED));
            Assert.AreEqual(value, dictionary.Lookup(template, new QName("exchange"), FastConstants.ANY_TYPE));
            Assert.AreEqual(marketValue, dictionary.Lookup(template, new QName("marketValue"), FastConstants.ANY_TYPE));
        }
        [Test]
        public void TestReset()
        {
            Dictionary dictionary = new TemplateDictionary();
            Group template = new MessageTemplate("Position",
                    new Field[] {
                    new Scalar("exchange", FASTType.STRING, Operator.COPY, ScalarValue.UNDEFINED, false)
                });
            ScalarValue value = new StringValue("NYSE");
            dictionary.Store(template, FastConstants.ANY_TYPE, new QName("exchange"), value);

            Assert.AreEqual(value, dictionary.Lookup(template, new QName("exchange"), FastConstants.ANY_TYPE));
            dictionary.Reset();
            Assert.AreEqual(ScalarValue.UNDEFINED,
                dictionary.Lookup(template, new QName("exchange"), FastConstants.ANY_TYPE));
        }
        [Test]
        public void TestExistingTemplateValueLookup()
        {
            Dictionary dictionary = new TemplateDictionary();
            Group template = new MessageTemplate("Position",
                    new Field[] {
                    new Scalar("exchange", FASTType.STRING, Operator.COPY, ScalarValue.UNDEFINED, false)
                });
            ScalarValue value = new StringValue("NYSE");
            dictionary.Store(template, FastConstants.ANY_TYPE, new QName("exchange"), value);

            Assert.AreEqual(ScalarValue.UNDEFINED, dictionary.Lookup(template, new QName("bid"), FastConstants.ANY_TYPE));
        }
    }

}
