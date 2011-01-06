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

namespace OpenFAST.Sessions.Template.Exchange
{
    public class GroupConverter : AbstractFieldInstructionConverter
    {
        public override Group[] TemplateExchangeTemplates
        {
            get { return new Group[] {SessionControlProtocol11.GroupInstr}; }
        }

        public override Field Convert(GroupValue fieldDef, ITemplateRegistry templateRegistry, ConversionContext context)
        {
            string name = fieldDef.GetString("Name");
            string namespacetemp = "";
            IFieldValue retns;
            
            if (fieldDef.TryGetValue("Ns", out retns) && retns != null)
                namespacetemp = retns.ToString();

            Field[] fields = ParseFieldInstructions(fieldDef, templateRegistry, context);
            bool optional = fieldDef.GetBool("Optional");
            var group = new Group(new QName(name, namespacetemp), fields, optional);
            
            IFieldValue retTypeRef;
            if (fieldDef.TryGetValue("TypeRef", out retTypeRef) && retTypeRef != null)
            {
                var typeRef = (GroupValue) retTypeRef;
                String typeRefName = typeRef.GetString("Name");
                String typeRefNs = ""; // context.getNamespace();
                IFieldValue retNsTypeRef;
                if (typeRef.TryGetValue("Ns", out retNsTypeRef) && retNsTypeRef != null)
                    typeRefNs = retNsTypeRef.ToString();
                group.TypeReference = new QName(typeRefName, typeRefNs);
            }
            
            IFieldValue retAuxId;
            if (fieldDef.TryGetValue("AuxId", out retAuxId) && retAuxId != null)
                group.Id = retAuxId.ToString();
            
            return group;
        }

        public override GroupValue Convert(Field field, ConversionContext context)
        {
            var group = (Group) field;
            Message groupMsg = Convert(group, new Message(SessionControlProtocol11.GroupInstr), context);
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
            if (group.TypeReference != null && !FastConstants.AnyType.Equals(group.TypeReference))
            {
                var typeRef =
                    new GroupValue(
                        (Group) SessionControlProtocol11
                                    .TypeRef
                                    .GetField(new QName("TypeRef", SessionControlProtocol11.Namespace)));

                SetName(typeRef, group.TypeReference);
                groupMsg.SetFieldValue("TypeRef", typeRef);
            }

            var instructions = new SequenceValue(
                SessionControlProtocol11.TemplateDefinition.GetSequence("Instructions"));

            if (group.TypeReference != null && !FastConstants.AnyType.Equals(group.TypeReference))
            {
                var typeRef =
                    new GroupValue(
                        (Group)
                        SessionControlProtocol11
                            .TypeRef
                            .GetField(new QName("TypeRef", SessionControlProtocol11.Namespace)));

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
                IFieldInstructionConverter converter = context.GetConverter(fieldDef.Group);
                if (converter == null)
                    throw new SystemException(
                        string.Format("Encountered unknown group {0} while processing field instructions {1}",
                                      fieldDef.Group, groupDef.Group));

                fields[i] = converter.Convert(fieldDef, registry, context);
            }
            return fields;
        }
    }
}