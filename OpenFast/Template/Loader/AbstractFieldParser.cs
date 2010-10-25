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
using System;
using System.Linq;
using System.Xml;

namespace OpenFAST.Template.Loader
{
    public abstract class AbstractFieldParser : IFieldParser
    {
        private readonly string[] _parseableNodeNames;

        protected internal AbstractFieldParser(string nodeName) : this(new[] {nodeName})
        {
        }

        protected internal AbstractFieldParser(string[] nodeNames)
        {
            _parseableNodeNames = nodeNames;
        }

        #region IFieldParser Members

        public virtual bool CanParse(XmlElement element, ParsingContext context)
        {
            return _parseableNodeNames.Any(t => t.Equals(element.Name));
        }

        public Field Parse(XmlElement fieldNode, ParsingContext parent)
        {
            bool optional = "optional".Equals(fieldNode.GetAttribute("presence"));
            return Parse(fieldNode, optional, new ParsingContext(fieldNode, parent));
        }

        #endregion

        public abstract Field Parse(XmlElement fieldNode, bool optional, ParsingContext context);

        protected static void ParseExternalAttributes(XmlElement element, Field field)
        {
            XmlNamedNodeMap attributes = element.Attributes;
            for (int i = 0; i < attributes.Count; i++)
            {
                var attribute = (XmlAttribute) attributes.Item(i);
                if (string.IsNullOrEmpty(attribute.NamespaceURI) ||
                    attribute.NamespaceURI.Equals(XmlMessageTemplateLoader.TemplateDefinitionNs))
                    continue;
                field.AddAttribute(new QName(attribute.LocalName, attribute.NamespaceURI), attribute.Value);
            }
        }

        protected static XmlElement GetElement(XmlElement fieldNode, int elementIndex)
        {
            XmlNodeList children = fieldNode.ChildNodes;
            int elemIndex = 0;
            for (int i = 0; i < children.Count; i++)
            {
                XmlNode item = children.Item(i);
                if (IsElement(item))
                {
                    elemIndex++;
                    if (elemIndex == elementIndex)
                        return (XmlElement) item;
                }
            }

            return null;
        }

        protected static bool IsElement(XmlNode item)
        {
            return Convert.ToInt16(item.NodeType) == (short) XmlNodeType.Element;
        }
    }
}