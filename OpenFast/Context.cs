using System;
using BasicDecodeTrace = OpenFAST.Debug.BasicDecodeTrace;
using BasicEncodeTrace = OpenFAST.Debug.BasicEncodeTrace;
using Trace = OpenFAST.Debug.Trace;
using ErrorHandler = OpenFAST.Error.ErrorHandler;
using BasicTemplateRegistry = OpenFAST.Template.BasicTemplateRegistry;
using Group = OpenFAST.Template.Group;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using TemplateRegisteredListener = OpenFAST.Template.TemplateRegisteredListener;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;

namespace OpenFAST
{
	public sealed class Context
	{
		public int LastTemplateId
		{
			get
			{
				return lastTemplateId;
			}
			
			set
			{
				lastTemplateId = value;
			}
			
		}
		public ErrorHandler ErrorHandler
		{
			set
			{
				this.errorHandler = value;
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
				this.templateRegistry = value;
			}
			
		}
		public bool TraceEnabled
		{
			get
			{
				return traceEnabled;
			}
			
			set
			{
				this.traceEnabled = value;
			}
			
		}
		public Trace DecodeTrace
		{
			get
			{
				return decodeTrace;
			}
			
			set
			{
				this.decodeTrace = value;
			}
			
		}
		private TemplateRegistry templateRegistry = new BasicTemplateRegistry();
		private int lastTemplateId;
        private System.Collections.Generic.Dictionary<string, Dictionary> dictionaries = new System.Collections.Generic.Dictionary<string, Dictionary>();
		private ErrorHandler errorHandler = OpenFAST.Error.ErrorHandler_Fields.DEFAULT;
		private QName currentApplicationType;
		private bool traceEnabled;
		private Trace encodeTrace;
		private Trace decodeTrace;
		
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
				errorHandler.Error(OpenFAST.Error.FastConstants.D9_TEMPLATE_NOT_REGISTERED, "The template " + template + " has not been registered.");
				return 0;
			}
			return templateRegistry.GetId(template);
		}
		public MessageTemplate GetTemplate(int templateId)
		{
			if (!templateRegistry.IsRegistered(templateId))
			{
				errorHandler.Error(OpenFAST.Error.FastConstants.D9_TEMPLATE_NOT_REGISTERED, "The template with id " + templateId + " has not been registered.");
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
			return (Dictionary) dictionaries[dictionary];
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
			currentApplicationType = (template.HasTypeReference())?template.TypeReference:OpenFAST.Error.FastConstants.ANY_TYPE;
		}
		public void  StartTrace()
		{
			SetEncodeTrace(new BasicEncodeTrace());
			DecodeTrace = new BasicDecodeTrace();
		}
		public void  SetEncodeTrace(BasicEncodeTrace encodeTrace)
		{
			this.encodeTrace = encodeTrace;
		}
		public Trace GetEncodeTrace()
		{
			return encodeTrace;
		}
	}
}