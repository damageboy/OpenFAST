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
using System;
using Dictionary = OpenFAST.Dictionary;
using GroupValue = OpenFAST.GroupValue;
using Message = OpenFAST.Message;
using SessionControlProtocol_1_1 = OpenFAST.Session.SessionControlProtocol_1_1;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using Scalar = OpenFAST.Template.Scalar;
using Operator = OpenFAST.Template.operator_Renamed.Operator;

namespace OpenFAST.Session.Template.Exchange
{
	public abstract class AbstractFieldInstructionConverter : FieldInstructionConverter
	{
		public abstract OpenFAST.Template.Group[] TemplateExchangeTemplates{get;}
		public static void  SetNameAndId(Field field, GroupValue fieldDef)
		{
			SetName(field, fieldDef);
			if (field.Id != null)
				fieldDef.SetString("AuxId", field.Id);
		}
		
		public static void  SetName(Field field, GroupValue fieldDef)
		{
			fieldDef.SetString("Name", field.Name);
			fieldDef.SetString("Ns", field.QName.Namespace);
		}
		
		public static GroupValue CreateOperator(Scalar scalar)
		{
			if (!OPERATOR_TEMPLATE_MAP.Contains(scalar.Operator))
				return null;
			MessageTemplate operatorTemplate = (MessageTemplate) OPERATOR_TEMPLATE_MAP[scalar.Operator];
			GroupValue operatorMessage = new Message(operatorTemplate);
			if (!scalar.Dictionary.Equals(OpenFAST.Dictionary_Fields.GLOBAL))
				operatorMessage.SetString("Dictionary", scalar.Dictionary);
			if (!scalar.Key.Equals(scalar.QName))
			{
				Group key = operatorTemplate.GetGroup("Key");
				GroupValue keyValue = new GroupValue(key);
				keyValue.SetString("Name", scalar.Key.Name);
				keyValue.SetString("Ns", scalar.Key.Namespace);
				operatorMessage.SetFieldValue(key, keyValue);
			}
			return operatorMessage;
		}
		
		public static Operator GetOperator(Group group)
		{
			return (Operator) TEMPLATE_OPERATOR_MAP[group];
		}
		
		private static readonly System.Collections.IDictionary OPERATOR_TEMPLATE_MAP = new System.Collections.Hashtable();
		private static readonly System.Collections.IDictionary TEMPLATE_OPERATOR_MAP = new System.Collections.Hashtable();
		public abstract OpenFAST.GroupValue Convert(OpenFAST.Template.Field param1, OpenFAST.Session.Template.Exchange.ConversionContext param2);
		public abstract OpenFAST.Template.Field Convert(OpenFAST.GroupValue param1, OpenFAST.Template.TemplateRegistry param2, OpenFAST.Session.Template.Exchange.ConversionContext param3);
		public abstract bool ShouldConvert(OpenFAST.Template.Field param1);
		static AbstractFieldInstructionConverter()
		{
			{
				OPERATOR_TEMPLATE_MAP[Operator.CONSTANT] = SessionControlProtocol_1_1.CONSTANT_OP;
				OPERATOR_TEMPLATE_MAP[Operator.DEFAULT] = SessionControlProtocol_1_1.DEFAULT_OP;
				OPERATOR_TEMPLATE_MAP[Operator.COPY] = SessionControlProtocol_1_1.COPY_OP;
				OPERATOR_TEMPLATE_MAP[Operator.INCREMENT] = SessionControlProtocol_1_1.INCREMENT_OP;
				OPERATOR_TEMPLATE_MAP[Operator.DELTA] = SessionControlProtocol_1_1.DELTA_OP;
				OPERATOR_TEMPLATE_MAP[Operator.TAIL] = SessionControlProtocol_1_1.TAIL_OP;
				
				TEMPLATE_OPERATOR_MAP[SessionControlProtocol_1_1.CONSTANT_OP] = Operator.CONSTANT;
				TEMPLATE_OPERATOR_MAP[SessionControlProtocol_1_1.DEFAULT_OP] = Operator.DEFAULT;
				TEMPLATE_OPERATOR_MAP[SessionControlProtocol_1_1.COPY_OP] = Operator.COPY;
				TEMPLATE_OPERATOR_MAP[SessionControlProtocol_1_1.INCREMENT_OP] = Operator.INCREMENT;
				TEMPLATE_OPERATOR_MAP[SessionControlProtocol_1_1.DELTA_OP] = Operator.DELTA;
				TEMPLATE_OPERATOR_MAP[SessionControlProtocol_1_1.TAIL_OP] = Operator.TAIL;
			}
		}
	}
}