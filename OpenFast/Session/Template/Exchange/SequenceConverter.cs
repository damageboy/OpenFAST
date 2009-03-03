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
using Global = OpenFAST.Global;
using GroupValue = OpenFAST.GroupValue;
using Message = OpenFAST.Message;
using QName = OpenFAST.QName;
using ScalarValue = OpenFAST.ScalarValue;
using SessionControlProtocol_1_1 = OpenFAST.Session.SessionControlProtocol_1_1;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using Scalar = OpenFAST.Template.Scalar;
using Sequence = OpenFAST.Template.Sequence;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using Type = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Session.Template.Exchange
{
	public class SequenceConverter:AbstractFieldInstructionConverter
	{
		override public Group[] TemplateExchangeTemplates
		{
			get
			{
				return new Group[]{SessionControlProtocol_1_1.SEQUENCE_INSTR};
			}
			
		}
		
		public override Field Convert(GroupValue fieldDef, TemplateRegistry templateRegistry, ConversionContext context)
		{
			string name = fieldDef.GetString("Name");
			string ns = fieldDef.GetString("Ns");
			QName qname = new QName(name, ns);
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
				Operator operator_Renamed = Operator.NONE;
				if (lengthDef.IsDefined("Operator"))
					operator_Renamed = GetOperator(lengthDef.GetGroup("Operator").GetGroup(0).GetGroup());
				ScalarValue initialValue = ScalarValue.UNDEFINED;
				if (lengthDef.IsDefined("InitialValue"))
					initialValue = (ScalarValue) lengthDef.GetValue("InitialValue");
				length = new Scalar(lengthName, Type.U32, operator_Renamed, initialValue, optional);
				length.Id = id;
			}
			
			return new Sequence(qname, length, fields, optional);
		}
		
		public override GroupValue Convert(Field field, ConversionContext context)
		{
			Sequence sequence = (Sequence) field;
			Message seqDef = GroupConverter.Convert(sequence.Group, new Message(SessionControlProtocol_1_1.SEQUENCE_INSTR), context);
			seqDef.SetBool("Optional", sequence.Optional);
			if (!sequence.ImplicitLength)
			{
				Group lengthGroup = SessionControlProtocol_1_1.SEQUENCE_INSTR.GetGroup("Length");
				GroupValue lengthDef = new GroupValue(lengthGroup);
				Scalar length = sequence.Length;
				GroupValue nameDef = new GroupValue(lengthGroup.GetGroup("Name"));
				SetNameAndId(length, nameDef);
				lengthDef.SetFieldValue("Name", nameDef);
				seqDef.SetFieldValue("Length", lengthDef);
				
				if (!length.Operator.Equals(Operator.NONE))
				{
					GroupValue operatorDef = new GroupValue(lengthGroup.GetGroup("Operator"));
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
			return field.GetType().Equals(typeof(Sequence));
		}
	}
}