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
using BasicDecodeTrace = OpenFAST.Debug.BasicDecodeTrace;
using BasicEncodeTrace = OpenFAST.Debug.BasicEncodeTrace;
using Trace = OpenFAST.Debug.Trace;
using ErrorHandler = OpenFAST.Error.ErrorHandler;
using BasicTemplateRegistry = OpenFAST.Template.BasicTemplateRegistry;
using Group = OpenFAST.Template.Group;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;

namespace OpenFAST
{
	public sealed class Context
	{
	    public int LastTemplateId { get; set; }

	    public ErrorHandler ErrorHandler
		{
			set
			{
				errorHandler = value;
			}
			
		}
		public QName CurrentApplicationType
		{
			set
			{
				currentApplicationType = value;
			}
			
		}
		public TemplateRegistry TemplateRegistry
		{
			get
			{
				return templateRegistry;
			}
			
			set
			{
				templateRegistry = value;
			}
			
		}

	    public bool TraceEnabled { get; set; }

	    public Trace DecodeTrace { get; set; }

	    private TemplateRegistry templateRegistry = new BasicTemplateRegistry();
	    private readonly System.Collections.Generic.Dictionary<string, Dictionary> dictionaries = new System.Collections.Generic.Dictionary<string, Dictionary>();
		private ErrorHandler errorHandler = Error.ErrorHandler_Fields.DEFAULT;
		private QName currentApplicationType;
	    private Trace encodeTraceInternal;

	    public Context()
		{
			dictionaries["global"] = new GlobalDictionary();
			dictionaries["template"] = new TemplateDictionary();
			dictionaries["type"] = new ApplicationTypeDictionary();
		}
		public int GetTemplateId(MessageTemplate template)
		{
			if (!templateRegistry.IsRegistered(template))
			{
				errorHandler.Error(Error.FastConstants.D9_TEMPLATE_NOT_REGISTERED, "The template " + template + " has not been registered.");
				return 0;
			}
			return templateRegistry.GetId(template);
		}
		public MessageTemplate GetTemplate(int templateId)
		{
			if (!templateRegistry.IsRegistered(templateId))
			{
				errorHandler.Error(Error.FastConstants.D9_TEMPLATE_NOT_REGISTERED, "The template with id " + templateId + " has not been registered.");
				return null;
			}
			return templateRegistry.get_Renamed(templateId);
		}
		public void  RegisterTemplate(int templateId, MessageTemplate template)
		{
			templateRegistry.Register(templateId, template);
		}
		public ScalarValue Lookup(string dictionary, Group group, QName key)
		{
			if (group.HasTypeReference())
				currentApplicationType = group.TypeReference;
			return GetDictionary(dictionary).Lookup(group, key, currentApplicationType);
		}
		private Dictionary GetDictionary(string dictionary)
		{
			if (!dictionaries.ContainsKey(dictionary))
				dictionaries[dictionary] = new GlobalDictionary();
			return dictionaries[dictionary];
		}
		public void  Store(string dictionary, Group group, QName key, ScalarValue valueToEncode)
		{
			if (group.HasTypeReference())
				currentApplicationType = group.TypeReference;
			GetDictionary(dictionary).Store(group, currentApplicationType, key, valueToEncode);
		}
		public void  Reset()
		{
            foreach (Dictionary dict in dictionaries.Values)
            {
                dict.Reset();
            }
		}
		public void  NewMessage(MessageTemplate template)
		{
			currentApplicationType = (template.HasTypeReference())?template.TypeReference:Error.FastConstants.ANY_TYPE;
		}
		public void  StartTrace()
		{
			SetEncodeTrace(new BasicEncodeTrace());
			DecodeTrace = new BasicDecodeTrace();
		}
		public void  SetEncodeTrace(BasicEncodeTrace encodeTrace)
		{
			encodeTraceInternal = encodeTrace;
		}
		public Trace GetEncodeTrace()
		{
			return encodeTraceInternal;
		}
	}
}