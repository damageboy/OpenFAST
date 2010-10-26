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
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;
using OpenFAST.Utility;

namespace OpenFAST.Sessions.Template.Exchange
{
    public class ScalarConverter : AbstractFieldInstructionConverter
    {
        private readonly Dictionary<Group, FastType> _templateTypeMap
            = new Dictionary<Group, FastType>
                  {
                      {SessionControlProtocol11.Int32Instr, FastType.I32},
                      {SessionControlProtocol11.Uint32Instr, FastType.U32},
                      {SessionControlProtocol11.Int64Instr, FastType.I64},
                      {SessionControlProtocol11.Uint64Instr, FastType.U64},
                      {SessionControlProtocol11.DecimalInstr, FastType.Decimal},
                      {SessionControlProtocol11.UnicodeInstr, FastType.Unicode},
                      {SessionControlProtocol11.AsciiInstr, FastType.Ascii},
                      {SessionControlProtocol11.ByteVectorInstr, FastType.ByteVector},
                  };

        private readonly Dictionary<FastType, Group> _typeTemplateMap;

        public ScalarConverter()
        {
            _typeTemplateMap = Util.ReverseDictionary(_templateTypeMap);
            _typeTemplateMap[FastType.String] = SessionControlProtocol11.AsciiInstr;
        }

        public override Group[] TemplateExchangeTemplates
        {
            get { return Util.ToArray(_templateTypeMap.Keys); }
        }

        public override Field Convert(GroupValue fieldDef, ITemplateRegistry templateRegistry, ConversionContext context)
        {
            FastType type = _templateTypeMap[fieldDef.Group];
            bool optional = fieldDef.GetBool("Optional");
            IFieldValue retInitialValue;
            ScalarValue initialValue = fieldDef.TryGetValue("InitialValue",out retInitialValue) && retInitialValue!=null
                                           ? (ScalarValue)retInitialValue
                                           : ScalarValue.Undefined;

            string name = fieldDef.GetString("Name");
            IFieldValue rettempNs;
            string tempNs = fieldDef.TryGetValue("Ns", out rettempNs) && rettempNs != null ? rettempNs.ToString() : "";
            var qname = new QName(name, tempNs);

            Scalar scalar;
            IFieldValue retOperator;
            if (fieldDef.TryGetValue("Operator", out retOperator) && retOperator!=null)
            {
                GroupValue operatorGroup = ((GroupValue)retOperator).GetGroup(0);
                Operator operatortemp = GetOperator(operatorGroup.Group);
                scalar = new Scalar(qname, type, operatortemp, initialValue, optional);
                IFieldValue retDictionary;
                if (operatorGroup.TryGetValue("Dictionary", out retDictionary) && retDictionary != null)
                    scalar.Dictionary = retDictionary.ToString();
                IFieldValue retKey;
                if (operatorGroup.TryGetValue("Key", out retKey) && retKey != null)
                {
                    GroupValue retOperatorGroup = (GroupValue) retKey;
                    string keyName = retOperatorGroup.GetString("Name");
                    string ns = retOperatorGroup.GetString("Ns");
                    scalar.Key = new QName(keyName, ns);
                }
            }
            else
            {
                scalar = new Scalar(qname, type, Operator.None, initialValue, optional);
            }
            IFieldValue retAuxId;
            if (fieldDef.TryGetValue("AuxId", out retAuxId) && retAuxId!=null)
            {
                scalar.Id = retAuxId.ToString();
            }
            return scalar;
        }

        public override GroupValue Convert(Field field, ConversionContext context)
        {
            var scalar = (Scalar) field;
            var scalarTemplate = (MessageTemplate) _typeTemplateMap[scalar.FASTType];
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