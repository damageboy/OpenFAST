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
using OpenFAST.Error;
using OpenFAST.Template;
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;

namespace OpenFAST.Sessions.Template.Exchange
{
    public class SequenceConverter : AbstractFieldInstructionConverter
    {
        public override Group[] TemplateExchangeTemplates
        {
            get { return new Group[] {SessionControlProtocol11.SequenceInstr}; }
        }

        public override Field Convert(GroupValue fieldDef, ITemplateRegistry templateRegistry, ConversionContext context)
        {
            string name = fieldDef.GetString("Name");
            string ns = fieldDef.GetString("Ns");
            var qname = new QName(name, ns);
            Field[] fields = GroupConverter.ParseFieldInstructions(fieldDef, templateRegistry, context);
            bool optional = fieldDef.GetBool("Optional");
            Scalar length = null;
            IFieldValue retLength;
            if (fieldDef.TryGetValue("Length", out retLength) && retLength!=null)
            {
                GroupValue lengthDef = (GroupValue)retLength;
                QName lengthName;
                string id = null;
                IFieldValue retName;
                if (lengthDef.TryGetValue("Name", out retName) && retName != null)
                {
                    GroupValue nameDef = (GroupValue)retName;
                    lengthName = new QName(nameDef.GetString("Name"), nameDef.GetString("Ns"));
                    IFieldValue retAuxId;
                    if (nameDef.TryGetValue("AuxId", out retAuxId) && retAuxId != null)
                        id = retAuxId.ToString();
                }
                else
                    lengthName = Global.CreateImplicitName(qname);
                Operator op = Operator.None;
                IFieldValue retOperator;
                if (lengthDef.TryGetValue("Operator", out retOperator) && retOperator != null)
                    op = GetOperator(((GroupValue)retOperator).GetGroup(0).Group);
                ScalarValue initialValue = ScalarValue.Undefined;
                IFieldValue retInitialValue;
                if (lengthDef.TryGetValue("InitialValue", out retInitialValue) && retInitialValue != null)
                    initialValue = (ScalarValue) retInitialValue;
                length = new Scalar(lengthName, FastType.U32, op, initialValue, optional) {Id = id};
            }

            var sequence = new Sequence(qname, length, fields, optional);
            IFieldValue retTypeRef;
            if (fieldDef.TryGetValue("TypeRef", out retTypeRef) && retTypeRef != null)
            {
                GroupValue typeRef = (GroupValue)retTypeRef;
                string typeRefName = typeRef.GetString("Name");
                string typeRefNs = ""; // context.getNamespace();
                IFieldValue rettypeRefNs;
                if (typeRef.TryGetValue("Ns", out rettypeRefNs) && rettypeRefNs!=null)
                    typeRefNs = rettypeRefNs.ToString();
                sequence.TypeReference = new QName(typeRefName, typeRefNs);
            }
            return sequence;
        }

        public override GroupValue Convert(Field field, ConversionContext context)
        {
            var sequence = (Sequence) field;
            Message seqDef = GroupConverter.Convert(
                sequence.Group,
                new Message(SessionControlProtocol11.SequenceInstr),
                context);

            seqDef.SetBool("Optional", sequence.IsOptional);
            if (!sequence.ImplicitLength)
            {
                Group lengthGroup = SessionControlProtocol11.SequenceInstr.GetGroup("Length");
                var lengthDef = new GroupValue(lengthGroup);
                Scalar length = sequence.Length;
                var nameDef = new GroupValue(lengthGroup.GetGroup("Name"));
                SetNameAndId(length, nameDef);
                lengthDef.SetFieldValue("Name", nameDef);
                seqDef.SetFieldValue("Length", lengthDef);

                if (!length.Operator.Equals(Operator.None))
                {
                    var operatorDef = new GroupValue(lengthGroup.GetGroup("Operator"));
                    operatorDef.SetFieldValue(0, CreateOperator(length));
                    lengthDef.SetFieldValue("Operator", operatorDef);
                }

                if (!length.DefaultValue.IsUndefined)
                {
                    lengthDef.SetFieldValue("InitialValue", length.DefaultValue);
                }
            }

            if (sequence.TypeReference != null && !FastConstants.AnyType.Equals(sequence.TypeReference))
            {
                var typeRef =
                    new GroupValue(
                        (Group)
                        SessionControlProtocol11.TypeRef.GetField(new QName("TypeRef",
                                                                              SessionControlProtocol11.Namespace)));
                SetName(typeRef, sequence.TypeReference);
                seqDef.SetFieldValue("TypeRef", typeRef);
            }

            return seqDef;
        }

        public override bool ShouldConvert(Field field)
        {
            return field.GetType().Equals(typeof (Sequence));
        }
    }
}