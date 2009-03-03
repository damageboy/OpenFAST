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
using Context = OpenFAST.Context;
using FieldValue = OpenFAST.FieldValue;
using Message = OpenFAST.Message;
using MessageHandler = OpenFAST.MessageHandler;
using QName = OpenFAST.QName;
using ScalarValue = OpenFAST.ScalarValue;
using Coder = OpenFAST.Codec.Coder;
using ErrorCode = OpenFAST.Error.ErrorCode;
using AbstractFieldInstructionConverter = OpenFAST.Session.Template.Exchange.AbstractFieldInstructionConverter;
using ComposedDecimalConverter = OpenFAST.Session.Template.Exchange.ComposedDecimalConverter;
using ConversionContext = OpenFAST.Session.Template.Exchange.ConversionContext;
using DynamicTemplateReferenceConverter = OpenFAST.Session.Template.Exchange.DynamicTemplateReferenceConverter;
using GroupConverter = OpenFAST.Session.Template.Exchange.GroupConverter;
using ScalarConverter = OpenFAST.Session.Template.Exchange.ScalarConverter;
using SequenceConverter = OpenFAST.Session.Template.Exchange.SequenceConverter;
using StaticTemplateReferenceConverter = OpenFAST.Session.Template.Exchange.StaticTemplateReferenceConverter;
using VariableLengthInstructionConverter = OpenFAST.Session.Template.Exchange.VariableLengthInstructionConverter;
using BasicTemplateRegistry = OpenFAST.Template.BasicTemplateRegistry;
using DynamicTemplateReference = OpenFAST.Template.DynamicTemplateReference;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using MessageTemplate = OpenFAST.Template.MessageTemplate;
using Scalar = OpenFAST.Template.Scalar;
using Sequence = OpenFAST.Template.Sequence;
using StaticTemplateReference = OpenFAST.Template.StaticTemplateReference;
using TemplateRegistry = OpenFAST.Template.TemplateRegistry;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using Type = OpenFAST.Template.Type.FASTType;
using OpenFAST.Template;

namespace OpenFAST.Session
{
	public class SessionControlProtocol_1_1:AbstractSessionControlProtocol
	{
		private class ProtocolDefinationSessionMessageHandler : SessionMessageHandler
		{
			public ProtocolDefinationSessionMessageHandler(SessionControlProtocol_1_1 enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}
			private void  InitBlock(SessionControlProtocol_1_1 enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private SessionControlProtocol_1_1 enclosingInstance;
			public SessionControlProtocol_1_1 Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			public virtual void  HandleMessage(Session session, Message message)
			{
				MessageTemplate template = Enclosing_Instance.CreateTemplateFromMessage(message, session.MessageInputStream.GetTemplateRegistry());
				session.AddDynamicTemplateDefinition(template);
				if (message.IsDefined("TemplateId"))
					session.RegisterDynamicTemplate(template.QName, message.GetInt("TemplateId"));
			}
		}
		private class ProtocolDeclarationSessionMessageHandler : SessionMessageHandler
		{
			public ProtocolDeclarationSessionMessageHandler(SessionControlProtocol_1_1 enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}
			private void  InitBlock(SessionControlProtocol_1_1 enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private SessionControlProtocol_1_1 enclosingInstance;
			public SessionControlProtocol_1_1 Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			public virtual void  HandleMessage(Session session, Message message)
			{
				session.RegisterDynamicTemplate(Enclosing_Instance.GetQName(message), message.GetInt("TemplateId"));
			}
		}
		[Serializable]
		public class RESETMessage:Message
		{
			internal RESETMessage(OpenFAST.Template.MessageTemplate Param1):base(Param1)
			{
			}
			private const long serialVersionUID = 1L;
			
