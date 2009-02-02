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
