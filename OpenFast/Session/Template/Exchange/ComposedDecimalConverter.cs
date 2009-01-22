using System;
using GroupValue = OpenFAST.GroupValue;
using IntegerValue = OpenFAST.IntegerValue;
using Message = OpenFAST.Message;
using QName = OpenFAST.QName;
using ScalarValue = OpenFAST.ScalarValue;
using SessionControlProtocol_1_1 = OpenFAST.Session.SessionControlProtocol_1_1;
using ComposedScalar = OpenFAST.Template.ComposedScalar;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using LongValue = OpenFAST.Template.LongValue;
using Scalar = OpenFAST.Template.Scalar;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using Util = OpenFAST.util.Util;

namespace OpenFAST.Session.Template.Exchange
{
	public class ComposedDecimalConverter:AbstractFieldInstructionConverter
	{
		override public Group[] TemplateExchangeTemplates
		{
			get
			{
				return new Group[]{SessionControlProtocol_1_1.COMP_DECIMAL_INSTR};
			}
			
		}
		public override Field Convert(GroupValue fieldDef, TemplateRegistry templateRegistry, ConversionContext context)
		{
			QName name = new QName(fieldDef.GetString("Name"), fieldDef.GetString("Ns"));
			bool optional = fieldDef.GetBool("Optional");
			GroupValue exponentDef = fieldDef.GetGroup("Exponent");
			GroupValue exponentOperatorDef = exponentDef.GetGroup("Operator").GetGroup(0);
			Operator exponentOperator = GetOperator(exponentOperatorDef.GetGroup());
			ScalarValue exponentDefaultValue = ScalarValue.UNDEFINED;
			if (exponentDef.IsDefined("InitialValue"))
				exponentDefaultValue = new IntegerValue(exponentDef.GetInt("InitialValue"));
			GroupValue mantissaDef = fieldDef.GetGroup("Mantissa");
			GroupValue mantissaOperatorDef = mantissaDef.GetGroup("Operator").GetGroup(0);
			Operator mantissaOperator = GetOperator(mantissaOperatorDef.GetGroup());
			ScalarValue mantissaDefaultValue = ScalarValue.UNDEFINED;
			if (mantissaDef.IsDefined("InitialValue"))
				mantissaDefaultValue = new LongValue(mantissaDef.GetInt("InitialValue"));
			return Util.ComposedDecimal(name, exponentOperator, exponentDefaultValue, mantissaOperator, mantissaDefaultValue, optional);
		}
		public override GroupValue Convert(Field field, ConversionContext context)
		{
			ComposedScalar composedScalar = (ComposedScalar) field;
			Message message = new Message(SessionControlProtocol_1_1.COMP_DECIMAL_INSTR);
			SetNameAndId(field, message);
			message.SetInteger("Optional", field.Optional?1:0);
			GroupValue exponentDef = CreateComponent(composedScalar.Fields[0], "Exponent");
			GroupValue mantissaDef = CreateComponent(composedScalar.Fields[1], "Mantissa");
			message.SetFieldValue("Exponent", exponentDef);
			message.SetFieldValue("Mantissa", mantissaDef);
			return message;
		}
		private GroupValue CreateComponent(Scalar component, string componentName)
		{
			Group componentGroup = SessionControlProtocol_1_1.COMP_DECIMAL_INSTR.GetGroup(componentName);
			GroupValue componentDef = new GroupValue(componentGroup);
			GroupValue componentOperatorDef = CreateOperator(component);
			GroupValue componentOperatorGroup = new GroupValue(componentGroup.GetGroup("Operator"));
			componentDef.SetFieldValue("Operator", componentOperatorGroup);
			componentOperatorGroup.SetFieldValue(0, componentOperatorDef);
			if (!component.DefaultValue.Undefined)
				componentDef.SetInteger("InitialValue", component.DefaultValue.ToInt());
			return componentDef;
		}
		public override bool ShouldConvert(Field field)
		{
			return field.GetType().Equals(typeof(ComposedScalar));
		}
	}
}