			public override void  SetFieldValue(int fieldIndex, FieldValue value_Renamed)
			{
				throw new System.SystemException("Cannot set values on a fast reserved message.");
			}
		}
		public class RESETMessageHandler : MessageHandler
		{
			public virtual void  HandleMessage(Message readMessage, Context context, Coder coder)
			{
				if (readMessage.Template.HasAttribute(OpenFAST.Session.SessionControlProtocol_1_1.RESET_PROPERTY))
					coder.Reset();
			}
		}
		public class ALERTSessionMessageHandler : SessionMessageHandler
		{
			public virtual void  HandleMessage(Session session, Message message)
			{
				ErrorCode alertCode = ErrorCode.GetAlertCode(message);
				if (alertCode.Equals(OpenFAST.Session.SessionConstants.CLOSE))
				{
					session.Close(alertCode);
				}
				else
				{
					session.ErrorHandler.Error(alertCode, message.GetString(4));
				}
			}
		}
		private static Group Namespace
		{
			set
			{
				value.ChildNamespace = NAMESPACE;
				Field[] fields = value.Fields;
				for (int i = 0; i < fields.Length; i++)
				{
					if (fields[i] is Group)
					{
						Namespace = (Group) fields[i];
					}
				}
			}
			
		}
		override public Message CloseMessage
		{
			get
			{
				return CLOSE;
			}
			
		}
		public const string NAMESPACE = "http://www.fixprotocol.org/ns/fast/scp/1.1";
		private static readonly QName RESET_PROPERTY = new QName("reset", NAMESPACE);
		private static readonly System.Collections.IDictionary messageHandlers = new System.Collections.Hashtable();
		private ConversionContext initialContext = CreateInitialContext();
		
		public SessionControlProtocol_1_1()
		{
			messageHandlers[FAST_ALERT_TEMPLATE] = ALERT_HANDLER;
			messageHandlers[TEMPLATE_DEFINITION] = new ProtocolDefinationSessionMessageHandler(this);
			messageHandlers[TEMPLATE_DECLARATION] = new ProtocolDeclarationSessionMessageHandler(this);
		}
		public static ConversionContext CreateInitialContext()
		{
			ConversionContext context = new ConversionContext();
			context.AddFieldInstructionConverter(new ScalarConverter());
			context.AddFieldInstructionConverter(new SequenceConverter());
			context.AddFieldInstructionConverter(new GroupConverter());
			context.AddFieldInstructionConverter(new DynamicTemplateReferenceConverter());
			context.AddFieldInstructionConverter(new StaticTemplateReferenceConverter());
			context.AddFieldInstructionConverter(new ComposedDecimalConverter());
			context.AddFieldInstructionConverter(new VariableLengthInstructionConverter());
			return context;
		}
		protected internal virtual QName GetQName(Message message)
		{
			string name = message.GetString("Name");
			string ns = message.GetString("Ns");
			return new QName(name, ns);
		}
		public override void  ConfigureSession(Session session)
		{
			RegisterSessionTemplates(session.MessageInputStream.GetTemplateRegistry());
			RegisterSessionTemplates(session.MessageOutputStream.GetTemplateRegistry());
			session.MessageInputStream.AddMessageHandler(FAST_RESET_TEMPLATE, RESET_HANDLER);
			session.MessageOutputStream.AddMessageHandler(FAST_RESET_TEMPLATE, RESET_HANDLER);
		}
		public virtual void  RegisterSessionTemplates(TemplateRegistry registry)
		{
			registry.RegisterAll(TEMPLATE_REGISTRY);
		}
        public override Session Connect(string senderName, Connection connection, TemplateRegistry inboundRegistry, TemplateRegistry outboundRegistry, MessageListener messageListener, SessionListener sessionListener)
		{
            Session session = new Session(connection, this, TemplateRegistry_Fields.NULL, TemplateRegistry_Fields.NULL);
			session.MessageOutputStream.WriteMessage(CreateHelloMessage(senderName));
			try
			{
				System.Threading.Thread.Sleep(new System.TimeSpan((System.Int64) 10000 * 20));
			}
			catch (System.Threading.ThreadInterruptedException)
			{
			}
			Message message = session.MessageInputStream.ReadMessage();
			string serverName = message.GetString(1);
			string vendorId = message.IsDefined(2)?message.GetString(2):"unknown";
			session.Client = new BasicClient(serverName, vendorId);
			return session;
		}
		public override void  OnError(Session session, ErrorCode code, string message)
		{
			session.MessageOutputStream.WriteMessage(CreateFastAlertMessage(code));
		}
		public override Session OnNewConnection(string serverName, Connection connection)
		{
            Session session = new Session(connection, this, TemplateRegistry_Fields.NULL, TemplateRegistry_Fields.NULL);
			Message message = session.MessageInputStream.ReadMessage();
			string clientName = message.GetString(1);
			string vendorId = message.IsDefined(2)?message.GetString(2):"unknown";
			session.Client = new BasicClient(clientName, vendorId);
			session.MessageOutputStream.WriteMessage(CreateHelloMessage(serverName));
			return session;
		}
		public virtual Message CreateHelloMessage(string senderName)
		{
			Message message = new Message(FAST_HELLO_TEMPLATE);
			message.SetString(1, senderName);
			message.SetString(2, OpenFAST.Session.SessionConstants.VENDOR_ID);
			return message;
		}
		public static Message CreateFastAlertMessage(ErrorCode code)
		{
			Message alert = new Message(FAST_ALERT_TEMPLATE);
			alert.SetInteger(1, code.Severity.Code);
			alert.SetInteger(2, code.Code);
			alert.SetString(4, code.Description);
			return alert;
		}
		public override void  HandleMessage(Session session, Message message)
		{
			if (!messageHandlers.Contains(message.Template))
				return ;
			((SessionMessageHandler) messageHandlers[message.Template]).HandleMessage(session, message);
		}
		public override bool IsProtocolMessage(Message message)
		{
			if (message == null)
				return false;
			return messageHandlers.Contains(message.Template);
		}
		public override bool SupportsTemplateExchange()
		{
			return true;
		}
		public override Message CreateTemplateDeclarationMessage(MessageTemplate messageTemplate, int templateId)
		{
			Message declaration = new Message(TEMPLATE_DECLARATION);
			AbstractFieldInstructionConverter.SetName(messageTemplate, declaration);
			declaration.SetInteger("TemplateId", templateId);
			return declaration;
		}
		public override Message CreateTemplateDefinitionMessage(MessageTemplate messageTemplate)
		{
			Message templateDefinition = GroupConverter.Convert(messageTemplate, new Message(TEMPLATE_DEFINITION), initialContext);
			int reset = messageTemplate.HasAttribute(RESET_PROPERTY)?1:0;
			templateDefinition.SetInteger("Reset", reset);
			return templateDefinition;
		}
		public virtual MessageTemplate CreateTemplateFromMessage(Message templateDef, TemplateRegistry registry)
		{
			string name = templateDef.GetString("Name");
			Field[] fields = GroupConverter.ParseFieldInstructions(templateDef, registry, initialContext);
			return new MessageTemplate(name, fields);
		}
		
