using System;
using QName = OpenFAST.QName;
using ScalarValue = OpenFAST.ScalarValue;
using ComposedScalar = OpenFAST.Template.ComposedScalar;
using Field = OpenFAST.Template.Field;
using Scalar = OpenFAST.Template.Scalar;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using FASTType = OpenFAST.Template.Type.FASTType;
using Util = OpenFAST.util.Util;

namespace OpenFAST.Template.Loader
{
	public class ComposedDecimalParser:AbstractFieldParser
	{
		
		public ComposedDecimalParser():base("decimal")
		{
		}
		
		public override bool CanParse(System.Xml.XmlElement element, ParsingContext context)
		{
			System.Xml.XmlNodeList children = element.ChildNodes;
			for (int i = 0; i < children.Count; i++)
			{
				string nodeName = children.Item(i).Name;
				if (nodeName.Equals("mantissa") || nodeName.Equals("exponent"))
					return true;
			}
			return false;
		}
		
		public override Field Parse(System.Xml.XmlElement fieldNode, bool optional, ParsingContext context)
		{
			System.Xml.XmlNodeList fieldChildren = fieldNode.ChildNodes;
			System.Xml.XmlNode mantissaNode = null;
			System.Xml.XmlNode exponentNode = null;
			
			for (int i = 0; i < fieldChildren.Count; i++)
			{
				if ("mantissa".Equals(fieldChildren.Item(i).Name))
				{
					mantissaNode = fieldChildren.Item(i);
				}
				else if ("exponent".Equals(fieldChildren.Item(i).Name))
				{
					exponentNode = fieldChildren.Item(i);
				}
			}
			return createComposedDecimal(fieldNode, context.GetName(), optional, mantissaNode, exponentNode, context);
		}
		
		private Field createComposedDecimal(System.Xml.XmlElement fieldNode, QName name, bool optional, System.Xml.XmlNode mantissaNode, System.Xml.XmlNode exponentNode, ParsingContext context)
		{
			string mantissaOperator = "none";
			string exponentOperator = "none";
			ScalarValue mantissaDefaultValue = ScalarValue.UNDEFINED;
			ScalarValue exponentDefaultValue = ScalarValue.UNDEFINED;
			QName mantissaKey = null;
			QName exponentKey = null;
			string mantissaDictionary = context.Dictionary;
			string exponentDictionary = context.Dictionary;
			string mantissaNamespace = context.Namespace;
			string exponentNamespace = context.Namespace;
			
			if ((mantissaNode != null) && mantissaNode.HasChildNodes)
			{
				System.Xml.XmlElement operatorElement = GetElement((System.Xml.XmlElement) mantissaNode, 1);
				mantissaOperator = operatorElement.Name;
				
				if (operatorElement.HasAttribute("value"))
					mantissaDefaultValue = FASTType.I64.GetValue(operatorElement.GetAttribute("value"));
				if (operatorElement.HasAttribute("ns"))
					mantissaNamespace = operatorElement.GetAttribute("ns");
				if (operatorElement.HasAttribute("key"))
					mantissaKey = new QName(operatorElement.GetAttribute("key"), mantissaNamespace);
				if (operatorElement.HasAttribute("dictionary"))
					mantissaDictionary = operatorElement.GetAttribute("dictionary");
			}
			
			if ((exponentNode != null) && exponentNode.HasChildNodes)
			{
				System.Xml.XmlElement operatorElement = GetElement((System.Xml.XmlElement) exponentNode, 1);
				exponentOperator = operatorElement.Name;
				
				if (operatorElement.HasAttribute("value"))
					exponentDefaultValue = FASTType.I32.GetValue(operatorElement.GetAttribute("value"));
				if (operatorElement.HasAttribute("ns"))
					exponentNamespace = operatorElement.GetAttribute("ns");
				if (operatorElement.HasAttribute("key"))
					exponentKey = new QName(operatorElement.GetAttribute("key"), exponentNamespace);
				if (operatorElement.HasAttribute("dictionary"))
					exponentDictionary = operatorElement.GetAttribute("dictionary");
			}
			
			ComposedScalar scalar = Util.ComposedDecimal(name, Operator.GetOperator(exponentOperator), exponentDefaultValue, Operator.GetOperator(mantissaOperator), mantissaDefaultValue, optional);
			
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