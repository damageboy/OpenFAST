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
using ErrorCode = OpenFAST.Error.ErrorCode;
using ErrorHandler = OpenFAST.Error.ErrorHandler;
using FastAlertSeverity = OpenFAST.Error.FastAlertSeverity;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.Loader
{
	public sealed class XMLMessageTemplateLoader : MessageTemplateLoader
	{
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
				loadTemplateIdFromAuxId = value;
			}
			
		}
		internal const string TEMPLATE_DEFINITION_NS = "http://www.fixprotocol.org/ns/fast/td/1.1";
		
		internal static readonly ErrorCode IO_ERROR;
		
		internal static readonly ErrorCode XML_PARSING_ERROR;
		
		internal static readonly ErrorCode INVALID_TYPE;
		
	    private readonly ParsingContext initialContext;
		
		private bool loadTemplateIdFromAuxId;
		
		public XMLMessageTemplateLoader()
		{
            initialContext = CreateInitialContext();
		}
		
		public static ParsingContext CreateInitialContext()
		{
			var initialContext = new ParsingContext
			                         {
			                             ErrorHandler = Error.ErrorHandler_Fields.DEFAULT,
			                             TemplateRegistry = new BasicTemplateRegistry(),
			                             TypeMap = FASTType.RegisteredTypeMap,
			                             FieldParsers = new System.Collections.ArrayList()
			                         };
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
			
			var root = document.DocumentElement;
			
			var templateParser = new TemplateParser(loadTemplateIdFromAuxId);
            if (root != null)
            {
                if (root.Name.Equals("template"))
                {
                    return new[] {(MessageTemplate) templateParser.Parse(root, initialContext)};
                }
                if (root.Name.Equals("templates"))
                {
                    var context = new ParsingContext(root, initialContext);

                    System.Xml.XmlNodeList templateTags = root.GetElementsByTagName("template");
                    var templates = new MessageTemplate[templateTags.Count];
                    for (int i = 0; i < templateTags.Count; i++)
                    {
                        var templateTag = (System.Xml.XmlElement) templateTags.Item(i);
                        templates[i] = (MessageTemplate) templateParser.Parse(templateTag, context);
                    }
                    return templates;
                }
                initialContext.ErrorHandler.Error(Error.FastConstants.S1_INVALID_XML,
                                                  "Invalid root node " + root.Name +
                                                  ", \"template\" or \"templates\" expected.");
                
            }
            return new MessageTemplate[] { };
		}
		
		private System.Xml.XmlDocument ParseXml(System.IO.Stream templateStream)
		{
			try
			{
                var builder = new System.Xml.XmlDocument();
				var inputSource = new XmlSourceSupport(templateStream);
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
			IO_ERROR = new ErrorCode(Error.FastConstants.STATIC, - 1, "IOERROR", "IO Error", FastAlertSeverity.ERROR);
			XML_PARSING_ERROR = new ErrorCode(Error.FastConstants.STATIC, - 1, "XMLPARSEERR", "XML Parsing Error", FastAlertSeverity.ERROR);
			INVALID_TYPE = new ErrorCode(Error.FastConstants.STATIC, - 1, "INVALIDTYPE", "Invalid Type", FastAlertSeverity.ERROR);
		}
	}
}