		new public const int FAST_RESET_TEMPLATE_ID = 120;
		public const int FAST_HELLO_TEMPLATE_ID = 16002;
		public const int FAST_ALERT_TEMPLATE_ID = 16003;
		public const int TEMPLATE_DECL_ID = 16010;
		public const int TEMPLATE_DEF_ID = 16011;
		public const int INT32_INSTR_ID = 16012;
		public const int UINT32_INSTR_ID = 16013;
		public const int INT64_INSTR_ID = 16014;
		public const int UINT64_INSTR_ID = 16015;
		public const int DECIMAL_INSTR_ID = 16016;
		public const int COMP_DECIMAL_INSTR_ID = 16017;
		public const int ASCII_INSTR_ID = 16018;
		public const int UNICODE_INSTR_ID = 16019;
		public const int BYTE_VECTOR_INSTR_ID = 16020;
		public const int STAT_TEMP_REF_INSTR_ID = 16021;
		public const int DYN_TEMP_REF_INSTR_ID = 16022;
		public const int SEQUENCE_INSTR_ID = 16023;
		public const int GROUP_INSTR_ID = 16024;
		public const int CONSTANT_OP_ID = 16025;
		public const int DEFAULT_OP_ID = 16026;
		public const int COPY_OP_ID = 16027;
		public const int INCREMENT_OP_ID = 16028;
		public const int DELTA_OP_ID = 16029;
		public const int TAIL_OP_ID = 16030;
		public const int FOREIGN_INSTR_ID = 16031;
		public const int ELEMENT_ID = 16032;
		public const int TEXT_ID = 16033;
		public static readonly MessageTemplate FAST_ALERT_TEMPLATE;
		public static readonly MessageTemplate FAST_HELLO_TEMPLATE;
		new public static readonly Message RESET;
		/// <summary> ************************ MESSAGE HANDLERS
		/// *********************************************
		/// </summary>
		private static MessageHandler RESET_HANDLER;
		private static SessionMessageHandler ALERT_HANDLER;

