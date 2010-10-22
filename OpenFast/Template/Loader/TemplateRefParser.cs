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
using System.Xml;
using OpenFAST.Error;

namespace OpenFAST.Template.Loader
{
    public class TemplateRefParser : IFieldParser
    {
        #region IFieldParser Members

        public virtual Field Parse(XmlElement element, ParsingContext context)
        {
            if (element.HasAttribute("name"))
            {
                QName templateName = element.HasAttribute("templateNs")
                                         ? new QName(element.GetAttribute("name"), element.GetAttribute("templateNs"))
                                         : new QName(element.GetAttribute("name"), "");

                MessageTemplate template;
                if (context.TemplateRegistry.TryGetTemplate(templateName, out template))
                    return new StaticTemplateReference(template);

                context.ErrorHandler.Error(FastConstants.D8TemplateNotExist,
                                           "The template '" + templateName + "' was not found.");
                return null;
            }
            return DynamicTemplateReference.INSTANCE;
        }

        public virtual bool CanParse(XmlElement element, ParsingContext context)
        {
            return "templateRef".Equals(element.Name);
        }

        #endregion
    }
}