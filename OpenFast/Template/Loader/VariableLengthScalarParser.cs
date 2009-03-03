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
using Field = OpenFAST.Template.Field;
using Scalar = OpenFAST.Template.Scalar;

namespace OpenFAST.Template.Loader
{
	public class VariableLengthScalarParser:ScalarParser
	{
		
		public VariableLengthScalarParser(string nodeName):base(nodeName)
		{
		}
		
		public override Field Parse(System.Xml.XmlElement fieldNode, bool optional, ParsingContext context)
		{
			Scalar scalar = (Scalar) base.Parse(fieldNode, optional, context);
			System.Xml.XmlElement element = GetElement(fieldNode, 1);
			if (element != null && element.Name.Equals("length"))
			{
				string length = element.GetAttribute("name");
				scalar.AddAttribute(OpenFAST.Error.FastConstants.LENGTH_FIELD, length);
			}
			return scalar;
		}
		
		protected internal override System.Xml.XmlElement GetOperatorElement(System.Xml.XmlElement fieldNode)
		{
			System.Xml.XmlElement operatorElement = base.GetOperatorElement(fieldNode);
			if (operatorElement != null && operatorElement.Name.Equals("length"))
				return GetElement(fieldNode, 2);
			return operatorElement;
		}
	}
}