using System;

namespace OpenFAST.Template
{
	public interface TemplateRegisteredListener
	{
		void  TemplateRegistered(MessageTemplate template, int templateId);
	}
}