		private static MessageTemplate ATTRIBUTE;
		private static MessageTemplate ELEMENT;
        private static MessageTemplate staticOTHER = null;
		private static MessageTemplate OTHER
        {
            get
            {
                if (staticOTHER == null)
                {
                    staticOTHER = new MessageTemplate(new QName("Other", NAMESPACE), new Field[] { new Group(qualify("Other"), new Field[] { new Sequence(qualify("ForeignAttributes"), new Field[] { new StaticTemplateReference(ATTRIBUTE) }, true), new Sequence(qualify("ForeignElements"), new Field[] { new StaticTemplateReference(ELEMENT) }, true) }, true) });
                }
                return staticOTHER;
            }
        }
		private static readonly MessageTemplate TEMPLATE_NAME;
		private static readonly MessageTemplate NS_NAME;
		private static readonly MessageTemplate NS_NAME_WITH_AUX_ID;
		private static readonly MessageTemplate FIELD_BASE;
		private static readonly MessageTemplate PRIM_FIELD_BASE;
        private static MessageTemplate staticLENGTH_PREAMBLE = null;

		private static MessageTemplate LENGTH_PREAMBLE
        {
            get{
                if (staticLENGTH_PREAMBLE == null)
                {
                    staticLENGTH_PREAMBLE = new MessageTemplate(new QName("LengthPreamble", NAMESPACE), new Field[] { new StaticTemplateReference(NS_NAME_WITH_AUX_ID), new StaticTemplateReference(OTHER) });
                }
                return staticLENGTH_PREAMBLE;
            }
        }
        private static MessageTemplate staticPRIM_FIELD_BASE_WITH_LENGTH = null;
		private static MessageTemplate PRIM_FIELD_BASE_WITH_LENGTH
        {
            get
            {
                if (staticPRIM_FIELD_BASE_WITH_LENGTH == null)
                {
                    staticPRIM_FIELD_BASE_WITH_LENGTH = new MessageTemplate(new QName("PrimFieldBaseWithLength", NAMESPACE), new Field[] { new StaticTemplateReference(PRIM_FIELD_BASE), new Group(qualify("Length"), new Field[] { new StaticTemplateReference(LENGTH_PREAMBLE) }, true) });
                }
                return staticPRIM_FIELD_BASE_WITH_LENGTH;
            }
        }
		public static readonly MessageTemplate INT32_INSTR;
		public static readonly MessageTemplate UINT32_INSTR;
		public static readonly MessageTemplate INT64_INSTR;
		public static readonly MessageTemplate UINT64_INSTR;
		public static readonly MessageTemplate DECIMAL_INSTR;
		public static readonly MessageTemplate UNICODE_INSTR;
		public static readonly MessageTemplate ASCII_INSTR;
		public static readonly MessageTemplate BYTE_VECTOR_INSTR;
        private static MessageTemplate staticTYPE_REF = null;
        public static MessageTemplate TYPE_REF
        {
            get
            {
                if (staticTYPE_REF == null)
                {
                    staticTYPE_REF = new MessageTemplate(new QName("TypeRef", NAMESPACE), new Field[] { new Group(qualify("TypeRef"), new Field[] { new StaticTemplateReference(NS_NAME), new StaticTemplateReference(OTHER) }, true) });
                }
                return staticTYPE_REF;
            }
        }
        private static MessageTemplate staticTEMPLATE_DECLARATION = null;
		public static MessageTemplate TEMPLATE_DECLARATION
        {
            get{
                if (staticTEMPLATE_DECLARATION == null)
                {
                    staticTEMPLATE_DECLARATION = new MessageTemplate(new QName("TemplateDecl", NAMESPACE), new Field[] { new StaticTemplateReference(TEMPLATE_NAME), u32("TemplateId") });
                }
                return staticTEMPLATE_DECLARATION;
            }
        }
		public static readonly MessageTemplate TEMPLATE_DEFINITION;
        private static MessageTemplate staticOP_BASE = null;
        public static MessageTemplate OP_BASE
        {
            get
            {
                if (staticOP_BASE == null)
                {
                    staticOP_BASE = new MessageTemplate(new QName("OpBase", NAMESPACE), new Field[] { unicodeopt("Dictionary"), new Group(qualify("Key"), new Field[] { new StaticTemplateReference(NS_NAME) }, true), new StaticTemplateReference(OTHER) });
                }
                return staticOP_BASE;
            }
        }
        private static MessageTemplate staticCONSTANT_OP = null;
		public static MessageTemplate CONSTANT_OP
        {
            get
            {
                if (staticCONSTANT_OP == null)
                {
                    staticCONSTANT_OP = new MessageTemplate(new QName("ConstantOp", NAMESPACE), new Field[] { new StaticTemplateReference(OP_BASE) });
                }
                return staticCONSTANT_OP;
            }
        }
        private static MessageTemplate staticDEFAULT_OP = null;
        public static MessageTemplate DEFAULT_OP
        {
            get
            {
                if (staticDEFAULT_OP == null)
                {
                    staticDEFAULT_OP = new MessageTemplate(new QName("DefaultOp", NAMESPACE), new Field[] { new StaticTemplateReference(OP_BASE) });
                }
                return staticDEFAULT_OP;
            }
        }
        private static MessageTemplate staticCOPY_OP = null;
        public static MessageTemplate COPY_OP
        {
            get
            {
                if (staticCOPY_OP == null)
                {
                    staticCOPY_OP = new MessageTemplate(new QName("CopyOp", NAMESPACE), new Field[] { new StaticTemplateReference(OP_BASE) });
                }
                return staticCOPY_OP;
            }
        }
        private static MessageTemplate staticINCREMENT_OP = null;

