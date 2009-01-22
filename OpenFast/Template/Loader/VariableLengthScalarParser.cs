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