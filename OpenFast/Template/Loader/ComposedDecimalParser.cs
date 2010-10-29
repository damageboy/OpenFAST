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
using System.Xml;
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;
using OpenFAST.Utility;

namespace OpenFAST.Template.Loader
{
    public class ComposedDecimalParser : AbstractFieldParser
    {
        public ComposedDecimalParser() : base("decimal")
        {
        }

        public override bool CanParse(XmlElement element, ParsingContext context)
        {
            XmlNodeList children = element.ChildNodes;
            for (int i = 0; i < children.Count; i++)
            {
                string nodeName = children.Item(i).LocalName;
                if (nodeName.Equals("mantissa") || nodeName.Equals("exponent"))
                    return true;
            }
            return false;
        }

        public override Field Parse(XmlElement fieldNode, bool optional, ParsingContext context)
        {
            XmlNodeList fieldChildren = fieldNode.ChildNodes;
            XmlNode mantissaNode = null;
            XmlNode exponentNode = null;

            for (int i = 0; i < fieldChildren.Count; i++)
            {
                if ("mantissa".Equals(fieldChildren.Item(i).LocalName))
                {
                    mantissaNode = fieldChildren.Item(i);
                }
                else if ("exponent".Equals(fieldChildren.Item(i).LocalName))
                {
                    exponentNode = fieldChildren.Item(i);
                }
            }
            return CreateComposedDecimal(fieldNode, context.Name, optional, mantissaNode, exponentNode, context);
        }

        private static Field CreateComposedDecimal(XmlElement fieldNode, QName name, bool optional,
                                                   XmlNode mantissaNode, XmlNode exponentNode,
                                                   ParsingContext context)
        {
            string mantissaOperator = "none";
            string exponentOperator = "none";
            ScalarValue mantissaDefaultValue = ScalarValue.Undefined;
            ScalarValue exponentDefaultValue = ScalarValue.Undefined;
            QName mantissaKey = null;
            QName exponentKey = null;
            string mantissaDictionary = context.Dictionary;
            string exponentDictionary = context.Dictionary;
            string mantissaNamespace = context.Namespace;
            string exponentNamespace = context.Namespace;

            if ((mantissaNode != null) && mantissaNode.HasChildNodes)
            {
                XmlElement operatorElement = GetElement((XmlElement) mantissaNode, 1);
                mantissaOperator = operatorElement.LocalName;

                if (operatorElement.HasAttribute("value"))
                    mantissaDefaultValue = FastType.I64.GetValue(operatorElement.GetAttribute("value"));
                if (operatorElement.HasAttribute("ns"))
                    mantissaNamespace = operatorElement.GetAttribute("ns");
                if (operatorElement.HasAttribute("key"))
                    mantissaKey = new QName(operatorElement.GetAttribute("key"), mantissaNamespace);
                if (operatorElement.HasAttribute("dictionary"))
                    mantissaDictionary = operatorElement.GetAttribute("dictionary");
            }

            if ((exponentNode != null) && exponentNode.HasChildNodes)
            {
                XmlElement operatorElement = GetElement((XmlElement) exponentNode, 1);
                exponentOperator = operatorElement.LocalName;

                if (operatorElement.HasAttribute("value"))
                    exponentDefaultValue = FastType.I32.GetValue(operatorElement.GetAttribute("value"));
                if (operatorElement.HasAttribute("ns"))
                    exponentNamespace = operatorElement.GetAttribute("ns");
                if (operatorElement.HasAttribute("key"))
                    exponentKey = new QName(operatorElement.GetAttribute("key"), exponentNamespace);
                if (operatorElement.HasAttribute("dictionary"))
                    exponentDictionary = operatorElement.GetAttribute("dictionary");
            }

            ComposedScalar scalar = Util.ComposedDecimal(name, Operator.GetOperator(exponentOperator),
                                                         exponentDefaultValue, Operator.GetOperator(mantissaOperator),
                                                         mantissaDefaultValue, optional);

            Scalar exponent = scalar.Fields[0];
            exponent.Dictionary = exponentDictionary;
            if (exponentKey != null)
                exponent.Key = exponentKey;

            Scalar mantissa = scalar.Fields[1];
            mantissa.Dictionary = mantissaDictionary;
            if (mantissaKey != null)
                mantissa.Key = mantissaKey;

            if (fieldNode.HasAttribute("id"))
                scalar.Id = fieldNode.GetAttribute("id");
            return scalar;
        }
    }
}