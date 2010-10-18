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
using OpenFAST.Template;
using OpenFAST.Template.Operator;
using OpenFAST.Template.Type;

namespace OpenFAST.Session.Template.Exchange
{
    public class SequenceConverter : AbstractFieldInstructionConverter
    {
        public override Group[] TemplateExchangeTemplates
        {
            get { return new Group[] {SessionControlProtocol_1_1.SEQUENCE_INSTR}; }
        }

        public override Field Convert(GroupValue fieldDef, ITemplateRegistry templateRegistry, ConversionContext context)
        {
            string name = fieldDef.GetString("Name");
            string ns = fieldDef.GetString("Ns");
            var qname = new QName(name, ns);
            Field[] fields = GroupConverter.ParseFieldInstructions(fieldDef, templateRegistry, context);
            bool optional = fieldDef.GetBool("Optional");
            Scalar length = null;
            if (fieldDef.IsDefined("Length"))
            {
                GroupValue lengthDef = fieldDef.GetGroup("Length");
                QName lengthName;
                string id = null;
                if (lengthDef.IsDefined("Name"))
                {
                    GroupValue nameDef = lengthDef.GetGroup("Name");
                    lengthName = new QName(nameDef.GetString("Name"), nameDef.GetString("Ns"));
                    if (nameDef.IsDefined("AuxId"))
                        id = nameDef.GetString("AuxId");
                }
                else
                    lengthName = Global.CreateImplicitName(qname);
                Operator op = Operator.NONE;
                if (lengthDef.IsDefined("Operator"))
                    op = GetOperator(lengthDef.GetGroup("Operator").GetGroup(0).GetGroup());
                ScalarValue initialValue = ScalarValue.UNDEFINED;
                if (lengthDef.IsDefined("InitialValue"))
                    initialValue = (ScalarValue) lengthDef.GetValue("InitialValue");
                length = new Scalar(lengthName, FASTType.U32, op, initialValue, optional) {Id = id};
            }

            return new Sequence(qname, length, fields, optional);
        }

        public override GroupValue Convert(Field field, ConversionContext context)
        {
            var sequence = (Sequence) field;
            Message seqDef = GroupConverter.Convert(sequence.Group,
                                                    new Message(SessionControlProtocol_1_1.SEQUENCE_INSTR),
                                                    context);
            seqDef.SetBool("Optional", sequence.Optional);
            if (!sequence.ImplicitLength)
            {
                Group lengthGroup = SessionControlProtocol_1_1.SEQUENCE_INSTR.GetGroup("Length");
                var lengthDef = new GroupValue(lengthGroup);
                Scalar length = sequence.Length;
                var nameDef = new GroupValue(lengthGroup.GetGroup("Name"));
                SetNameAndId(length, nameDef);
                lengthDef.SetFieldValue("Name", nameDef);
                seqDef.SetFieldValue("Length", lengthDef);

                if (!length.Operator.Equals(Operator.NONE))
                {
                    var operatorDef = new GroupValue(lengthGroup.GetGroup("Operator"));
                    operatorDef.SetFieldValue(0, CreateOperator(length));
                    lengthDef.SetFieldValue("Operator", operatorDef);
                }

                if (!length.DefaultValue.Undefined)
                {
                    lengthDef.SetFieldValue("InitialValue", length.DefaultValue);
                }
            }
            return seqDef;
        }

        public override bool ShouldConvert(Field field)
        {
            return field.GetType().Equals(typeof (Sequence));
        }
    }
}