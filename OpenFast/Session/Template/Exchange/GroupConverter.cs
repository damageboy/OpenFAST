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
using OpenFAST.Error;
using OpenFAST.Template;

namespace OpenFAST.Session.Template.Exchange
{
    public class GroupConverter : AbstractFieldInstructionConverter
    {
        public override Group[] TemplateExchangeTemplates
        {
            get { return new Group[] {SessionControlProtocol_1_1.GROUP_INSTR}; }
        }

        public override Field Convert(GroupValue fieldDef, ITemplateRegistry templateRegistry, ConversionContext context)
        {
            string name = fieldDef.GetString("Name");
            string namespacetemp = "";
            if (fieldDef.IsDefined("Ns"))
                namespacetemp = fieldDef.GetString("Ns");
            Field[] fields = ParseFieldInstructions(fieldDef, templateRegistry, context);
            bool optional = fieldDef.GetBool("Optional");
            Group group = new Group(new QName(name, namespacetemp), fields, optional);
            if (fieldDef.IsDefined("TypeRef")) {
                GroupValue typeRef = fieldDef.GetGroup("TypeRef");
                String typeRefName = typeRef.GetString("Name");
                String typeRefNs = ""; // context.getNamespace();
                if (typeRef.IsDefined("Ns"))
                    typeRefNs = typeRef.GetString("Ns");
                group.SetTypeReference(new QName(typeRefName, typeRefNs));
            }
            if (fieldDef.IsDefined("AuxId")) {
                group.Id= fieldDef.GetString("AuxId");
            }
            return group;
        }

        public override GroupValue Convert(Field field, ConversionContext context)
        {
            var group = (Group) field;
            Message groupMsg = Convert(group, new Message(SessionControlProtocol_1_1.GROUP_INSTR), context);
            groupMsg.SetBool("Optional", field.IsOptional);
            return groupMsg;
        }

        public override bool ShouldConvert(Field field)
        {
            return field.GetType().Equals(typeof (Group));
        }

        public static Message Convert(Group group, Message groupMsg, ConversionContext context)
        {
            SetNameAndId(group, groupMsg);
            if (group.TypeReference != null && !FastConstants.ANY_TYPE.Equals(group.TypeReference))
            {
                GroupValue typeRef =
                    new GroupValue(
                        (Group)
                        SessionControlProtocol_1_1.TypeRef.GetField(new QName("TypeRef",
                                                                              SessionControlProtocol_1_1.NAMESPACE)));
                SetName(typeRef, group.TypeReference);
                groupMsg.SetFieldValue("TypeRef", typeRef);
            }

            var instructions = new SequenceValue(
                SessionControlProtocol_1_1.TEMPLATE_DEFINITION.GetSequence("Instructions"));
            
            if (group.TypeReference != null && !OpenFAST.Error.FastConstants.ANY_TYPE.Equals(group.TypeReference))
            {
                GroupValue typeRef = new GroupValue((Group)SessionControlProtocol_1_1.TypeRef.GetField(new QName("TypeRef", SessionControlProtocol_1_1.NAMESPACE)));
                SetName(typeRef, group.TypeReference);
                groupMsg.SetFieldValue("TypeRef", typeRef);
            }

            Field[] fields = group.FieldDefinitions;
            for (int i = group is MessageTemplate ? 1 : 0; i < fields.Length; i++)
            {
                Field field = fields[i];
                
                IFieldInstructionConverter converter = context.GetConverter(field);
                if (converter == null)
                    throw new InvalidOperationException("No converter found for type " + field.GetType());

                IFieldValue v = converter.Convert(field, context);
                instructions.Add(new[] {v});
            }
            groupMsg.SetFieldValue("Instructions", instructions);
            return groupMsg;
        }

        public static Field[] ParseFieldInstructions(GroupValue groupDef, ITemplateRegistry registry,
                                                     ConversionContext context)
        {
            SequenceValue instructions = groupDef.GetSequence("Instructions");
            var fields = new Field[instructions.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                GroupValue fieldDef = instructions[i].GetGroup(0);
                IFieldInstructionConverter converter = context.GetConverter(fieldDef.GetGroup());
                if (converter == null)
                {
                    throw new SystemException("Encountered unknown group " + fieldDef.GetGroup() +
                                              "while processing field instructions " + groupDef.GetGroup());
                }
                fields[i] = converter.Convert(fieldDef, registry, context);
            }
            return fields;
        }
    }
}