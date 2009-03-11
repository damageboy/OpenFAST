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
using Operator = openfast.Template.Operator.Operator;
using FASTType = OpenFAST.Template.Type.FASTType;
using Util = OpenFAST.util.Util;

namespace OpenFAST.Template.Loader
{
	public class ScalarParser:AbstractFieldParser
	{
		
		public ScalarParser(string[] nodeNames):base(nodeNames)
		{
		}
		
		public ScalarParser(string nodeName):base(nodeName)
		{
		}
		
		public ScalarParser():base(new string[]{})
		{
		}
		
		public override bool CanParse(System.Xml.XmlElement element, ParsingContext context)
		{
			return context.TypeMap.Contains(GetTypeName(element));
		}
		
		public override Field Parse(System.Xml.XmlElement fieldNode, bool optional, ParsingContext context)
		{
			Operator operator_Renamed = Operator.NONE;
			string defaultValue = null;
			string key = null;
			string ns = "";
			System.Xml.XmlElement operatorElement = GetOperatorElement(fieldNode);
			if (operatorElement != null)
			{
				if (operatorElement.HasAttribute("value"))
					defaultValue = operatorElement.GetAttribute("value");
				operator_Renamed = Operator.GetOperator(operatorElement.Name);
				if (operatorElement.HasAttribute("key"))
					key = operatorElement.GetAttribute("key");
				if (operatorElement.HasAttribute("ns"))
					ns = operatorElement.GetAttribute("ns");
				if (operatorElement.HasAttribute("dictionary"))
					context.Dictionary = operatorElement.GetAttribute("dictionary");
			}
			FASTType type = GetType(fieldNode, context);
			Scalar scalar = new Scalar(GetName(fieldNode, context), type, operator_Renamed, type.GetValue(defaultValue), optional);
			if (fieldNode.HasAttribute("id"))
				scalar.Id = fieldNode.GetAttribute("id");
			if (key != null)
				scalar.Key = new QName(key, ns);
			scalar.Dictionary = context.Dictionary;
			ParseExternalAttributes(fieldNode, scalar);
			return scalar;
		}
		
		protected internal virtual QName GetName(System.Xml.XmlElement fieldNode, ParsingContext context)
		{
			return context.GetName();
		}
		
		protected internal virtual FASTType GetType(System.Xml.XmlElement fieldNode, ParsingContext context)
		{
			string typeName = GetTypeName(fieldNode);
			if (!context.TypeMap.Contains(typeName))
			{
				context.ErrorHandler.Error(XMLMessageTemplateLoader.INVALID_TYPE, "The type " + typeName + " is not defined.  Possible types: " + Util.CollectionToString(new SupportClass.HashSetSupport(context.TypeMap.Keys), ", "));
			}
			return (FASTType) context.TypeMap[typeName];
		}
		
		protected internal virtual string GetTypeName(System.Xml.XmlElement fieldNode)
		{
			return fieldNode.Name;
		}
		
		protected internal virtual System.Xml.XmlElement GetOperatorElement(System.Xml.XmlElement fieldNode)
		{
			return GetElement(fieldNode, 1);
		}
	}
}