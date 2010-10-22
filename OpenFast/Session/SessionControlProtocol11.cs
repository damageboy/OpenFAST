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
    public class SessionControlProtocol11 : AbstractSessionControlProtocol
    {
        public const string Namespace = "http://www.fixprotocol.org/ns/fast/scp/1.1";

        private const int ResetTemplateId = 120;
        private const int HelloTemplateId = 16002;
        private const int AlertTemplateId = 16003;
        private const int TemplateDeclId = 16010;
        private const int TemplateDefId = 16011;
        private const int Int32InstrId = 16012;
        private const int Uint32InstrId = 16013;
        private const int Int64InstrId = 16014;
        private const int Uint64InstrId = 16015;
        private const int DecimalInstrId = 16016;
        private const int CompDecimalInstrId = 16017;
        private const int AsciiInstrId = 16018;
        private const int UnicodeInstrId = 16019;
        private const int ByteVectorInstrId = 16020;
        private const int StatTempRefInstrId = 16021;
        private const int DynTempRefInstrId = 16022;
        private const int SequenceInstrId = 16023;
        private const int GroupInstrId = 16024;
        private const int ConstantOpId = 16025;
        private const int DefaultOpId = 16026;
        private const int CopyOpId = 16027;
        private const int IncrementOpId = 16028;
        private const int DeltaOpId = 16029;
        private const int TailOpId = 16030;
        private const int ForeignInstrId = 16031;
        private const int ElementId = 16032;
        private const int TextId = 16033;

        private static readonly QName ResetProperty = new QName("reset", Namespace);

        private static readonly Dictionary<MessageTemplate, ISessionMessageHandler> MessageHandlers =
            new Dictionary<MessageTemplate, ISessionMessageHandler>();

        private static readonly MessageTemplate FASTAlertTemplate;
        private static readonly MessageTemplate FASTHelloTemplate;
#warning usage?
        private static readonly Message RESET;

        /// <summary>
        /// ************************ MESSAGE HANDLERS *********************************************
        /// </summary>
        private static readonly IMessageHandler ResetHandler;

        private static readonly ISessionMessageHandler AlertHandler;

        private static readonly MessageTemplate Attribute;
        private static readonly MessageTemplate Element;

        private static readonly MessageTemplate TemplateName;
        private static readonly MessageTemplate NsName;
        private static readonly MessageTemplate NsNameWithAuxId;
        private static readonly MessageTemplate FieldBase;
        private static readonly MessageTemplate PrimFieldBase;

        public static readonly MessageTemplate Int32Instr;
        public static readonly MessageTemplate Uint32Instr;
        public static readonly MessageTemplate Int64Instr;
        public static readonly MessageTemplate Uint64Instr;
        public static readonly MessageTemplate DecimalInstr;
        public static readonly MessageTemplate UnicodeInstr;
        public static readonly MessageTemplate AsciiInstr;
        public static readonly MessageTemplate ByteVectorInstr;
        public static readonly MessageTemplate TemplateDefinition;
        public static readonly MessageTemplate GroupInstr;
        public static readonly MessageTemplate SequenceInstr;
        public static readonly MessageTemplate Text;
        public static readonly MessageTemplate CompDecimalInstr;

        private static MessageTemplate _staticOther;
        private static MessageTemplate _staticTailOp;
        private static MessageTemplate _staticDeltaOp;
        private static MessageTemplate _staticLengthPreamble;
        private static MessageTemplate _staticPrimFieldBaseWithLength;
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
        private static Message _staticClose;

        private static readonly ITemplateRegistry TemplateRegistry = new BasicTemplateRegistry();

        private readonly ConversionContext _initialContext = CreateInitialContext();

        static SessionControlProtocol11()
        {
            FASTAlertTemplate = new MessageTemplate(
                "Alert",
                new Field[]
                    {
                        new Scalar("Severity", Type.U32, Operator.None, ScalarValue.Undefined, false),
                        new Scalar("Code", Type.U32, Operator.None, ScalarValue.Undefined, false),
                        new Scalar("Value", Type.U32, Operator.None, ScalarValue.Undefined, true),
                        new Scalar("Description", Type.Ascii, Operator.None, ScalarValue.Undefined, false)
                    });
            FASTHelloTemplate = new MessageTemplate(
                "Hello",
                new Field[]
                    {
                        new Scalar("SenderName", Type.Ascii, Operator.None, ScalarValue.Undefined, false),
                        new Scalar("VendorId", Type.Ascii, Operator.None, ScalarValue.Undefined, true)
                    });

            RESET = new ResetMessageObj(FastResetTemplate);
            FastResetTemplate.AddAttribute(ResetProperty, "yes");

            ResetHandler = new ResetMessageHandler();
            AlertHandler = new AlertSessionMessageHandler();
            Attribute = new MessageTemplate(
                new QName("Attribute", Namespace),
                new[] {Dict("Ns", true, DictionaryFields.Template), Unicode("Name"), Unicode("Value")});
            Element = new MessageTemplate(
                new QName("Element", Namespace),
                new[]
                    {
                        Dict("Ns", true, DictionaryFields.Template), Unicode("Name"),
                        new Sequence(
                            Qualify("Attributes"), new Field[] {new StaticTemplateReference(Attribute)}, false),
                        new Sequence(
                            Qualify("Content"), new Field[] {DynamicTemplateReference.INSTANCE}, false)
                    });
            TemplateName = new MessageTemplate(
                new QName("TemplateName", Namespace),
                new Field[]
                    {
                        new Scalar(Qualify("Ns"), Type.Unicode, Operator.Copy, null, false),
                        new Scalar(Qualify("Name"), Type.Unicode, Operator.None, null, false)
                    });
            NsName = new MessageTemplate(
                new QName("NsName", Namespace),
                new[]
                    {
                        Dict("Ns", false, DictionaryFields.Template),
                        new Scalar(Qualify("Name"), Type.Unicode, Operator.None, null, false)
                    });
            NsNameWithAuxId = new MessageTemplate(
                new QName("NsNameWithAuxId", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(NsName),
                        new Scalar(Qualify("AuxId"), Type.Unicode, Operator.None, null, true)
                    });
            FieldBase = new MessageTemplate(
                new QName("PrimFieldBase", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(NsNameWithAuxId),
                        new Scalar(Qualify("Optional"), Type.U32, Operator.None, null, false),
                        new StaticTemplateReference(Other)
                    });
            PrimFieldBase = new MessageTemplate(
                new QName("PrimFieldBase", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(FieldBase),
                        new Group(Qualify("Operator"), new Field[] {DynamicTemplateReference.INSTANCE}, true)
                    });
            Int32Instr = new MessageTemplate(
                new QName("Int32Instr", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(Qualify("InitialValue"), Type.I32, Operator.None, null, true)
                    });
            Uint32Instr = new MessageTemplate(
                new QName("UInt32Instr", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(Qualify("InitialValue"), Type.U32, Operator.None, null, true)
                    });
            Int64Instr = new MessageTemplate(
                new QName("Int64Instr", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(Qualify("InitialValue"), Type.I64, Operator.None, null, true)
                    });
            Uint64Instr = new MessageTemplate(
                new QName("UInt64Instr", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(Qualify("InitialValue"), Type.U64, Operator.None, null, true)
                    });
            DecimalInstr = new MessageTemplate(
                new QName("DecimalInstr", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(Qualify("InitialValue"), Type.Decimal, Operator.None, null, true)
                    });
            UnicodeInstr = new MessageTemplate(
                new QName("UnicodeInstr", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBaseWithLength),
                        new Scalar(Qualify("InitialValue"), Type.Unicode, Operator.None, null, true)
                    });
            AsciiInstr = new MessageTemplate(
                new QName("AsciiInstr", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(Qualify("InitialValue"), Type.Ascii, Operator.None, null, true)
                    });
            ByteVectorInstr = new MessageTemplate(
                new QName("ByteVectorInstr", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBaseWithLength),
                        new Scalar(Qualify("InitialValue"), Type.ByteVector, Operator.None, null, true)
                    });
            TemplateDefinition = new MessageTemplate(
                new QName("TemplateDef", Namespace),
                new[]
                    {
                        new StaticTemplateReference(TemplateName),
                        Unicodeopt("AuxId"), U32Opt("TemplateId"),
                        new StaticTemplateReference(TypeRef), U32("Reset"),
                        new StaticTemplateReference(Other),
                        new Sequence(Qualify("Instructions"), new Field[] {DynamicTemplateReference.INSTANCE}, false)
                    });
            GroupInstr = new MessageTemplate(
                new QName("GroupInstr", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(FieldBase),
                        new StaticTemplateReference(TypeRef),
                        new Sequence(Qualify("Instructions"),
                                     new Field[] {DynamicTemplateReference.INSTANCE},
                                     false)
                    });
            SequenceInstr = new MessageTemplate(
                new QName("SequenceInstr", Namespace),
                new Field[]
                    {
                        new StaticTemplateReference(FieldBase),
                        new StaticTemplateReference(TypeRef),
                        new Group(
                            Qualify("Length"),
                            new Field[]
                                {
                                    new Group(Qualify("Name"),
                                              new Field[] {new StaticTemplateReference(NsNameWithAuxId)}, true),
                                    new Group(Qualify("Operator"),
                                              new Field[] {DynamicTemplateReference.INSTANCE}, true),
                                    new Scalar(Qualify("InitialValue"), Type.U32, Operator.None, null, true),
                                    new StaticTemplateReference(Other)
                                }, true),
                        new Sequence(Qualify("Instructions"), new Field[] {DynamicTemplateReference.INSTANCE}, false)
                    });
            Text = new MessageTemplate(
                Qualify("Text"),
                new Field[]
                    {
                        new Scalar(Qualify("Value"), Type.Unicode, Operator.None, ScalarValue.Undefined, false)
                    });
            CompDecimalInstr = new MessageTemplate(
                Qualify("CompositeDecimalInstr"),
                new Field[]
                    {
                        new StaticTemplateReference(FieldBase),
                        new Group(
                            Qualify("Exponent"),
                            new Field[]
                                {
                                    new Group(
                                        Qualify("Operator"), new Field[] {DynamicTemplateReference.INSTANCE}, false),
                                    new Scalar(
                                        Qualify("InitialValue"), Type.I32, Operator.None, ScalarValue.Undefined, true),
                                    new StaticTemplateReference(Other)
                                }, true),
                        new Group(
                            Qualify("Mantissa"),
                            new Field[]
                                {
                                    new Group(
                                        Qualify("Operator"),
                                        new Field[] {DynamicTemplateReference.INSTANCE}, false),
                                    new Scalar(
                                        Qualify("InitialValue"), Type.I32, Operator.None, ScalarValue.Undefined, true),
                                    new StaticTemplateReference(Other)
                                }, true)
                    });
            {
                TemplateRegistry.Register(HelloTemplateId, FASTHelloTemplate);
                TemplateRegistry.Register(AlertTemplateId, FASTAlertTemplate);
                TemplateRegistry.Register(ResetTemplateId, FastResetTemplate);
                TemplateRegistry.Register(TemplateDeclId, TemplateDeclaration);
                TemplateRegistry.Register(TemplateDefId, TemplateDefinition);
                TemplateRegistry.Register(Int32InstrId, Int32Instr);
                TemplateRegistry.Register(Uint32InstrId, Uint32Instr);
                TemplateRegistry.Register(Int64InstrId, Int64Instr);
                TemplateRegistry.Register(Uint64InstrId, Uint64Instr);
                TemplateRegistry.Register(DecimalInstrId, DecimalInstr);
                TemplateRegistry.Register(CompDecimalInstrId, CompDecimalInstr);
                TemplateRegistry.Register(AsciiInstrId, AsciiInstr);
                TemplateRegistry.Register(UnicodeInstrId, UnicodeInstr);
                TemplateRegistry.Register(ByteVectorInstrId, ByteVectorInstr);
                TemplateRegistry.Register(StatTempRefInstrId, StatTempRefInstr);
                TemplateRegistry.Register(DynTempRefInstrId, DynTempRefInstr);
                TemplateRegistry.Register(SequenceInstrId, SequenceInstr);
                TemplateRegistry.Register(GroupInstrId, GroupInstr);
                TemplateRegistry.Register(ConstantOpId, ConstantOp);
                TemplateRegistry.Register(DefaultOpId, DefaultOp);
                TemplateRegistry.Register(CopyOpId, CopyOp);
                TemplateRegistry.Register(IncrementOpId, IncrementOp);
                TemplateRegistry.Register(DeltaOpId, DeltaOp);
                TemplateRegistry.Register(TailOpId, TailOp);
                TemplateRegistry.Register(ForeignInstrId, ForeignInstr);
                TemplateRegistry.Register(ElementId, Element);
                TemplateRegistry.Register(TextId, Text);

                foreach (MessageTemplate t in TemplateRegistry.Templates)
                    SetNamespaces(t);
            }
        }

        public SessionControlProtocol11()
        {
            MessageHandlers[FASTAlertTemplate] = AlertHandler;
            MessageHandlers[TemplateDefinition] = new ProtocolDefinationSessionMessageHandler(this);
            MessageHandlers[TemplateDeclaration] = new ProtocolDeclarationSessionMessageHandler(this);
        }

        public override Message CloseMessage
        {
            get { return Close; }
        }

        private static MessageTemplate Other
        {
            get
            {
                return _staticOther ??
                       (_staticOther =
                        new MessageTemplate(
                            new QName("Other", Namespace),
                            new Field[]
                                {
                                    new Group(
                                        Qualify("Other"),
                                        new Field[]
                                            {
                                                new Sequence(
                                                    Qualify("ForeignAttributes"),
                                                    new Field[] {new StaticTemplateReference(Attribute)}, true),
                                                new Sequence(
                                                    Qualify("ForeignElements"),
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
                            new QName("LengthPreamble", Namespace),
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
                            new QName("PrimFieldBaseWithLength", Namespace),
                            new Field[]
                                {
                                    new StaticTemplateReference(PrimFieldBase),
                                    new Group(Qualify("Length"),
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
                                             new QName("TypeRef", Namespace),
                                             new Field[]
                                                 {
                                                     new Group(
                                                         Qualify("TypeRef"),
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
                            new QName("TemplateDecl", Namespace),
                            new[]
                                {
                                    new StaticTemplateReference(TemplateName), U32("TemplateId")
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
                            new QName("OpBase", Namespace),
                            new[]
                                {
                                    Unicodeopt("Dictionary"),
                                    new Group(Qualify("Key"), new Field[] {new StaticTemplateReference(NsName)}, true),
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
                                                new QName("ConstantOp", Namespace),
                                                new Field[] {new StaticTemplateReference(Other)}));
            }
        }

        public static MessageTemplate DefaultOp
        {
            get
            {
                return _staticDefaultOp ??
                       (_staticDefaultOp = new MessageTemplate(
                                               new QName("DefaultOp", Namespace),
                                               new Field[] {new StaticTemplateReference(Other)}));
            }
        }

        public static MessageTemplate CopyOp
        {
            get
            {
                return _staticCopyOp ??
                       (_staticCopyOp = new MessageTemplate(
                                            new QName("CopyOp", Namespace),
                                            new Field[] {new StaticTemplateReference(OpBase)}));
            }
        }

        public static MessageTemplate IncrementOp
        {
            get
            {
                return _staticIncrementOp ??
                       (_staticIncrementOp = new MessageTemplate(
                                                 new QName("IncrementOp", Namespace),
                                                 new Field[] {new StaticTemplateReference(OpBase)}));
            }
        }

        public static MessageTemplate DeltaOp
        {
            get
            {
                return _staticDeltaOp
                       ?? (_staticDeltaOp = new MessageTemplate(
                                                new QName("DeltaOp", Namespace),
                                                new Field[] {new StaticTemplateReference(OpBase)}));
            }
        }

        public static MessageTemplate TailOp
        {
            get
            {
                return _staticTailOp ??
                       (_staticTailOp = new MessageTemplate(
                                            new QName("TailOp", Namespace),
                                            new Field[] {new StaticTemplateReference(OpBase)}));
            }
        }

        public static MessageTemplate StatTempRefInstr
        {
            get
            {
                return _staticStatTempRefInstr
                       ?? (_staticStatTempRefInstr =
                           new MessageTemplate(
                               new QName("StaticTemplateRefInstr", Namespace),
                               new Field[]
                                   {
                                       new StaticTemplateReference(TemplateName),
                                       new StaticTemplateReference(Other)
                                   }));
            }
        }

        public static MessageTemplate DynTempRefInstr
        {
            get
            {
                return _staticDynTempRefInstr
                       ?? (_staticDynTempRefInstr = new MessageTemplate(
                                                        new QName("DynamicTemplateRefInstr", Namespace),
                                                        new Field[] {new StaticTemplateReference(Other)}));
            }
        }

        public static MessageTemplate ForeignInstr
        {
            get
            {
                return _staticForeignInstr
                       ?? (_staticForeignInstr = new MessageTemplate(
                                                     Qualify("ForeignInstr"),
                                                     new Field[] {new StaticTemplateReference(Element)}));
            }
        }

        public static Message DynTempRefMessage
        {
            get
            {
                return _staticDynTempRefMessage
                       ?? (_staticDynTempRefMessage = new Message(DynTempRefInstr));
            }
        }

        private static Message Close
        {
            get
            {
                return _staticClose ??
                       (_staticClose = CreateFastAlertMessage(SessionConstants.Close));
            }
        }

        private static void SetNamespaces(Group value)
        {
            value.ChildNamespace = Namespace;
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
            message.SetString(2, SessionConstants.VendorId);
            return message;
        }

        public static Message CreateFastAlertMessage(ErrorCode code)
        {
            var alert = new Message(FASTAlertTemplate);
            alert.SetInteger(1, (int) code.Severity);
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
            Message templateDefinition = GroupConverter.Convert(messageTemplate, new Message(TemplateDefinition),
                                                                _initialContext);
            int reset = messageTemplate.HasAttribute(ResetProperty) ? 1 : 0;
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

        private static Field U32(string name)
        {
            return new Scalar(Qualify(name), Type.U32, Operator.None, null, false);
        }

        private static Field Dict(string name, bool optional, string dictionary)
        {
            var scalar = new Scalar(Qualify(name), Type.Unicode, Operator.Copy, null, optional)
                             {Dictionary = dictionary};
            return scalar;
        }

        private static QName Qualify(string name)
        {
            return new QName(name, Namespace);
        }

        private static Field Unicodeopt(string name)
        {
            return new Scalar(Qualify(name), Type.Unicode, Operator.None, null, true);
        }

        private static Field Unicode(string name)
        {
            return new Scalar(Qualify(name), Type.Unicode, Operator.None, null, false);
        }

        private static Field U32Opt(string name)
        {
            return new Scalar(Qualify(name), Type.U32, Operator.None, null, true);
        }

        #region Nested type: AlertSessionMessageHandler

        public class AlertSessionMessageHandler : ISessionMessageHandler
        {
            #region ISessionMessageHandler Members

            public virtual void HandleMessage(Session session, Message message)
            {
                ErrorCode alertCode = ErrorCode.GetAlertCode(message.GetInt(2));
                if (alertCode.Equals(SessionConstants.Close))
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
            private SessionControlProtocol11 _enclosingInstance;

            public ProtocolDeclarationSessionMessageHandler(SessionControlProtocol11 enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }

            #region ISessionMessageHandler Members

            public void HandleMessage(Session session, Message message)
            {
                session.RegisterDynamicTemplate(_enclosingInstance.GetQName(message), message.GetInt("TemplateId"));
            }

            #endregion

            private void InitBlock(SessionControlProtocol11 internalInstance)
            {
                _enclosingInstance = internalInstance;
            }
        }

        #endregion

        #region Nested type: ProtocolDefinationSessionMessageHandler

        private sealed class ProtocolDefinationSessionMessageHandler : ISessionMessageHandler
        {
            private SessionControlProtocol11 _enclosingInstance;

            public ProtocolDefinationSessionMessageHandler(SessionControlProtocol11 enclosingInstance)
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

            private void InitBlock(SessionControlProtocol11 internalInstance)
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
                if (readMessage.Template.HasAttribute(ResetProperty))
                    coder.Reset();
            }

            #endregion
        }

        #endregion

        #region Nested type: ResetMessageObj

#warning BUG? Is this object needed? It is almost identical to the parent's ResetMessageObj
        [Serializable]
        public class ResetMessageObj : Message
        {
            internal ResetMessageObj(MessageTemplate template)
                : base(template)
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