		public static MessageTemplate INCREMENT_OP
        {
            get
            {
                if (staticINCREMENT_OP == null)
                {
                    staticINCREMENT_OP = new MessageTemplate(new QName("IncrementOp", NAMESPACE), new Field[] { new StaticTemplateReference(OP_BASE) });
                }
                return staticINCREMENT_OP;
            }
        }
        public static MessageTemplate staticDELTA_OP;

        public static MessageTemplate DELTA_OP
        {
            get
            {
                if (staticDELTA_OP == null)
                {
                    staticDELTA_OP = new MessageTemplate(new QName("DeltaOp", NAMESPACE), new Field[] { new StaticTemplateReference(OP_BASE) });
                }
                return staticDELTA_OP;
            }
        }
        public static MessageTemplate staticTAIL_OP;
		public static MessageTemplate TAIL_OP
        {
            get
            {
                if (staticTAIL_OP == null)
                {
                    staticTAIL_OP = new MessageTemplate(new QName("TailOp", NAMESPACE), new Field[] { new StaticTemplateReference(OP_BASE) });
                }
                return staticTAIL_OP;
            }
        }

		public static readonly MessageTemplate GROUP_INSTR;
		public static readonly MessageTemplate SEQUENCE_INSTR;

        private static MessageTemplate staticSTAT_TEMP_REF_INSTR;
        public static MessageTemplate STAT_TEMP_REF_INSTR
        {
            get
            {
                if (staticSTAT_TEMP_REF_INSTR == null)
                {
                    staticSTAT_TEMP_REF_INSTR = new MessageTemplate(new QName("StaticTemplateRefInstr", NAMESPACE), new Field[] { new StaticTemplateReference(TEMPLATE_NAME), new StaticTemplateReference(OTHER) });
                }
                return staticSTAT_TEMP_REF_INSTR;
            }
        }
        private static MessageTemplate staticDYN_TEMP_REF_INSTR;
        public static MessageTemplate DYN_TEMP_REF_INSTR
        {
            get
            {
                if (staticDYN_TEMP_REF_INSTR == null)
                {
                    staticDYN_TEMP_REF_INSTR = new MessageTemplate(new QName("DynamicTemplateRefInstr", NAMESPACE), new Field[] { new StaticTemplateReference(OTHER) });
                } 
                return staticDYN_TEMP_REF_INSTR;
            }
        }
        private static MessageTemplate staticFOREIGN_INSTR;
        public static MessageTemplate FOREIGN_INSTR
        {
            get
            {
                if (staticFOREIGN_INSTR == null)
                {
                    staticFOREIGN_INSTR = new MessageTemplate(qualify("ForeignInstr"), new Field[] { new StaticTemplateReference(ELEMENT) });
                }
                return staticFOREIGN_INSTR;
            }
        }

		public static readonly MessageTemplate TEXT;
		public static readonly MessageTemplate COMP_DECIMAL_INSTR;
        private static Message staticDYN_TEMP_REF_MESSAGE;
        public static Message DYN_TEMP_REF_MESSAGE
        {
            get
            {
                if (staticDYN_TEMP_REF_MESSAGE == null)
                {
                    staticDYN_TEMP_REF_MESSAGE = new Message(DYN_TEMP_REF_INSTR);
                }
                return staticDYN_TEMP_REF_MESSAGE;
            }
        }
		
