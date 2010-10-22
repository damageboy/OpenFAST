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
                      {SessionControlProtocol11.Int32Instr, FASTType.I32},
                      {SessionControlProtocol11.Uint32Instr, FASTType.U32},
                      {SessionControlProtocol11.Int64Instr, FASTType.I64},
                      {SessionControlProtocol11.Uint64Instr, FASTType.U64},
                      {SessionControlProtocol11.DecimalInstr, FASTType.Decimal},
                      {SessionControlProtocol11.UnicodeInstr, FASTType.Unicode},
                      {SessionControlProtocol11.AsciiInstr, FASTType.Ascii},
                      {SessionControlProtocol11.ByteVectorInstr, FASTType.ByteVector},
                  };

        private readonly Dictionary<FASTType, Group> _typeTemplateMap;

        public ScalarConverter()
        {
            _typeTemplateMap = Util.ReverseDictionary(_templateTypeMap);
            _typeTemplateMap[FASTType.String] = SessionControlProtocol11.AsciiInstr;
        }

        public override Group[] TemplateExchangeTemplates
        {
            get { return Util.ToArray(_templateTypeMap.Keys); }
        }

        public override Field Convert(GroupValue fieldDef, ITemplateRegistry templateRegistry, ConversionContext context)
        {
            FASTType type = _templateTypeMap[fieldDef.GetGroup()];
            bool optional = fieldDef.GetBool("Optional");
            ScalarValue initialValue = fieldDef.IsDefined("InitialValue")
                                           ? (ScalarValue) fieldDef.GetValue("InitialValue")
                                           : ScalarValue.Undefined;

            string name = fieldDef.GetString("Name");
            string tempNs = fieldDef.IsDefined("Ns") ? fieldDef.GetString("Ns") : "";
            var qname = new QName(name, tempNs);

            Scalar scalar;
            if (fieldDef.IsDefined("Operator"))
            {
                GroupValue operatorGroup = fieldDef.GetGroup("Operator").GetGroup(0);
                Operator operatortemp = GetOperator(operatorGroup.GetGroup());
                scalar = new Scalar(qname, type, operatortemp, initialValue, optional);
                if (operatorGroup.IsDefined("Dictionary"))
                    scalar.Dictionary = operatorGroup.GetString("Dictionary");
                if (operatorGroup.IsDefined("Key"))
                {
                    string keyName = operatorGroup.GetGroup("Key").GetString("Name");
                    string ns = operatorGroup.GetGroup("Key").GetString("Ns");
                    scalar.Key = new QName(keyName, ns);
                }
            }
            else
            {
                scalar = new Scalar(qname, type, Operator.None, initialValue, optional);
            }
            if (fieldDef.IsDefined("AuxId"))
            {
                scalar.Id = fieldDef.GetString("AuxId");
            }
            return scalar;
        }

        public override GroupValue Convert(Field field, ConversionContext context)
        {
            var scalar = (Scalar) field;
            var scalarTemplate = (MessageTemplate) _typeTemplateMap[scalar.Type];
            var scalarMsg = new Message(scalarTemplate);
            SetNameAndId(scalar, scalarMsg);
            scalarMsg.SetInteger("Optional", scalar.IsOptional ? 1 : 0);

            if (!scalar.Operator.Equals(Operator.None))
                scalarMsg.SetFieldValue(
                    "Operator",
                    new GroupValue(scalarTemplate.GetGroup("Operator"), new IFieldValue[] {CreateOperator(scalar)}));

            if (!scalar.DefaultValue.IsUndefined)
                scalarMsg.SetFieldValue("InitialValue", scalar.DefaultValue);

            return scalarMsg;
        }

        public override bool ShouldConvert(Field field)
        {
            return field.GetType().Equals(typeof (Scalar));
        }
    }
}