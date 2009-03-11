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

namespace OpenFAST.Session.Template.Exchange
{
	public class ConversionContext
	{
        private readonly System.Collections.Generic.Dictionary<Group, FieldInstructionConverter> converterTemplateMap = new System.Collections.Generic.Dictionary<Group, FieldInstructionConverter>();
        private readonly System.Collections.Generic.List<FieldInstructionConverter> converters = new System.Collections.Generic.List<FieldInstructionConverter>();
		
		public virtual void  AddFieldInstructionConverter(FieldInstructionConverter converter)
		{
			Group[] templateExchangeTemplates = converter.TemplateExchangeTemplates;
			for (int i = 0; i < templateExchangeTemplates.Length; i++)
			{
				converterTemplateMap[templateExchangeTemplates[i]] = converter;
			}
			converters.Add(converter);
		}
		
		public virtual FieldInstructionConverter GetConverter(Group group)
		{
			return converterTemplateMap[group];
		}
		
		public virtual FieldInstructionConverter GetConverter(Field field)
		{
			for (int i = converters.Count - 1; i >= 0; i--)
			{
				var converter = converters[i];
				if (converter.ShouldConvert(field))
					return converter;
			}
			throw new System.SystemException("No valid converter found for the field: " + field);
		}
	}
}