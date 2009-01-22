using System;
using Field = OpenFAST.Template.Field;

namespace OpenFAST.Template.Loader
{
	public interface FieldParser
	{
		Field Parse(System.Xml.XmlElement fieldNode, ParsingContext context);
		bool CanParse(System.Xml.XmlElement element, ParsingContext context);
	}
}