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
using SessionControlProtocol_1_1 = OpenFAST.Session.SessionControlProtocol_1_1;
using DynamicTemplateReference = OpenFAST.Template.DynamicTemplateReference;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;

namespace OpenFAST.Session.Template.Exchange
{
	public class DynamicTemplateReferenceConverter : FieldInstructionConverter
	{
		virtual public Group[] TemplateExchangeTemplates
		{
			get
			{
				return new Group[]{SessionControlProtocol_1_1.DYN_TEMP_REF_INSTR};
			}
			
		}
		
		public virtual Field Convert(GroupValue groupValue, TemplateRegistry templateRegistry, ConversionContext context)
		{
			return DynamicTemplateReference.INSTANCE;
		}
		
		public virtual GroupValue Convert(Field field, ConversionContext context)
		{
			return SessionControlProtocol_1_1.DYN_TEMP_REF_MESSAGE;
		}
		
		public virtual bool ShouldConvert(Field field)
		{
			return field.GetType().Equals(typeof(DynamicTemplateReference));
		}
	}
}