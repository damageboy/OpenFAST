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
using OpenFAST.Template;

namespace OpenFAST.Session.Template.Exchange
{
    public class StaticTemplateReferenceConverter : AbstractFieldInstructionConverter
    {
        public override Group[] TemplateExchangeTemplates
        {
            get { return new Group[] {SessionControlProtocol_1_1.STAT_TEMP_REF_INSTR}; }
        }

        public override Field Convert(GroupValue fieldDef, ITemplateRegistry templateRegistry, ConversionContext context)
        {
            var name = new QName(fieldDef.GetString("Name"), fieldDef.GetString("Ns"));
            MessageTemplate template;
            if (templateRegistry.TryGetTemplate(name, out template))
                return new StaticTemplateReference(template);

            throw new ArgumentOutOfRangeException("fieldDef", name, "Referenced template name not defined.");
        }

        public override GroupValue Convert(Field field, ConversionContext context)
        {
            var strDef = new Message(SessionControlProtocol_1_1.STAT_TEMP_REF_INSTR);
            SetNameAndId(field, strDef);
            return strDef;
        }

        public override bool ShouldConvert(Field field)
        {
            return field.GetType().Equals(typeof (StaticTemplateReference));
        }
    }
}