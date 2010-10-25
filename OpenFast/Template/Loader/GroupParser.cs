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
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using System.Collections.Generic;
using System.Xml;
using OpenFAST.Error;

namespace OpenFAST.Template.Loader
{
    public class GroupParser : AbstractFieldParser
    {
        public GroupParser() : base("group")
        {
        }

        public override Field Parse(XmlElement groupElement, bool optional, ParsingContext context)
        {
            var group = new Group(context.Name, ParseFields(groupElement, context), optional);
            ParseMore(groupElement, group, context);
            return group;
        }

        protected internal static void ParseMore(XmlElement groupElement, Group group, ParsingContext context)
        {
            group.ChildNamespace = context.Namespace;
            if (groupElement.HasAttribute("id"))
                group.Id = groupElement.GetAttribute("id");
            group.TypeReference = GetTypeReference(groupElement);
            ParseExternalAttributes(groupElement, group);
        }

        protected internal static Field[] ParseFields(XmlElement template, ParsingContext context)
        {
            XmlNodeList childNodes = template.ChildNodes;
            var fields = new List<Field>();

            for (int i = 0; i < childNodes.Count; i++)
            {
                XmlNode item = childNodes.Item(i);

                if (IsElement(item))
                {
                    if ("typeRef".Equals(item.Name) || "length".Equals(item.Name))
                        continue;
                    var element = (XmlElement) item;
                    IFieldParser fieldParser = context.GetFieldParser(element);
                    if (fieldParser == null)
                        context.ErrorHandler.OnError(null, DynError.ParseError, "No parser registered for {0}",
                                                     element.Name);
                    if (fieldParser != null) fields.Add(fieldParser.Parse(element, context));
                }
            }

            return fields.ToArray();
        }

        private static QName GetTypeReference(XmlElement templateTag)
        {
            string typeRefNs = "";
            XmlNodeList typeReferenceTags = templateTag.GetElementsByTagName("typeRef");

            if (typeReferenceTags.Count > 0)
            {
                var messageRef = (XmlElement) typeReferenceTags.Item(0);
                string typeReference = messageRef.GetAttribute("name");
                if (messageRef.HasAttribute("ns"))
                    typeRefNs = messageRef.GetAttribute("ns");
                return new QName(typeReference, typeRefNs);
            }
            return FastConstants.AnyType;
        }
    }
}