		private static Field u32(string name)
		{
			return new Scalar(qualify(name), Type.U32, Operator.NONE, null, false);
		}

		private static Field dict(string name, Type type, bool optional, string dictionary)
		{
			Scalar scalar = new Scalar(qualify(name), Type.UNICODE, Operator.COPY, null, optional);
			scalar.Dictionary = dictionary;
			return scalar;
		}
		private static QName qualify(string name)
		{
			return new QName(name, NAMESPACE);
		}
		private static Field unicodeopt(string name)
		{
			return new Scalar(qualify(name), Type.UNICODE, Operator.NONE, null, true);
		}
		private static Field unicode(string name)
		{
			return new Scalar(qualify(name), Type.UNICODE, Operator.NONE, null, false);
		}
		private static Field u32opt(string name)
		{
			return new Scalar(qualify(name), Type.U32, Operator.NONE, null, true);
		}
		
		private static readonly TemplateRegistry TEMPLATE_REGISTRY = new BasicTemplateRegistry();

        private static Message staticCLOSE = null;
        private static Message CLOSE
        {
            get
            {
                if (staticCLOSE == null)
                {
                    staticCLOSE = CreateFastAlertMessage(OpenFAST.Session.SessionConstants.CLOSE);
                }
                return staticCLOSE;
            }
        }
		static SessionControlProtocol_1_1()
		{
			FAST_ALERT_TEMPLATE = new MessageTemplate("Alert", new Field[]{new Scalar("Severity", Type.U32, Operator.NONE, ScalarValue.UNDEFINED, false), new Scalar("Code", Type.U32, Operator.NONE, ScalarValue.UNDEFINED, false), new Scalar("Value", Type.U32, Operator.NONE, ScalarValue.UNDEFINED, true), new Scalar("Description", Type.ASCII, Operator.NONE, ScalarValue.UNDEFINED, false)});
			FAST_HELLO_TEMPLATE = new MessageTemplate("Hello", new Field[]{new Scalar("SenderName", Type.ASCII, Operator.NONE, ScalarValue.UNDEFINED, false), new Scalar("VendorId", Type.ASCII, Operator.NONE, ScalarValue.UNDEFINED, true)});
			RESET = new RESETMessage(FAST_RESET_TEMPLATE);
			{
				FAST_RESET_TEMPLATE.AddAttribute(RESET_PROPERTY, "yes");
			}
			RESET_HANDLER = new RESETMessageHandler();
			ALERT_HANDLER = new ALERTSessionMessageHandler();
			ATTRIBUTE = new MessageTemplate(new QName("Attribute", NAMESPACE), new Field[]{dict("Ns", Type.UNICODE, true, "template"), unicode("Name"), unicode("Value")});
			ELEMENT = new MessageTemplate(new QName("Element", NAMESPACE), new Field[]{dict("Ns", Type.UNICODE, true, "template"), unicode("Name"), new Sequence(qualify("Attributes"), new Field[]{new StaticTemplateReference(ATTRIBUTE)}, false), new Sequence(qualify("Content"), new Field[]{DynamicTemplateReference.INSTANCE}, false)});
			TEMPLATE_NAME = new MessageTemplate(new QName("TemplateName", NAMESPACE), new Field[]{new Scalar(qualify("Ns"), Type.UNICODE, Operator.COPY, null, false), new Scalar(qualify("Name"), Type.UNICODE, Operator.NONE, null, false)});
			NS_NAME = new MessageTemplate(new QName("NsName", NAMESPACE), new Field[]{dict("Ns", Type.UNICODE, false, "template"), new Scalar(qualify("Name"), Type.UNICODE, Operator.NONE, null, false)});
			NS_NAME_WITH_AUX_ID = new MessageTemplate(new QName("NsNameWithAuxId", NAMESPACE), new Field[]{new StaticTemplateReference(NS_NAME), new Scalar(qualify("AuxId"), Type.UNICODE, Operator.NONE, null, true)});
			FIELD_BASE = new MessageTemplate(new QName("PrimFieldBase", NAMESPACE), new Field[]{new StaticTemplateReference(NS_NAME_WITH_AUX_ID), new Scalar(qualify("Optional"), Type.U32, Operator.NONE, null, false), new StaticTemplateReference(OTHER)});
			PRIM_FIELD_BASE = new MessageTemplate(new QName("PrimFieldBase", NAMESPACE), new Field[]{new StaticTemplateReference(FIELD_BASE), new Group(qualify("Operator"), new Field[]{DynamicTemplateReference.INSTANCE}, true)});
			INT32_INSTR = new MessageTemplate(new QName("Int32Instr", NAMESPACE), new Field[]{new StaticTemplateReference(PRIM_FIELD_BASE), new Scalar(qualify("InitialValue"), Type.I32, Operator.NONE, null, true)});
			UINT32_INSTR = new MessageTemplate(new QName("UInt32Instr", NAMESPACE), new Field[]{new StaticTemplateReference(PRIM_FIELD_BASE), new Scalar(qualify("InitialValue"), Type.U32, Operator.NONE, null, true)});
			INT64_INSTR = new MessageTemplate(new QName("Int64Instr", NAMESPACE), new Field[]{new StaticTemplateReference(PRIM_FIELD_BASE), new Scalar(qualify("InitialValue"), Type.I64, Operator.NONE, null, true)});
			UINT64_INSTR = new MessageTemplate(new QName("UInt64Instr", NAMESPACE), new Field[]{new StaticTemplateReference(PRIM_FIELD_BASE), new Scalar(qualify("InitialValue"), Type.U64, Operator.NONE, null, true)});
			DECIMAL_INSTR = new MessageTemplate(new QName("DecimalInstr", NAMESPACE), new Field[]{new StaticTemplateReference(PRIM_FIELD_BASE), new Scalar(qualify("InitialValue"), Type.DECIMAL, Operator.NONE, null, true)});
			UNICODE_INSTR = new MessageTemplate(new QName("UnicodeInstr", NAMESPACE), new Field[]{new StaticTemplateReference(PRIM_FIELD_BASE_WITH_LENGTH), new Scalar(qualify("InitialValue"), Type.UNICODE, Operator.NONE, null, true)});
			ASCII_INSTR = new MessageTemplate(new QName("AsciiInstr", NAMESPACE), new Field[]{new StaticTemplateReference(PRIM_FIELD_BASE), new Scalar(qualify("InitialValue"), Type.ASCII, Operator.NONE, null, true)});
			BYTE_VECTOR_INSTR = new MessageTemplate(new QName("ByteVectorInstr", NAMESPACE), new Field[]{new StaticTemplateReference(PRIM_FIELD_BASE_WITH_LENGTH), new Scalar(qualify("InitialValue"), Type.BYTE_VECTOR, Operator.NONE, null, true)});
			TEMPLATE_DEFINITION = new MessageTemplate(new QName("TemplateDef", NAMESPACE), new Field[]{new StaticTemplateReference(TEMPLATE_NAME), unicodeopt("AuxId"), u32opt("TemplateId"), new StaticTemplateReference(TYPE_REF), u32("Reset"), new StaticTemplateReference(OTHER), new Sequence(qualify("Instructions"), new Field[]{DynamicTemplateReference.INSTANCE}, false)});
			GROUP_INSTR = new MessageTemplate(new QName("GroupInstr", NAMESPACE), new Field[]{new StaticTemplateReference(FIELD_BASE), new StaticTemplateReference(TYPE_REF), new Sequence(qualify("Instructions"), new Field[]{DynamicTemplateReference.INSTANCE}, false)});
			SEQUENCE_INSTR = new MessageTemplate(new QName("SequenceInstr", NAMESPACE), new Field[]{new StaticTemplateReference(FIELD_BASE), new StaticTemplateReference(TYPE_REF), new Group(qualify("Length"), new Field[]{new Group(qualify("Name"), new Field[]{new StaticTemplateReference(NS_NAME_WITH_AUX_ID)}, true), new Group(qualify("Operator"), new Field[]{DynamicTemplateReference.INSTANCE}, true), new Scalar(qualify("InitialValue"), Type.U32, Operator.NONE, null, true), new StaticTemplateReference(OTHER)}, true), new Sequence(qualify("Instructions"), new Field[]{DynamicTemplateReference.INSTANCE}, false)});
			TEXT = new MessageTemplate(qualify("Text"), new Field[]{new Scalar(qualify("Value"), Type.UNICODE, Operator.NONE, ScalarValue.UNDEFINED, false)});
			COMP_DECIMAL_INSTR = new MessageTemplate(qualify("CompositeDecimalInstr"), new Field[]{new StaticTemplateReference(FIELD_BASE), new Group(qualify("Exponent"), new Field[]{new Group(qualify("Operator"), new Field[]{DynamicTemplateReference.INSTANCE}, false), new Scalar(qualify("InitialValue"), Type.I32, Operator.NONE, ScalarValue.UNDEFINED, true), new StaticTemplateReference(OTHER)}, true), new Group(qualify("Mantissa"), new Field[]{new Group(qualify("Operator"), new Field[]{DynamicTemplateReference.INSTANCE}, false), new Scalar(qualify("InitialValue"), Type.I32, Operator.NONE, ScalarValue.UNDEFINED, true), new StaticTemplateReference(OTHER)}, true)});
			{
				TEMPLATE_REGISTRY.Register(FAST_HELLO_TEMPLATE_ID, FAST_HELLO_TEMPLATE);
				TEMPLATE_REGISTRY.Register(FAST_ALERT_TEMPLATE_ID, FAST_ALERT_TEMPLATE);
				TEMPLATE_REGISTRY.Register(FAST_RESET_TEMPLATE_ID, FAST_RESET_TEMPLATE);
				TEMPLATE_REGISTRY.Register(TEMPLATE_DECL_ID, TEMPLATE_DECLARATION);
				TEMPLATE_REGISTRY.Register(TEMPLATE_DEF_ID, TEMPLATE_DEFINITION);
				TEMPLATE_REGISTRY.Register(INT32_INSTR_ID, INT32_INSTR);
				TEMPLATE_REGISTRY.Register(UINT32_INSTR_ID, UINT32_INSTR);
				TEMPLATE_REGISTRY.Register(INT64_INSTR_ID, INT64_INSTR);
				TEMPLATE_REGISTRY.Register(UINT64_INSTR_ID, UINT64_INSTR);
				TEMPLATE_REGISTRY.Register(DECIMAL_INSTR_ID, DECIMAL_INSTR);
				TEMPLATE_REGISTRY.Register(COMP_DECIMAL_INSTR_ID, COMP_DECIMAL_INSTR);
				TEMPLATE_REGISTRY.Register(ASCII_INSTR_ID, ASCII_INSTR);
				TEMPLATE_REGISTRY.Register(UNICODE_INSTR_ID, UNICODE_INSTR);
				TEMPLATE_REGISTRY.Register(BYTE_VECTOR_INSTR_ID, BYTE_VECTOR_INSTR);
				TEMPLATE_REGISTRY.Register(STAT_TEMP_REF_INSTR_ID, STAT_TEMP_REF_INSTR);
				TEMPLATE_REGISTRY.Register(DYN_TEMP_REF_INSTR_ID, DYN_TEMP_REF_INSTR);
				TEMPLATE_REGISTRY.Register(SEQUENCE_INSTR_ID, SEQUENCE_INSTR);
				TEMPLATE_REGISTRY.Register(GROUP_INSTR_ID, GROUP_INSTR);
				TEMPLATE_REGISTRY.Register(CONSTANT_OP_ID, CONSTANT_OP);
				TEMPLATE_REGISTRY.Register(DEFAULT_OP_ID, DEFAULT_OP);
				TEMPLATE_REGISTRY.Register(COPY_OP_ID, COPY_OP);
				TEMPLATE_REGISTRY.Register(INCREMENT_OP_ID, INCREMENT_OP);
				TEMPLATE_REGISTRY.Register(DELTA_OP_ID, DELTA_OP);
				TEMPLATE_REGISTRY.Register(TAIL_OP_ID, TAIL_OP);
				TEMPLATE_REGISTRY.Register(FOREIGN_INSTR_ID, FOREIGN_INSTR);
				TEMPLATE_REGISTRY.Register(ELEMENT_ID, ELEMENT);
				TEMPLATE_REGISTRY.Register(TEXT_ID, TEXT);
				MessageTemplate[] templates = TEMPLATE_REGISTRY.Templates;
				for (int i = 0; i < templates.Length; i++)
				{
					Namespace = templates[i];
				}
			}
		}
	}
}