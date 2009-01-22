using System;
using QName = OpenFAST.QName;
using Field = OpenFAST.Template.Field;

namespace OpenFAST.Template.Loader
{
	public abstract class AbstractFieldParser : FieldParser
	{
		private string[] parseableNodeNames;
		
		protected internal AbstractFieldParser(string nodeName):this(new string[]{nodeName})
		{
		}
		
		protected internal AbstractFieldParser(string[] nodeNames)
		{
			this.parseableNodeNames = nodeNames;
		}
		
		public virtual bool CanParse(System.Xml.XmlElement element, ParsingContext context)
		{
			for (int i = 0; i < parseableNodeNames.Length; i++)
				if (parseableNodeNames[i].Equals(element.Name))
					return true;
			return false;
		}
		
		public Field Parse(System.Xml.XmlElement fieldNode, ParsingContext parent)
		{
			bool optional = "optional".Equals(fieldNode.GetAttribute("presence"));
			return Parse(fieldNode, optional, new ParsingContext(fieldNode, parent));
		}
		
		public abstract Field Parse(System.Xml.XmlElement fieldNode, bool optional, ParsingContext context);
		
		protected internal static void  ParseExternalAttributes(System.Xml.XmlElement element, Field field)
		{
			System.Xml.XmlNamedNodeMap attributes = (System.Xml.XmlAttributeCollection) element.Attributes;
			for (int i = 0; i < attributes.Count; i++)
			{
				System.Xml.XmlAttribute attribute = (System.Xml.XmlAttribute) attributes.Item(i);
				if (attribute.NamespaceURI == null || attribute.NamespaceURI.Equals("") || attribute.NamespaceURI.Equals(XMLMessageTemplateLoader.TEMPLATE_DEFINITION_NS))
					continue;
				field.AddAttribute(new QName(attribute.LocalName, attribute.NamespaceURI), attribute.Value);
			}
		}
		

		protected internal static System.Xml.XmlElement GetElement(System.Xml.XmlElement fieldNode, int elementIndex)
		{
			System.Xml.XmlNodeList children = fieldNode.ChildNodes;
			int elemIndex = 0;
			for (int i = 0; i < children.Count; i++)
			{
				System.Xml.XmlNode item = children.Item(i);
				if (IsElement(item))
				{
					elemIndex++;
					if (elemIndex == elementIndex)
						return ((System.Xml.XmlElement) item);
				}
			}
			
			return null;
		}

		protected internal static bool IsElement(System.Xml.XmlNode item)
		{
			return System.Convert.ToInt16(item.NodeType) == (short) System.Xml.XmlNodeType.Element;
		}
	}
}