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
using GroupValue = OpenFAST.GroupValue;
using Message = OpenFAST.Message;
using QName = OpenFAST.QName;
using SessionControlProtocol_1_1 = OpenFAST.Session.SessionControlProtocol_1_1;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using StaticTemplateReference = OpenFAST.Template.StaticTemplateReference;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;

namespace OpenFAST.Session.Template.Exchange
{
	public class StaticTemplateReferenceConverter:AbstractFieldInstructionConverter
	{
		override public Group[] TemplateExchangeTemplates
		{
			get
			{
				return new Group[]{SessionControlProtocol_1_1.STAT_TEMP_REF_INSTR};
			}
			
		}
		
		public override Field Convert(GroupValue fieldDef, TemplateRegistry templateRegistry, ConversionContext context)
		{
			QName name = new QName(fieldDef.GetString("Name"), fieldDef.GetString("Ns"));
			if (!templateRegistry.IsDefined(name))
			{
				throw new System.SystemException("Referenced template " + name + " not defined.");
			}
			return new StaticTemplateReference(templateRegistry.get_Renamed(name));
		}
		
		public override GroupValue Convert(Field field, ConversionContext context)
		{
			Message strDef = new Message(SessionControlProtocol_1_1.STAT_TEMP_REF_INSTR);
			SetNameAndId(field, strDef);
			return strDef;
		}
		
		public override bool ShouldConvert(Field field)
		{
			return field.GetType().Equals(typeof(StaticTemplateReference));
		}
	}
}