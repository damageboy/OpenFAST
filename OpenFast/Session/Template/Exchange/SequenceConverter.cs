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
using Scalar = OpenFAST.Template.Scalar;
using Sequence = OpenFAST.Template.Sequence;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;
using Operator = OpenFAST.Template.Operator.Operator;
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
			var qname = new QName(name, ns);
			var fields = GroupConverter.ParseFieldInstructions(fieldDef, templateRegistry, context);
			bool optional = fieldDef.GetBool("Optional");
			Scalar length = null;
			if (fieldDef.IsDefined("Length"))
			{
				var lengthDef = fieldDef.GetGroup("Length");
				QName lengthName;
				string id = null;
				if (lengthDef.IsDefined("Name"))
				{
					var nameDef = lengthDef.GetGroup("Name");
					lengthName = new QName(nameDef.GetString("Name"), nameDef.GetString("Ns"));
					if (nameDef.IsDefined("AuxId"))
						id = nameDef.GetString("AuxId");
				}
				else
					lengthName = Global.CreateImplicitName(qname);
				var operator_Renamed = Operator.NONE;
				if (lengthDef.IsDefined("Operator"))
					operator_Renamed = GetOperator(lengthDef.GetGroup("Operator").GetGroup(0).GetGroup());
				var initialValue = ScalarValue.UNDEFINED;
				if (lengthDef.IsDefined("InitialValue"))
					initialValue = (ScalarValue) lengthDef.GetValue("InitialValue");
				length = new Scalar(lengthName, Type.U32, operator_Renamed, initialValue, optional) {Id = id};
			}
			
			return new Sequence(qname, length, fields, optional);
		}
		
		public override GroupValue Convert(Field field, ConversionContext context)
		{
			var sequence = (Sequence) field;
			var seqDef = GroupConverter.Convert(sequence.Group, new Message(SessionControlProtocol_1_1.SEQUENCE_INSTR), context);
			seqDef.SetBool("Optional", sequence.Optional);
			if (!sequence.ImplicitLength)
			{
				var lengthGroup = SessionControlProtocol_1_1.SEQUENCE_INSTR.GetGroup("Length");
				var lengthDef = new GroupValue(lengthGroup);
				var length = sequence.Length;
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
			return field.GetType().Equals(typeof(Sequence));
		}
	}
}