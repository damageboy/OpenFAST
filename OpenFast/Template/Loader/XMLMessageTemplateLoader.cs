using System;
using ErrorCode = OpenFAST.Error.ErrorCode;
using ErrorHandler = OpenFAST.Error.ErrorHandler;
using FastAlertSeverity = OpenFAST.Error.FastAlertSeverity;
using BasicTemplateRegistry = OpenFAST.Template.BasicTemplateRegistry;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.Loader
{
	public sealed class XMLMessageTemplateLoader : MessageTemplateLoader
	{
        //private class AnonymousClassErrorHandler : XmlSaxErrorHandler
        //{
        //    public AnonymousClassErrorHandler(XMLMessageTemplateLoader enclosingInstance)
        //    {
        //        InitBlock(enclosingInstance);
        //    }
        //    private void  InitBlock(XMLMessageTemplateLoader enclosingInstance)
        //    {
        //        this.enclosingInstance = enclosingInstance;
        //    }
        //    private XMLMessageTemplateLoader enclosingInstance;
        //    public XMLMessageTemplateLoader Enclosing_Instance
        //    {
        //        get
        //        {
        //            return enclosingInstance;
        //        }
				
        //    }
        //    public virtual void  error(System.Xml.XmlException exception)
        //    {
        //        Enclosing_Instance.initialContext.ErrorHandler.Error(OpenFAST.Template.Loader.XMLMessageTemplateLoader.XML_PARSING_ERROR, "ERROR: " + exception.Message, exception);
        //    }
        //    public virtual void  fatalError(System.Xml.XmlException exception)
        //    {
        //        Enclosing_Instance.initialContext.ErrorHandler.Error(OpenFAST.Template.Loader.XMLMessageTemplateLoader.XML_PARSING_ERROR, "FATAL: " + exception.Message, exception);
        //    }
        //    public virtual void  warning(System.Xml.XmlException exception)
        //    {
        //        Enclosing_Instance.initialContext.ErrorHandler.Error(OpenFAST.Template.Loader.XMLMessageTemplateLoader.XML_PARSING_ERROR, "WARNING: " + exception.Message, exception);
        //    }
        //}

		public ErrorHandler ErrorHandler
		{
			set
			{
				initialContext.ErrorHandler = value;
			}
			
		}

		public TemplateRegistry TemplateRegistry
		{
			get
			{
				return initialContext.TemplateRegistry;
			}
			
			set
			{
				initialContext.TemplateRegistry = value;
			}
			
		}
		public System.Collections.IDictionary TypeMap
		{
			set
			{
				initialContext.TypeMap = value;
			}
			
		}
		public bool LoadTemplateIdFromAuxId
		{
			set
			{
				this.loadTemplateIdFromAuxId = value;
			}
			
		}
		internal const string TEMPLATE_DEFINITION_NS = "http://www.fixprotocol.org/ns/fast/td/1.1";
		
		internal static readonly ErrorCode IO_ERROR;
		
		internal static readonly ErrorCode XML_PARSING_ERROR;
		
		internal static readonly ErrorCode INVALID_TYPE;
		
		// IMMUTABLE
		private bool namespaceAwareness;
		
		private ParsingContext initialContext;
		
		private bool loadTemplateIdFromAuxId;
		
		public XMLMessageTemplateLoader():this(false)
		{
		}
		
		public XMLMessageTemplateLoader(bool namespaceAwareness)
		{
			this.namespaceAwareness = namespaceAwareness;
			this.initialContext = CreateInitialContext();
		}
		
		public static ParsingContext CreateInitialContext()
		{
			ParsingContext initialContext = new ParsingContext();
			initialContext.ErrorHandler = OpenFAST.Error.ErrorHandler_Fields.DEFAULT;
			initialContext.TemplateRegistry = new BasicTemplateRegistry();
			initialContext.TypeMap = FASTType.RegisteredTypeMap;
			initialContext.FieldParsers = new System.Collections.ArrayList();
			initialContext.AddFieldParser(new ScalarParser());
			initialContext.AddFieldParser(new GroupParser());
			initialContext.AddFieldParser(new SequenceParser());
			initialContext.AddFieldParser(new ComposedDecimalParser());
			initialContext.AddFieldParser(new StringParser());
			initialContext.AddFieldParser(new ByteVectorParser());
			initialContext.AddFieldParser(new TemplateRefParser());
			return initialContext;
		}
		
		public void  AddFieldParser(FieldParser fieldParser)
		{
			initialContext.FieldParsers.Add(fieldParser);
		}
		
		public MessageTemplate[] Load(System.IO.Stream source)
		{
			System.Xml.XmlDocument document = ParseXml(source);
			
			if (document == null)
			{
				return new MessageTemplate[]{};
			}
			
			System.Xml.XmlElement root = (System.Xml.XmlElement) document.DocumentElement;
			
			TemplateParser templateParser = new TemplateParser(loadTemplateIdFromAuxId);
			
			if (root.Name.Equals("template"))
			{
				return new MessageTemplate[]{(MessageTemplate) templateParser.Parse(root, initialContext)};
			}
			else if (root.Name.Equals("templates"))
			{
				ParsingContext context = new ParsingContext(root, initialContext);
				
				System.Xml.XmlNodeList templateTags = root.GetElementsByTagName("template");
				MessageTemplate[] templates = new MessageTemplate[templateTags.Count];
				for (int i = 0; i < templateTags.Count; i++)
				{
					System.Xml.XmlElement templateTag = (System.Xml.XmlElement) templateTags.Item(i);
					templates[i] = (MessageTemplate) templateParser.Parse(templateTag, context);
				}
				return templates;
			}
			else
			{
				initialContext.ErrorHandler.Error(OpenFAST.Error.FastConstants.S1_INVALID_XML, "Invalid root node " + root.Name + ", \"template\" or \"templates\" expected.");
				return new MessageTemplate[]{};
			}
		}
		
		private System.Xml.XmlDocument ParseXml(System.IO.Stream templateStream)
		{
			try
			{
                System.Xml.XmlDocument builder = new System.Xml.XmlDocument();
				XmlSourceSupport inputSource = new XmlSourceSupport(templateStream);
				System.Xml.XmlDocument document = SupportClass.ParseDocument(builder, inputSource);
				return document;
			}
			catch (System.IO.IOException e)
			{
				initialContext.ErrorHandler.Error(IO_ERROR, "Error occurred while trying to read xml template: " + e.Message, e);
			}
			catch (System.Exception e)
			{
				initialContext.ErrorHandler.Error(XML_PARSING_ERROR, "Error occurred while parsing xml template: " + e.Message, e);
			}
			
			return null;
		}
		static XMLMessageTemplateLoader()
		{
			IO_ERROR = new ErrorCode(OpenFAST.Error.FastConstants.STATIC, - 1, "IOERROR", "IO Error", FastAlertSeverity.ERROR);
			XML_PARSING_ERROR = new ErrorCode(OpenFAST.Error.FastConstants.STATIC, - 1, "XMLPARSEERR", "XML Parsing Error", FastAlertSeverity.ERROR);
			INVALID_TYPE = new ErrorCode(OpenFAST.Error.FastConstants.STATIC, - 1, "INVALIDTYPE", "Invalid Type", FastAlertSeverity.ERROR);
		}
	}
}