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
			var group = new Group(context.GetName(), ParseFields(groupElement, context), optional);
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
			var fields = new List<Field>();
			
			for (int i = 0; i < childNodes.Count; i++)
			{
				System.Xml.XmlNode item = childNodes.Item(i);
				
				if (IsElement(item))
				{
					if ("typeRef".Equals(item.Name) || "length".Equals(item.Name))
						continue;
					var element = (System.Xml.XmlElement) item;
					FieldParser fieldParser = context.GetFieldParser(element);
					if (fieldParser == null)
						context.ErrorHandler.Error(Error.FastConstants.PARSE_ERROR, "No parser registered for " + element.Name);
				    if (fieldParser != null) fields.Add(fieldParser.Parse(element, context));
				}
			}
			
			return fields.ToArray();
		}
		
		protected internal static QName GetTypeReference(System.Xml.XmlElement templateTag)
		{
			
			string typeRefNs = "";
			System.Xml.XmlNodeList typeReferenceTags = templateTag.GetElementsByTagName("typeRef");
			
			if (typeReferenceTags.Count > 0)
			{
			    var messageRef = (System.Xml.XmlElement) typeReferenceTags.Item(0);
				string typeReference = messageRef.GetAttribute("name");
				if (messageRef.HasAttribute("ns"))
					typeRefNs = messageRef.GetAttribute("ns");
				return new QName(typeReference, typeRefNs);
			}
		    return Error.FastConstants.ANY_TYPE;
		}
	}
}