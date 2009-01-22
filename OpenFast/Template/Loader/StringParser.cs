using System;
namespace OpenFAST.Template.Loader
{
	public class StringParser:VariableLengthScalarParser
	{
		
		public StringParser():base("string")
		{
		}
		
		public override bool CanParse(System.Xml.XmlElement element, ParsingContext context)
		{
			return element.Name.Equals("string");
		}
		
		protected internal override string GetTypeName(System.Xml.XmlElement fieldNode)
		{
			if (fieldNode.HasAttribute("charset"))
				return fieldNode.GetAttribute("charset");
			return "ascii";
		}
	}
}