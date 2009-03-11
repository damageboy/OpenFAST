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
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using Scalar = OpenFAST.Template.Scalar;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;
using Operator = openfast.Template.Operator.Operator;
using Type = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Session.Template.Exchange
{
	public class ScalarConverter:AbstractFieldInstructionConverter
	{
		override public Group[] TemplateExchangeTemplates
		{
			get
			{
				return SupportClass.ICollectionSupport.ToArray<Group>(new SupportClass.HashSetSupport(TEMPLATE_TYPE_MAP.Keys));
			}
			
		}
		
		private readonly System.Collections.IDictionary TYPE_TEMPLATE_MAP = new System.Collections.Hashtable();
		private readonly System.Collections.IDictionary TEMPLATE_TYPE_MAP = new System.Collections.Hashtable();
		
		public ScalarConverter()
		{
			TYPE_TEMPLATE_MAP[Type.I32] = SessionControlProtocol_1_1.INT32_INSTR;
			TYPE_TEMPLATE_MAP[Type.U32] = SessionControlProtocol_1_1.UINT32_INSTR;
			TYPE_TEMPLATE_MAP[Type.I64] = SessionControlProtocol_1_1.INT64_INSTR;
			TYPE_TEMPLATE_MAP[Type.U64] = SessionControlProtocol_1_1.UINT64_INSTR;
			TYPE_TEMPLATE_MAP[Type.DECIMAL] = SessionControlProtocol_1_1.DECIMAL_INSTR;
			TYPE_TEMPLATE_MAP[Type.UNICODE] = SessionControlProtocol_1_1.UNICODE_INSTR;
			TYPE_TEMPLATE_MAP[Type.ASCII] = SessionControlProtocol_1_1.ASCII_INSTR;
			TYPE_TEMPLATE_MAP[Type.STRING] = SessionControlProtocol_1_1.ASCII_INSTR;
			TYPE_TEMPLATE_MAP[Type.BYTE_VECTOR] = SessionControlProtocol_1_1.BYTE_VECTOR_INSTR;
			
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.INT32_INSTR] = Type.I32;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.UINT32_INSTR] = Type.U32;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.INT64_INSTR] = Type.I64;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.UINT64_INSTR] = Type.U64;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.DECIMAL_INSTR] = Type.DECIMAL;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.UNICODE_INSTR] = Type.UNICODE;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.ASCII_INSTR] = Type.ASCII;
			TEMPLATE_TYPE_MAP[SessionControlProtocol_1_1.BYTE_VECTOR_INSTR] = Type.BYTE_VECTOR;
		}
		
		public override Field Convert(GroupValue fieldDef, TemplateRegistry templateRegistry, ConversionContext context)
		{
			var type = (Type) TEMPLATE_TYPE_MAP[fieldDef.GetGroup()];
			bool optional = fieldDef.GetBool("Optional");
			var initialValue = ScalarValue.UNDEFINED;
			if (fieldDef.IsDefined("InitialValue"))
				initialValue = (ScalarValue) fieldDef.GetValue("InitialValue");
			
			if (fieldDef.IsDefined("Operator"))
			{
				GroupValue operatorGroup = fieldDef.GetGroup("Operator").GetGroup(0);
				Operator operator_Renamed = GetOperator(operatorGroup.GetGroup());
				var scalar = new Scalar(fieldDef.GetString("Name"), type, operator_Renamed, initialValue, optional);
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
			var scalarTemplate = (MessageTemplate) TYPE_TEMPLATE_MAP[scalar.Type];
			var scalarMsg = new Message(scalarTemplate);
			SetNameAndId(scalar, scalarMsg);
			scalarMsg.SetInteger("Optional", scalar.Optional?1:0);
			if (!scalar.Operator.Equals(Operator.NONE))
				scalarMsg.SetFieldValue("Operator", new GroupValue(scalarTemplate.GetGroup("Operator"), new FieldValue[]{CreateOperator(scalar)}));
			if (!scalar.DefaultValue.Undefined)
				scalarMsg.SetFieldValue("InitialValue", scalar.DefaultValue);
			return scalarMsg;
		}
		
		public override bool ShouldConvert(Field field)
		{
			return field.GetType().Equals(typeof(Scalar));
		}
	}
}