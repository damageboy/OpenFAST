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
using OpenFAST.Error;
using OpenFAST.Template;
using Type = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Session.Template.Exchange
{
    public class VariableLengthInstructionConverter : ScalarConverter
    {
        public override Group[] TemplateExchangeTemplates
        {
            get { return new[] {SessionControlProtocol_1_1.BYTE_VECTOR_INSTR, SessionControlProtocol_1_1.UNICODE_INSTR}; }
        }

        public override Field Convert(GroupValue fieldDef, ITemplateRegistry templateRegistry, ConversionContext context)
        {
            var scalar = (Scalar) base.Convert(fieldDef, templateRegistry, context);
            if (fieldDef.IsDefined("Length"))
                scalar.AddAttribute(FastConstants.LENGTH_FIELD, fieldDef.GetGroup("Length").GetString("Name"));
            return scalar;
        }

        public override GroupValue Convert(Field field, ConversionContext context)
        {
            var scalar = (Scalar) field;
            GroupValue fieldDef = base.Convert(field, context);
            string value;
            if (scalar.TryGetAttribute(FastConstants.LENGTH_FIELD, out value))
            {
                var lengthDef = new GroupValue(fieldDef.GetGroup().GetGroup("Length"));
                lengthDef.SetString("Name", value);
                fieldDef.SetFieldValue("Length", lengthDef);
            }
            return fieldDef;
        }

        public override bool ShouldConvert(Field field)
        {
            if (!field.GetType().Equals(typeof (Scalar)))
                return false;
            Type type = ((Scalar) field).Type;
            return type.Equals(Type.BYTE_VECTOR) || type.Equals(Type.UNICODE);
        }
    }
}