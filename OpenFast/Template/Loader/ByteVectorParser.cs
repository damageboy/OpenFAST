using System;
namespace OpenFAST.Template.Loader
{
	public class ByteVectorParser:VariableLengthScalarParser
	{
		public ByteVectorParser():base("byteVector")
		{
		}
		
		public override bool CanParse(System.Xml.XmlElement element, ParsingContext context)
		{
			return "byteVector".Equals(element.Name);
		}
	}
}