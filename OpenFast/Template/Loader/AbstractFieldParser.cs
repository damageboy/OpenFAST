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
namespace OpenFAST.Template.Loader
{
	public abstract class AbstractFieldParser : FieldParser
	{
		private readonly string[] parseableNodeNames;
		
		protected internal AbstractFieldParser(string nodeName):this(new[]{nodeName})
		{
		}
		
		protected internal AbstractFieldParser(string[] nodeNames)
		{
			parseableNodeNames = nodeNames;
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
			System.Xml.XmlNamedNodeMap attributes = element.Attributes;
			for (int i = 0; i < attributes.Count; i++)
			{
				var attribute = (System.Xml.XmlAttribute) attributes.Item(i);
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