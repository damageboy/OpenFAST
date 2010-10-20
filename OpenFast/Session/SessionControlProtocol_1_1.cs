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
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using System;
using System.Collections.Generic;
using System.Threading;
using OpenFAST.Codec;
using OpenFAST.Error;
using OpenFAST.Session.Template.Exchange;
using OpenFAST.Template;
using OpenFAST.Template.Operator;
using Type = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Session
{
    public class SessionControlProtocol_1_1 : AbstractSessionControlProtocol
    {
        public const string NAMESPACE = "http://www.fixprotocol.org/ns/fast/scp/1.1";

        private new const int FAST_RESET_TEMPLATE_ID = 120;
        private const int FAST_HELLO_TEMPLATE_ID = 16002;
        private const int FAST_ALERT_TEMPLATE_ID = 16003;
        private const int TEMPLATE_DECL_ID = 16010;
        private const int TEMPLATE_DEF_ID = 16011;
        private const int INT32_INSTR_ID = 16012;
        private const int UINT32_INSTR_ID = 16013;
        private const int INT64_INSTR_ID = 16014;
        private const int UINT64_INSTR_ID = 16015;
        private const int DECIMAL_INSTR_ID = 16016;
        private const int COMP_DECIMAL_INSTR_ID = 16017;
        private const int ASCII_INSTR_ID = 16018;
        private const int UNICODE_INSTR_ID = 16019;
        private const int BYTE_VECTOR_INSTR_ID = 16020;
        private const int STAT_TEMP_REF_INSTR_ID = 16021;
        private const int DYN_TEMP_REF_INSTR_ID = 16022;
        private const int SEQUENCE_INSTR_ID = 16023;
        private const int GROUP_INSTR_ID = 16024;
        private const int CONSTANT_OP_ID = 16025;
        private const int DEFAULT_OP_ID = 16026;
        private const int COPY_OP_ID = 16027;
        private const int INCREMENT_OP_ID = 16028;
        private const int DELTA_OP_ID = 16029;
        private const int TAIL_OP_ID = 16030;
        private const int FOREIGN_INSTR_ID = 16031;
        private const int ELEMENT_ID = 16032;
        private const int TEXT_ID = 16033;

        private static readonly QName RESET_PROPERTY = new QName("reset", NAMESPACE);

        private static readonly Dictionary<MessageTemplate, ISessionMessageHandler> MessageHandlers =
            new Dictionary<MessageTemplate, ISessionMessageHandler>();

        private static readonly MessageTemplate FASTAlertTemplate;
        private static readonly MessageTemplate FASTHelloTemplate;
        private new static readonly Message RESET;

        /// <summary>
        /// ************************ MESSAGE HANDLERS *********************************************
        /// </summary>
        private static readonly IMessageHandler ResetHandler;

        private static readonly ISessionMessageHandler AlertHandler;

        private static readonly MessageTemplate Attribute;
        private static readonly MessageTemplate Element;
        private static MessageTemplate _staticOther;

        private static readonly MessageTemplate TemplateName;
        private static readonly MessageTemplate NsName;
        private static readonly MessageTemplate NsNameWithAuxId;
        private static readonly MessageTemplate FieldBase;
        private static readonly MessageTemplate PrimFieldBase;

        private static MessageTemplate _staticLengthPreamble;
        private static MessageTemplate _staticPrimFieldBaseWithLength;

        public static readonly MessageTemplate INT32_INSTR;
        public static readonly MessageTemplate UINT32_INSTR;
        public static readonly MessageTemplate INT64_INSTR;
        public static readonly MessageTemplate UINT64_INSTR;
        public static readonly MessageTemplate DECIMAL_INSTR;
        public static readonly MessageTemplate UNICODE_INSTR;
        public static readonly MessageTemplate ASCII_INSTR;
        public static readonly MessageTemplate BYTE_VECTOR_INSTR;
        public static readonly MessageTemplate TEMPLATE_DEFINITION;
        public static MessageTemplate staticTAIL_OP;
        public static MessageTemplate staticDELTA_OP;
        public static readonly MessageTemplate GROUP_INSTR;
        public static readonly MessageTemplate SEQUENCE_INSTR;
        public static readonly MessageTemplate TEXT;
        public static readonly MessageTemplate COMP_DECIMAL_INSTR;

        private static MessageTemplate _staticTypeRef;
        private static MessageTemplate _staticTemplateDeclaration;
        private static MessageTemplate _staticOpBase;
        private static MessageTemplate _staticConstantOp;
        private static MessageTemplate _staticDefaultOp;
        private static MessageTemplate _staticCopyOp;
        private static MessageTemplate _staticIncrementOp;
        private static MessageTemplate _staticStatTempRefInstr;
        private static MessageTemplate _staticDynTempRefInstr;
        private static MessageTemplate _staticForeignInstr;
        private static Message _staticDynTempRefMessage;

        private static readonly ITemplateRegistry TemplateRegistry = new BasicTemplateRegistry();

        private static Message _staticClose;

        private readonly ConversionContext _initialContext = CreateInitialContext();

        static SessionControlProtocol_1_1()
        {
            FASTAlertTemplate = new MessageTemplate(
                "Alert",
                new Field[]
                    {
                        new Scalar("Severity", Type.U32, Operator.NONE, ScalarValue.Undefined, false),
                        new Scalar("Code", Type.U32, Operator.NONE, ScalarValue.Undefined, false),
                        new Scalar("Value", Type.U32, Operator.NONE, ScalarValue.Undefined, true),
                        new Scalar("Description", Type.ASCII, Operator.NONE, ScalarValue.Undefined, false)
                    });
            FASTHelloTemplate = new MessageTemplate(
                "Hello",
                new Field[]
                    {
                        new Scalar("SenderName", Type.ASCII, Operator.NONE, ScalarValue.Undefined, false),
                        new Scalar("VendorId", Type.ASCII, Operator.NONE, ScalarValue.Undefined, true)
                    });

            RESET = new ResetMessageObj(FastResetTemplate);
            FastResetTemplate.AddAttribute(RESET_PROPERTY, "yes");

            ResetHandler = new ResetMessageHandler();
            AlertHandler = new AlertSessionMessageHandler();
            Attribute = new MessageTemplate(
                new QName("Attribute", NAMESPACE),
                new[] {dict("Ns", true, DictionaryFields.Template), unicode("Name"), unicode("Value")});
            Element = new MessageTemplate(
                new QName("Element", NAMESPACE),
                new[]
                    {
                        dict("Ns", true, DictionaryFields.Template), unicode("Name"),
                        new Sequence(
                            qualify("Attributes"), new Field[] {new StaticTemplateReference(Attribute)}, false),
                        new Sequence(
                            qualify("Content"), new Field[] {DynamicTemplateReference.INSTANCE}, false)
                    });
            TemplateName = new MessageTemplate(
                new QName("TemplateName", NAMESPACE),
                new Field[]
                    {
                        new Scalar(qualify("Ns"), Type.UNICODE, Operator.COPY, null, false),
                        new Scalar(qualify("Name"), Type.UNICODE, Operator.NONE, null, false)
                    });
            NsName = new MessageTemplate(
                new QName("NsName", NAMESPACE),
                new[]
                    {
                        dict("Ns", false, DictionaryFields.Template),
                        new Scalar(qualify("Name"), Type.UNICODE, Operator.NONE, null, false)
                    });
            NsNameWithAuxId = new MessageTemplate(
                new QName("NsNameWithAuxId", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(NsName),
                        new Scalar(qualify("AuxId"), Type.UNICODE, Operator.NONE, null, true)
                    });
            FieldBase = new MessageTemplate(
                new QName("PrimFieldBase", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(NsNameWithAuxId),
                        new Scalar(qualify("Optional"), Type.U32, Operator.NONE, null, false),
                        new StaticTemplateReference(Other)
                    });
            PrimFieldBase = new MessageTemplate(
                new QName("PrimFieldBase", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(FieldBase),
                        new Group(qualify("Operator"), new Field[] {DynamicTemplateReference.INSTANCE}, true)
                    });
            INT32_INSTR = new MessageTemplate(
                new QName("Int32Instr", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(qualify("InitialValue"), Type.I32, Operator.NONE, null, true)
                    });
            UINT32_INSTR = new MessageTemplate(
                new QName("UInt32Instr", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(qualify("InitialValue"), Type.U32, Operator.NONE, null, true)
                    });
            INT64_INSTR = new MessageTemplate(
                new QName("Int64Instr", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(qualify("InitialValue"), Type.I64, Operator.NONE, null, true)
                    });
            UINT64_INSTR = new MessageTemplate(
                new QName("UInt64Instr", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(qualify("InitialValue"), Type.U64, Operator.NONE, null, true)
                    });
            DECIMAL_INSTR = new MessageTemplate(
                new QName("DecimalInstr", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(qualify("InitialValue"), Type.DECIMAL, Operator.NONE, null, true)
                    });
            UNICODE_INSTR = new MessageTemplate(
                new QName("UnicodeInstr", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBaseWithLength),
                        new Scalar(qualify("InitialValue"), Type.UNICODE, Operator.NONE, null, true)
                    });
            ASCII_INSTR = new MessageTemplate(
                new QName("AsciiInstr", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(qualify("InitialValue"), Type.ASCII, Operator.NONE, null, true)
                    });
            BYTE_VECTOR_INSTR = new MessageTemplate(
                new QName("ByteVectorInstr", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBaseWithLength),
                        new Scalar(qualify("InitialValue"), Type.BYTE_VECTOR, Operator.NONE, null, true)
                    });
            TEMPLATE_DEFINITION = new MessageTemplate(
                new QName("TemplateDef", NAMESPACE),
                new[]
                    {
                        new StaticTemplateReference(TemplateName),
                        unicodeopt("AuxId"), u32opt("TemplateId"),
                        new StaticTemplateReference(TypeRef), u32("Reset"),
                        new StaticTemplateReference(Other),
                        new Sequence(qualify("Instructions"), new Field[] {DynamicTemplateReference.INSTANCE}, false)
                    });
            GROUP_INSTR = new MessageTemplate(
                new QName("GroupInstr", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(FieldBase),
                        new StaticTemplateReference(TypeRef),
                        new Sequence(qualify("Instructions"),
                                     new Field[] {DynamicTemplateReference.INSTANCE},
                                     false)
                    });
            SEQUENCE_INSTR = new MessageTemplate(
                new QName("SequenceInstr", NAMESPACE),
                new Field[]
                    {
                        new StaticTemplateReference(FieldBase),
                        new StaticTemplateReference(TypeRef),
                        new Group(
                            qualify("Length"),
                            new Field[]
                                {
                                    new Group(qualify("Name"),
                                              new Field[] {new StaticTemplateReference(NsNameWithAuxId)}, true),
                                    new Group(qualify("Operator"),
                                              new Field[] {DynamicTemplateReference.INSTANCE}, true),
                                    new Scalar(qualify("InitialValue"), Type.U32, Operator.NONE, null, true),
                                    new StaticTemplateReference(Other)
                                }, true),
                        new Sequence(qualify("Instructions"), new Field[] {DynamicTemplateReference.INSTANCE}, false)
                    });
            TEXT = new MessageTemplate(
                qualify("Text"),
                new Field[]
                    {
                        new Scalar(qualify("Value"), Type.UNICODE, Operator.NONE, ScalarValue.Undefined, false)
                    });
            COMP_DECIMAL_INSTR = new MessageTemplate(
                qualify("CompositeDecimalInstr"),
                new Field[]
                    {
                        new StaticTemplateReference(FieldBase),
                        new Group(
                            qualify("Exponent"),
                            new Field[]
                                {
                                    new Group(
                                        qualify("Operator"), new Field[] {DynamicTemplateReference.INSTANCE}, false),
                                    new Scalar(
                                        qualify("InitialValue"), Type.I32, Operator.NONE, ScalarValue.Undefined, true),
                                    new StaticTemplateReference(Other)
                                }, true),
                        new Group(
                            qualify("Mantissa"),
                            new Field[]
                                {
                                    new Group(
                                        qualify("Operator"),
                                        new Field[] {DynamicTemplateReference.INSTANCE}, false),
                                    new Scalar(
                                        qualify("InitialValue"), Type.I32, Operator.NONE, ScalarValue.Undefined, true),
                                    new StaticTemplateReference(Other)
                                }, true)
                    });
            {
                TemplateRegistry.Register(FAST_HELLO_TEMPLATE_ID, FASTHelloTemplate);
                TemplateRegistry.Register(FAST_ALERT_TEMPLATE_ID, FASTAlertTemplate);
                TemplateRegistry.Register(FAST_RESET_TEMPLATE_ID, FastResetTemplate);
                TemplateRegistry.Register(TEMPLATE_DECL_ID, TemplateDeclaration);
                TemplateRegistry.Register(TEMPLATE_DEF_ID, TEMPLATE_DEFINITION);
                TemplateRegistry.Register(INT32_INSTR_ID, INT32_INSTR);
                TemplateRegistry.Register(UINT32_INSTR_ID, UINT32_INSTR);
                TemplateRegistry.Register(INT64_INSTR_ID, INT64_INSTR);
                TemplateRegistry.Register(UINT64_INSTR_ID, UINT64_INSTR);
                TemplateRegistry.Register(DECIMAL_INSTR_ID, DECIMAL_INSTR);
                TemplateRegistry.Register(COMP_DECIMAL_INSTR_ID, COMP_DECIMAL_INSTR);
                TemplateRegistry.Register(ASCII_INSTR_ID, ASCII_INSTR);
                TemplateRegistry.Register(UNICODE_INSTR_ID, UNICODE_INSTR);
                TemplateRegistry.Register(BYTE_VECTOR_INSTR_ID, BYTE_VECTOR_INSTR);
                TemplateRegistry.Register(STAT_TEMP_REF_INSTR_ID, STAT_TEMP_REF_INSTR);
                TemplateRegistry.Register(DYN_TEMP_REF_INSTR_ID, DYN_TEMP_REF_INSTR);
                TemplateRegistry.Register(SEQUENCE_INSTR_ID, SEQUENCE_INSTR);
                TemplateRegistry.Register(GROUP_INSTR_ID, GROUP_INSTR);
                TemplateRegistry.Register(CONSTANT_OP_ID, ConstantOp);
                TemplateRegistry.Register(DEFAULT_OP_ID, DEFAULT_OP);
                TemplateRegistry.Register(COPY_OP_ID, COPY_OP);
                TemplateRegistry.Register(INCREMENT_OP_ID, INCREMENT_OP);
                TemplateRegistry.Register(DELTA_OP_ID, DELTA_OP);
                TemplateRegistry.Register(TAIL_OP_ID, TAIL_OP);
                TemplateRegistry.Register(FOREIGN_INSTR_ID, FOREIGN_INSTR);
                TemplateRegistry.Register(ELEMENT_ID, Element);
                TemplateRegistry.Register(TEXT_ID, TEXT);

                foreach (MessageTemplate t in TemplateRegistry.Templates)
                    SetNamespaces(t);
            }
        }

        public SessionControlProtocol_1_1()
        {
            MessageHandlers[FASTAlertTemplate] = AlertHandler;
            MessageHandlers[TEMPLATE_DEFINITION] = new ProtocolDefinationSessionMessageHandler(this);
            MessageHandlers[TemplateDeclaration] = new ProtocolDeclarationSessionMessageHandler(this);
        }

        public override Message CloseMessage
        {
            get { return CLOSE; }
        }

        private static MessageTemplate Other
        {
            get
            {
                return _staticOther ??
                       (_staticOther =
                        new MessageTemplate(
                            new QName("Other", NAMESPACE),
                            new Field[]
                                {
                                    new Group(
                                        qualify("Other"),
                                        new Field[]
                                            {
                                                new Sequence(
                                                    qualify("ForeignAttributes"),
                                                    new Field[] {new StaticTemplateReference(Attribute)}, true),
                                                new Sequence(
                                                    qualify("ForeignElements"),
                                                    new Field[] {new StaticTemplateReference(Element)}, true)
                                            }, true)
                                }));
            }
        }

        private static MessageTemplate LengthPreamble
        {
            get
            {
                return _staticLengthPreamble ??
                       (_staticLengthPreamble =
                        new MessageTemplate(
                            new QName("LengthPreamble", NAMESPACE),
                            new Field[]
                                {
                                    new StaticTemplateReference(NsNameWithAuxId),
                                    new StaticTemplateReference(Other)
                                }));
            }
        }

        private static MessageTemplate PrimFieldBaseWithLength
        {
            get
            {
                return _staticPrimFieldBaseWithLength ??
                       (_staticPrimFieldBaseWithLength =
                        new MessageTemplate(
                            new QName("PrimFieldBaseWithLength", NAMESPACE),
                            new Field[]
                                {
                                    new StaticTemplateReference(PrimFieldBase),
                                    new Group(qualify("Length"),
                                              new Field[] {new StaticTemplateReference(LengthPreamble)}, true)
                                }));
            }
        }

        public static MessageTemplate TypeRef
        {
            get
            {
                return _staticTypeRef ??
                       (_staticTypeRef = new MessageTemplate(
                                             new QName("TypeRef", NAMESPACE),
                                             new Field[]
                                                 {
                                                     new Group(
                                                         qualify("TypeRef"),
                                                         new Field[]
                                                             {
                                                                 new StaticTemplateReference(NsName),
                                                                 new StaticTemplateReference(Other)
                                                             },
                                                         true)
                                                 }));
            }
        }

        public static MessageTemplate TemplateDeclaration
        {
            get
            {
                return _staticTemplateDeclaration ??
                       (_staticTemplateDeclaration =
                        new MessageTemplate(
                            new QName("TemplateDecl", NAMESPACE),
                            new[]
                                {
                                    new StaticTemplateReference(TemplateName), u32("TemplateId")
                                }));
            }
        }

        public static MessageTemplate OpBase
        {
            get
            {
                return _staticOpBase ??
                       (_staticOpBase =
                        new MessageTemplate(
                            new QName("OpBase", NAMESPACE),
                            new[]
                                {
                                    unicodeopt("Dictionary"),
                                    new Group(qualify("Key"), new Field[] {new StaticTemplateReference(NsName)}, true),
                                    new StaticTemplateReference(Other)
                                }));
            }
        }

        public static MessageTemplate ConstantOp
        {
            get
            {
                return _staticConstantOp ??
                       (_staticConstantOp = new MessageTemplate(
                                                new QName("ConstantOp", NAMESPACE),
                                                new Field[] {new StaticTemplateReference(Other)}));
            }
        }

        public static MessageTemplate DEFAULT_OP
        {
            get
            {
                return _staticDefaultOp ??
                       (_staticDefaultOp = new MessageTemplate(
                                               new QName("DefaultOp", NAMESPACE),
                                               new Field[] {new StaticTemplateReference(Other)}));
            }
        }

        public static MessageTemplate COPY_OP
        {
            get
            {
                return _staticCopyOp ??
                       (_staticCopyOp = new MessageTemplate(
                                            new QName("CopyOp", NAMESPACE),
                                            new Field[] {new StaticTemplateReference(OpBase)}));
            }
        }

        public static MessageTemplate INCREMENT_OP
        {
            get
            {
                return _staticIncrementOp ??
                       (_staticIncrementOp = new MessageTemplate(
                                                 new QName("IncrementOp", NAMESPACE),
                                                 new Field[] {new StaticTemplateReference(OpBase)}));
            }
        }

        public static MessageTemplate DELTA_OP
        {
            get
            {
                return staticDELTA_OP
                       ?? (staticDELTA_OP = new MessageTemplate(
                                                new QName("DeltaOp", NAMESPACE),
                                                new Field[] {new StaticTemplateReference(OpBase)}));
            }
        }

        public static MessageTemplate TAIL_OP
        {
            get
            {
                return staticTAIL_OP ??
                       (staticTAIL_OP = new MessageTemplate(
                                            new QName("TailOp", NAMESPACE),
                                            new Field[] {new StaticTemplateReference(OpBase)}));
            }
        }

        public static MessageTemplate STAT_TEMP_REF_INSTR
        {
            get
            {
                return _staticStatTempRefInstr
                       ?? (_staticStatTempRefInstr =
                           new MessageTemplate(
                               new QName("StaticTemplateRefInstr", NAMESPACE),
                               new Field[]
                                   {
                                       new StaticTemplateReference(TemplateName),
                                       new StaticTemplateReference(Other)
                                   }));
            }
        }

        public static MessageTemplate DYN_TEMP_REF_INSTR
        {
            get
            {
                return _staticDynTempRefInstr
                       ?? (_staticDynTempRefInstr = new MessageTemplate(
                                                        new QName("DynamicTemplateRefInstr", NAMESPACE),
                                                        new Field[] {new StaticTemplateReference(Other)}));
            }
        }

        public static MessageTemplate FOREIGN_INSTR
        {
            get
            {
                return _staticForeignInstr
                       ?? (_staticForeignInstr = new MessageTemplate(
                                                     qualify("ForeignInstr"),
                                                     new Field[] {new StaticTemplateReference(Element)}));
            }
        }

        public static Message DYN_TEMP_REF_MESSAGE
        {
            get
            {
                return _staticDynTempRefMessage
                       ?? (_staticDynTempRefMessage = new Message(DYN_TEMP_REF_INSTR));
            }
        }

        private static Message CLOSE
        {
            get
            {
                return _staticClose ??
                       (_staticClose = CreateFastAlertMessage(SessionConstants.CLOSE));
            }
        }

        private static void SetNamespaces(Group value)
        {
            value.ChildNamespace = NAMESPACE;
            foreach (Field fld in value.Fields)
            {
                var grp = fld as Group;
                if (grp != null)
                    SetNamespaces(grp);
            }
        }

        public static ConversionContext CreateInitialContext()
        {
            var context = new ConversionContext();
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

        public override void ConfigureSession(Session session)
        {
            RegisterSessionTemplates(session.MessageInputStream.GetTemplateRegistry());
            RegisterSessionTemplates(session.MessageOutputStream.GetTemplateRegistry());
            session.MessageInputStream.AddMessageHandler(FastResetTemplate, ResetHandler);
            session.MessageOutputStream.AddMessageHandler(FastResetTemplate, ResetHandler);
        }

        public virtual void RegisterSessionTemplates(ITemplateRegistry registry)
        {
            registry.RegisterAll(TemplateRegistry);
        }

        public override Session Connect(string senderName, IConnection connection, ITemplateRegistry inboundRegistry,
                                        ITemplateRegistry outboundRegistry, IMessageListener messageListener,
                                        ISessionListener sessionListener)
        {
            var session = new Session(connection, this, TemplateRegistryFields.Null, TemplateRegistryFields.Null);
            session.MessageOutputStream.WriteMessage(CreateHelloMessage(senderName));
            try
            {
                Thread.Sleep(new TimeSpan((Int64) 10000*20));
            }
            catch (ThreadInterruptedException)
            {
            }

            Message message = session.MessageInputStream.ReadMessage();
            string serverName = message.GetString(1);
            string vendorId = message.IsDefined(2) ? message.GetString(2) : "unknown";
            session.Client = new BasicClient(serverName, vendorId);
            return session;
        }

        public override void OnError(Session session, ErrorCode code, string message)
        {
            session.MessageOutputStream.WriteMessage(CreateFastAlertMessage(code));
        }

        public override Session OnNewConnection(string serverName, IConnection connection)
        {
            var session = new Session(connection, this, TemplateRegistryFields.Null, TemplateRegistryFields.Null);
            Message message = session.MessageInputStream.ReadMessage();
            string clientName = message.GetString(1);
            string vendorId = message.IsDefined(2) ? message.GetString(2) : "unknown";
            session.Client = new BasicClient(clientName, vendorId);
            session.MessageOutputStream.WriteMessage(CreateHelloMessage(serverName));
            return session;
        }

        public virtual Message CreateHelloMessage(string senderName)
        {
            var message = new Message(FASTHelloTemplate);
            message.SetString(1, senderName);
            message.SetString(2, SessionConstants.VENDOR_ID);
            return message;
        }

        public static Message CreateFastAlertMessage(ErrorCode code)
        {
            var alert = new Message(FASTAlertTemplate);
            alert.SetInteger(1, code.Severity.Code);
            alert.SetInteger(2, code.Code);
            alert.SetString(4, code.Description);
            return alert;
        }

        public override void HandleMessage(Session session, Message message)
        {
            ISessionMessageHandler value;
            if (!MessageHandlers.TryGetValue(message.Template, out value))
                return;
            value.HandleMessage(session, message);
        }

        public override bool IsProtocolMessage(Message message)
        {
            if (message == null)
                return false;
            return MessageHandlers.ContainsKey(message.Template);
        }

        public override bool SupportsTemplateExchange()
        {
            return true;
        }

        public override Message CreateTemplateDeclarationMessage(MessageTemplate messageTemplate, int templateId)
        {
            var declaration = new Message(TemplateDeclaration);
            AbstractFieldInstructionConverter.SetName(messageTemplate, declaration);
            declaration.SetInteger("TemplateId", templateId);
            return declaration;
        }

        public override Message CreateTemplateDefinitionMessage(MessageTemplate messageTemplate)
        {
            Message templateDefinition = GroupConverter.Convert(messageTemplate, new Message(TEMPLATE_DEFINITION),
                                                                _initialContext);
            int reset = messageTemplate.HasAttribute(RESET_PROPERTY) ? 1 : 0;
            templateDefinition.SetInteger("Reset", reset);
            return templateDefinition;
        }

        public virtual MessageTemplate CreateTemplateFromMessage(Message templateDef, ITemplateRegistry registry)
        {
            //string name = templateDef.GetString("Name");
            //Field[] fields = GroupConverter.ParseFieldInstructions(templateDef, registry, _initialContext);
            //return new MessageTemplate(name, fields);
            string name = templateDef.GetString("Name");
            string tempnamespace = "";
            if (templateDef.IsDefined("Ns"))
                tempnamespace = templateDef.GetString("Ns");
            Field[] fields = GroupConverter.ParseFieldInstructions(templateDef, registry, _initialContext);
            var group = new MessageTemplate(new QName(name, tempnamespace), fields);
            if (templateDef.IsDefined("TypeRef"))
            {
                GroupValue typeRef = templateDef.GetGroup("TypeRef");
                string typeRefName = typeRef.GetString("Name");
                string typeRefNs = ""; // context.getNamespace();
                if (typeRef.IsDefined("Ns"))
                    typeRefNs = typeRef.GetString("Ns");
                group.SetTypeReference(new QName(typeRefName, typeRefNs));
            }
            if (templateDef.IsDefined("AuxId"))
            {
                group.Id = templateDef.GetString("AuxId");
            }
            return group;
        }

        private static Field u32(string name)
        {
            return new Scalar(qualify(name), Type.U32, Operator.NONE, null, false);
        }

        private static Field dict(string name, bool optional, string dictionary)
        {
            var scalar = new Scalar(qualify(name), Type.UNICODE, Operator.COPY, null, optional)
                             {Dictionary = dictionary};
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

        #region Nested type: AlertSessionMessageHandler

        public class AlertSessionMessageHandler : ISessionMessageHandler
        {
            #region ISessionMessageHandler Members

            public virtual void HandleMessage(Session session, Message message)
            {
                ErrorCode alertCode = ErrorCode.GetAlertCode(message);
                if (alertCode.Equals(SessionConstants.CLOSE))
                {
                    session.Close(alertCode);
                }
                else
                {
                    session.ErrorHandler.Error(alertCode, message.GetString(4));
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: ProtocolDeclarationSessionMessageHandler

        private sealed class ProtocolDeclarationSessionMessageHandler : ISessionMessageHandler
        {
            private SessionControlProtocol_1_1 _enclosingInstance;

            public ProtocolDeclarationSessionMessageHandler(SessionControlProtocol_1_1 enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }

            #region ISessionMessageHandler Members

            public void HandleMessage(Session session, Message message)
            {
                session.RegisterDynamicTemplate(_enclosingInstance.GetQName(message), message.GetInt("TemplateId"));
            }

            #endregion

            private void InitBlock(SessionControlProtocol_1_1 internalInstance)
            {
                _enclosingInstance = internalInstance;
            }
        }

        #endregion

        #region Nested type: ProtocolDefinationSessionMessageHandler

        private sealed class ProtocolDefinationSessionMessageHandler : ISessionMessageHandler
        {
            private SessionControlProtocol_1_1 _enclosingInstance;

            public ProtocolDefinationSessionMessageHandler(SessionControlProtocol_1_1 enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }

            #region ISessionMessageHandler Members

            public void HandleMessage(Session session, Message message)
            {
                MessageTemplate template = _enclosingInstance.CreateTemplateFromMessage(
                    message, session.MessageInputStream.GetTemplateRegistry());
                session.AddDynamicTemplateDefinition(template);
                if (message.IsDefined("TemplateId"))
                    session.RegisterDynamicTemplate(template.QName, message.GetInt("TemplateId"));
            }

            #endregion

            private void InitBlock(SessionControlProtocol_1_1 internalInstance)
            {
                _enclosingInstance = internalInstance;
            }
        }

        #endregion

        #region Nested type: ResetMessageHandler

        private sealed class ResetMessageHandler : IMessageHandler
        {
            #region IMessageHandler Members

            public void HandleMessage(Message readMessage, Context context, ICoder coder)
            {
                if (readMessage.Template.HasAttribute(RESET_PROPERTY))
                    coder.Reset();
            }

            #endregion
        }

        #endregion

        #region Nested type: ResetMessageObj

        [Serializable]
        public class ResetMessageObj : Message
        {
            internal ResetMessageObj(MessageTemplate template) : base(template)
            {
            }

            public override void SetFieldValue(int fieldIndex, IFieldValue value)
            {
                throw new InvalidOperationException("Cannot set values on a fast reserved message.");
            }
        }

        #endregion
    }
}