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
using OpenFAST.Template.Operators;
using OpenFAST.Utility;

namespace OpenFAST.Sessions.Template.Exchange
{
    public class ComposedDecimalConverter : AbstractFieldInstructionConverter
    {
        public override Group[] TemplateExchangeTemplates
        {
            get { return new Group[] {SessionControlProtocol11.CompDecimalInstr}; }
        }

        public override Field Convert(GroupValue fieldDef, ITemplateRegistry templateRegistry, ConversionContext context)
        {
            var name = new QName(fieldDef.GetString("Name"), fieldDef.GetString("Ns"));
            bool optional = fieldDef.GetBool("Optional");
            GroupValue exponentDef = fieldDef.GetGroup("Exponent");
            GroupValue exponentOperatorDef = exponentDef.GetGroup("Operator").GetGroup(0);
            Operator exponentOperator = GetOperator(exponentOperatorDef.Group);
            ScalarValue exponentDefaultValue = ScalarValue.Undefined;
            if (exponentDef.IsDefined("InitialValue"))
                exponentDefaultValue = new IntegerValue(exponentDef.GetInt("InitialValue"));
            GroupValue mantissaDef = fieldDef.GetGroup("Mantissa");
            GroupValue mantissaOperatorDef = mantissaDef.GetGroup("Operator").GetGroup(0);
            Operator mantissaOperator = GetOperator(mantissaOperatorDef.Group);
            ScalarValue mantissaDefaultValue = ScalarValue.Undefined;
            if (mantissaDef.IsDefined("InitialValue"))
                mantissaDefaultValue = new LongValue(mantissaDef.GetInt("InitialValue"));
            ComposedScalar composedDecimal = Util.ComposedDecimal(name, exponentOperator, exponentDefaultValue,
                                                                  mantissaOperator, mantissaDefaultValue, optional);
            if (fieldDef.IsDefined("AuxId"))
            {
                composedDecimal.Id = fieldDef.GetString("AuxId");
            }
            return composedDecimal;
        }

        public override GroupValue Convert(Field field, ConversionContext context)
        {
            var composedScalar = (ComposedScalar) field;
            var message = new Message(SessionControlProtocol11.CompDecimalInstr);
            SetNameAndId(field, message);
            message.SetInteger("Optional", field.IsOptional ? 1 : 0);
            GroupValue exponentDef = CreateComponent(composedScalar.Fields[0], "Exponent");
            GroupValue mantissaDef = CreateComponent(composedScalar.Fields[1], "Mantissa");
            message.SetFieldValue("Exponent", exponentDef);
            message.SetFieldValue("Mantissa", mantissaDef);
            return message;
        }

        private static GroupValue CreateComponent(Scalar component, string componentName)
        {
            Group componentGroup = SessionControlProtocol11.CompDecimalInstr.GetGroup(componentName);
            var componentDef = new GroupValue(componentGroup);
            GroupValue componentOperatorDef = CreateOperator(component);
            var componentOperatorGroup = new GroupValue(componentGroup.GetGroup("Operator"));
            componentDef.SetFieldValue("Operator", componentOperatorGroup);
            componentOperatorGroup.SetFieldValue(0, componentOperatorDef);
            if (!component.DefaultValue.IsUndefined)
                componentDef.SetInteger("InitialValue", component.DefaultValue.ToInt());
            return componentDef;
        }

        public override bool ShouldConvert(Field field)
        {
            return field.GetType().Equals(typeof (ComposedScalar));
        }
    }
}