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
using OpenFAST.Template;
using OpenFAST.Template.Operator;
using OpenFAST.Template.Type;
using OpenFAST.Utility;

namespace OpenFAST.Session.Template.Exchange
{
    public class ScalarConverter : AbstractFieldInstructionConverter
    {
        private readonly Dictionary<Group, FASTType> _templateTypeMap
            = new Dictionary<Group, FASTType>
                  {
                      {SessionControlProtocol_1_1.INT32_INSTR, FASTType.I32},
                      {SessionControlProtocol_1_1.UINT32_INSTR, FASTType.U32},
                      {SessionControlProtocol_1_1.INT64_INSTR, FASTType.I64},
                      {SessionControlProtocol_1_1.UINT64_INSTR, FASTType.U64},
                      {SessionControlProtocol_1_1.DECIMAL_INSTR, FASTType.DECIMAL},
                      {SessionControlProtocol_1_1.UNICODE_INSTR, FASTType.UNICODE},
                      {SessionControlProtocol_1_1.ASCII_INSTR, FASTType.ASCII},
                      {SessionControlProtocol_1_1.BYTE_VECTOR_INSTR, FASTType.BYTE_VECTOR},
                  };

        private readonly Dictionary<FASTType, Group> _typeTemplateMap;

        public ScalarConverter()
        {
            _typeTemplateMap = Util.ReverseDictionary(_templateTypeMap);
            _typeTemplateMap[FASTType.STRING] = SessionControlProtocol_1_1.ASCII_INSTR;
        }

        public override Group[] TemplateExchangeTemplates
        {
            get { return Util.ToArray(_templateTypeMap.Keys); }
        }

        public override Field Convert(GroupValue fieldDef, ITemplateRegistry templateRegistry, ConversionContext context)
        {
            FASTType type = _templateTypeMap[fieldDef.GetGroup()];
            bool optional = fieldDef.GetBool("Optional");
            ScalarValue initialValue = ScalarValue.UNDEFINED;
            if (fieldDef.IsDefined("InitialValue"))
                initialValue = (ScalarValue) fieldDef.GetValue("InitialValue");

            if (fieldDef.IsDefined("Operator"))
            {
                GroupValue operatorGroup = fieldDef.GetGroup("Operator").GetGroup(0);
                Operator op = GetOperator(operatorGroup.GetGroup());
                var scalar = new Scalar(fieldDef.GetString("Name"), type, op, initialValue, optional);
                if (operatorGroup.IsDefined("Dictionary"))
                    scalar.Dictionary = operatorGroup.GetString("Dictionary");
                if (operatorGroup.IsDefined("Key"))
                {
                    string name = operatorGroup.GetGroup("Key").GetString("Name");
                    string ns = operatorGroup.GetGroup("Key").GetString("Ns");
                    scalar.Key = new QName(name, ns);
                }
                return scalar;
            }
            return new Scalar(fieldDef.GetString("Name"), type, Operator.NONE, initialValue, optional);
        }

        public override GroupValue Convert(Field field, ConversionContext context)
        {
            var scalar = (Scalar) field;
            var scalarTemplate = (MessageTemplate) _typeTemplateMap[scalar.Type];
            var scalarMsg = new Message(scalarTemplate);
            SetNameAndId(scalar, scalarMsg);
            scalarMsg.SetInteger("Optional", scalar.Optional ? 1 : 0);
            
            if (!scalar.Operator.Equals(Operator.NONE))
                scalarMsg.SetFieldValue(
                    "Operator",
                    new GroupValue(scalarTemplate.GetGroup("Operator"), new IFieldValue[] {CreateOperator(scalar)}));

            if (!scalar.DefaultValue.Undefined)
                scalarMsg.SetFieldValue("InitialValue", scalar.DefaultValue);

            return scalarMsg;
        }

        public override bool ShouldConvert(Field field)
        {
            return field.GetType().Equals(typeof (Scalar));
        }
    }
}