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
using QName = OpenFAST.QName;
using DynamicTemplateReference = OpenFAST.Template.DynamicTemplateReference;
using Field = OpenFAST.Template.Field;
using StaticTemplateReference = OpenFAST.Template.StaticTemplateReference;

namespace OpenFAST.Template.Loader
{
	public class TemplateRefParser : FieldParser
	{
		public virtual Field Parse(System.Xml.XmlElement element, ParsingContext context)
		{
			if (element.HasAttribute("name"))
			{
				QName templateName;
				if (element.HasAttribute("templateNs"))
					templateName = new QName(element.GetAttribute("name"), element.GetAttribute("templateNs"));
				else
					templateName = new QName(element.GetAttribute("name"), "");
				
				if (context.TemplateRegistry.IsDefined(templateName))
					return new StaticTemplateReference(context.TemplateRegistry.get_Renamed(templateName));
				else
				{
					context.ErrorHandler.Error(OpenFAST.Error.FastConstants.D8_TEMPLATE_NOT_EXIST, "The template \"" + templateName + "\" was not found.");
					return null;
				}
			}
			else
			{
				return DynamicTemplateReference.INSTANCE;
			}
		}
		
		public virtual bool CanParse(System.Xml.XmlElement element, ParsingContext context)
		{
			return "templateRef".Equals(element.Name);
		}
	}
}