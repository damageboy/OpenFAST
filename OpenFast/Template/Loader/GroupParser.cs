using System;
using QName = OpenFAST.QName;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using System.Collections.Generic;

namespace OpenFAST.Template.Loader
{
	public class GroupParser:AbstractFieldParser
	{
		
		public GroupParser():base("group")
		{
		}
		
		public override Field Parse(System.Xml.XmlElement groupElement, bool optional, ParsingContext context)
		{
			Group group = new Group(context.GetName(), ParseFields(groupElement, context), optional);
			ParseMore(groupElement, group, context);
			return group;
		}
		
		protected internal static void  ParseMore(System.Xml.XmlElement groupElement, Group group, ParsingContext context)
		{
			group.ChildNamespace = context.Namespace;
			if (groupElement.HasAttribute("id"))
				group.Id = groupElement.GetAttribute("id");
			group.TypeReference = GetTypeReference(groupElement);
			ParseExternalAttributes(groupElement, group);
		}
		
		protected internal static Field[] ParseFields(System.Xml.XmlElement template, ParsingContext context)
		{
			System.Xml.XmlNodeList childNodes = template.ChildNodes;
			List<Field> fields = new List<Field>();
			
			for (int i = 0; i < childNodes.Count; i++)
			{
				System.Xml.XmlNode item = childNodes.Item(i);
				
				if (IsElement(item))
				{
					if ("typeRef".Equals(item.Name) || "length".Equals(item.Name))
						continue;
					System.Xml.XmlElement element = (System.Xml.XmlElement) item;
					FieldParser fieldParser = context.GetFieldParser(element);
					if (fieldParser == null)
						context.ErrorHandler.Error(OpenFAST.Error.FastConstants.PARSE_ERROR, "No parser registered for " + element.Name);
					fields.Add(fieldParser.Parse(element, context));
				}
			}
			
			return fields.ToArray();
		}
		
		protected internal static QName GetTypeReference(System.Xml.XmlElement templateTag)
		{
			string typeReference = null;
			string typeRefNs = "";
			System.Xml.XmlNodeList typeReferenceTags = templateTag.GetElementsByTagName("typeRef");
			
			if (typeReferenceTags.Count > 0)
			{
				System.Xml.XmlElement messageRef = (System.Xml.XmlElement) typeReferenceTags.Item(0);
				typeReference = messageRef.GetAttribute("name");
				if (messageRef.HasAttribute("ns"))
					typeRefNs = messageRef.GetAttribute("ns");
				return new QName(typeReference, typeRefNs);
			}
			else
			{
				return OpenFAST.Error.FastConstants.ANY_TYPE;
			}
		}
	}
}