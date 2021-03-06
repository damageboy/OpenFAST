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
using OpenFAST.Sessions.Template.Exchange;
using OpenFAST.Template;
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;

namespace OpenFAST.Sessions
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

        #region Templates

        //
        // *** THE ORDERING IS CRITICAL: Some of these templates are referenced by other templates.
        // Make sure the template is initialized before (declared above) the template which uses it.
        // MS Compiler does not verify it. Use Resharper or other tool to see warnings.
        //

        protected new static readonly MessageTemplate FastResetTemplate =
            new MessageTemplate("Reset", Namespace, new Field[0]);

        public static readonly MessageTemplate AlertTemplate =
            new MessageTemplate(
                "Alert", Namespace,
                new[]
                    {
                        new Scalar("Severity", FastType.U32, Operator.None, ScalarValue.Undefined, false),
                        new Scalar("Code", FastType.U32, Operator.None, ScalarValue.Undefined, false),
                        new Scalar("Value", FastType.U32, Operator.None, ScalarValue.Undefined, true),
                        new Scalar("Description", FastType.Ascii, Operator.None, ScalarValue.Undefined, false)
                    });

        public static readonly MessageTemplate HelloTemplate =
            new MessageTemplate(
                "Hello", Namespace,
                new[]
                    {
                        new Scalar("SenderName", FastType.Ascii, Operator.None, ScalarValue.Undefined, false),
                        new Scalar("VendorId", FastType.Ascii, Operator.None, ScalarValue.Undefined, true)
                    });

        public static readonly MessageTemplate Attribute =
            new MessageTemplate(
                Qualify("Attribute"), Namespace,
                new[]
                    {
                        Dict("Ns", true, DictionaryFields.Template), Unicode("Name"), Unicode("Value"),
                    });

        public static readonly MessageTemplate Element =
            new MessageTemplate(
                Qualify("Element"), Namespace,
                new[]
                    {
                        Dict("Ns", true, DictionaryFields.Template), Unicode("Name"),
                        new Sequence(Qualify("Attributes"), new Field[] {new StaticTemplateReference(Attribute)}, false)
                        ,
                        new Sequence(Qualify("Content"), new Field[] {DynamicTemplateReference.Instance}, false)
                    });

        public static readonly MessageTemplate TemplateName =
            new MessageTemplate(
                Qualify("TemplateName"), Namespace,
                new[]
                    {
                        new Scalar(Qualify("Ns"), FastType.Unicode, Operator.Copy, null, false),
                        new Scalar(Qualify("Name"), FastType.Unicode, Operator.None, null, false)
                    });

        public static readonly MessageTemplate NsName =
            new MessageTemplate(
                Qualify("NsName"), Namespace,
                new[]
                    {
                        Dict("Ns", false, DictionaryFields.Template),
                        new Scalar(Qualify("Name"), FastType.Unicode, Operator.None, null, false)
                    });

        public static readonly MessageTemplate NsNameWithAuxId =
            new MessageTemplate(
                Qualify("NsNameWithAuxId"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(NsName),
                        new Scalar(Qualify("AuxId"), FastType.Unicode, Operator.None, null, true)
                    });

        public static readonly MessageTemplate Other =
            new MessageTemplate(
                Qualify("Other"), Namespace,
                new Field[]
                    {
                        new Group(
                            Qualify("Other"), Namespace,
                            new Field[]
                                {
                                    new Sequence(
                                        Qualify("ForeignAttributes"),
                                        new Field[] {new StaticTemplateReference(Attribute)}, true),
                                    new Sequence(
                                        Qualify("ForeignElements"),
                                        new Field[] {new StaticTemplateReference(Element)}, true)
                                }, true)
                    });

        public static readonly MessageTemplate FieldBase =
            new MessageTemplate(
                Qualify("PrimFieldBase"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(NsNameWithAuxId),
                        new Scalar(Qualify("Optional"), FastType.U32, Operator.None, null, false),
                        new StaticTemplateReference(Other)
                    });

        public static readonly MessageTemplate PrimFieldBase =
            new MessageTemplate(
                Qualify("PrimFieldBase"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(FieldBase),
                        new Group(Qualify("Operator"), Namespace, new Field[] {DynamicTemplateReference.Instance}, true)
                    });

        public static readonly MessageTemplate Int32Instr =
            new MessageTemplate(
                Qualify("Int32Instr"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(Qualify("InitialValue"), FastType.I32, Operator.None, null, true)
                    });

        public static readonly MessageTemplate Uint32Instr =
            new MessageTemplate(
                Qualify("UInt32Instr"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(Qualify("InitialValue"), FastType.U32, Operator.None, null, true)
                    });

        public static readonly MessageTemplate Int64Instr =
            new MessageTemplate(
                Qualify("Int64Instr"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(Qualify("InitialValue"), FastType.I64, Operator.None, null, true)
                    });

        public static readonly MessageTemplate Uint64Instr =
            new MessageTemplate(
                Qualify("UInt64Instr"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(Qualify("InitialValue"), FastType.U64, Operator.None, null, true)
                    });

        public static readonly MessageTemplate DecimalInstr =
            new MessageTemplate(
                Qualify("DecimalInstr"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(Qualify("InitialValue"), FastType.Decimal, Operator.None, null, true)
                    });

        private static readonly MessageTemplate LengthPreamble =
            new MessageTemplate(
                Qualify("LengthPreamble"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(NsNameWithAuxId),
                        new StaticTemplateReference(Other)
                    });

        private static readonly MessageTemplate PrimFieldBaseWithLength =
            new MessageTemplate(
                Qualify("PrimFieldBaseWithLength"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Group(Qualify("Length"), Namespace,
                                  new Field[] {new StaticTemplateReference(LengthPreamble)}, true)
                    });

        public static readonly MessageTemplate TypeRef =
            new MessageTemplate(
                Qualify("TypeRef"), Namespace,
                new Field[]
                    {
                        new Group(
                            Qualify("TypeRef"), Namespace,
                            new Field[]
                                {
                                    new StaticTemplateReference(NsName),
                                    new StaticTemplateReference(Other)
                                },
                            true)
                    });

        public static readonly MessageTemplate TemplateDeclaration =
            new MessageTemplate(
                Qualify("TemplateDecl"), Namespace,
                new[]
                    {
                        new StaticTemplateReference(TemplateName), U32("TemplateId")
                    });

        public static readonly MessageTemplate OpBase =
            new MessageTemplate(
                Qualify("OpBase"), Namespace,
                new[]
                    {
                        Unicodeopt("Dictionary"),
                        new Group(Qualify("Key"), Namespace, new Field[] {new StaticTemplateReference(NsName)}, true),
                        new StaticTemplateReference(Other)
                    });

        public static readonly MessageTemplate ConstantOp =
            new MessageTemplate(
                Qualify("ConstantOp"), Namespace,
                new Field[] {new StaticTemplateReference(Other)});

        public static readonly MessageTemplate DefaultOp =
            new MessageTemplate(
                Qualify("DefaultOp"), Namespace,
                new Field[] {new StaticTemplateReference(Other)});

        public static readonly MessageTemplate CopyOp =
            new MessageTemplate(
                Qualify("CopyOp"), Namespace,
                new Field[] {new StaticTemplateReference(OpBase)});

        public static readonly MessageTemplate IncrementOp =
            new MessageTemplate(
                Qualify("IncrementOp"), Namespace,
                new Field[] {new StaticTemplateReference(OpBase)});

        public static readonly MessageTemplate DeltaOp =
            new MessageTemplate(
                Qualify("DeltaOp"), Namespace,
                new Field[] {new StaticTemplateReference(OpBase)});

        public static readonly MessageTemplate TailOp =
            new MessageTemplate(
                Qualify("TailOp"), Namespace,
                new Field[] {new StaticTemplateReference(OpBase)});

        public static readonly MessageTemplate StatTempRefInstr =
            new MessageTemplate(
                Qualify("StaticTemplateRefInstr"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(TemplateName),
                        new StaticTemplateReference(Other)
                    });

        public static readonly MessageTemplate DynTempRefInstr =
            new MessageTemplate(
                Qualify("DynamicTemplateRefInstr"), Namespace,
                new Field[] {new StaticTemplateReference(Other)});

        public static readonly MessageTemplate ForeignInstr =
            new MessageTemplate(
                Qualify("ForeignInstr"), Namespace,
                new Field[] {new StaticTemplateReference(Element)});

        public static readonly MessageTemplate UnicodeInstr =
            new MessageTemplate(
                Qualify("UnicodeInstr"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBaseWithLength),
                        new Scalar(Qualify("InitialValue"), FastType.Unicode, Operator.None, null, true)
                    });

        public static readonly MessageTemplate AsciiInstr =
            new MessageTemplate(
                Qualify("AsciiInstr"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBase),
                        new Scalar(Qualify("InitialValue"), FastType.Ascii, Operator.None, null, true)
                    });

        public static readonly MessageTemplate ByteVectorInstr =
            new MessageTemplate(
                Qualify("ByteVectorInstr"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(PrimFieldBaseWithLength),
                        new Scalar(Qualify("InitialValue"), FastType.ByteVector, Operator.None, null, true)
                    });

        public static readonly MessageTemplate TemplateDefinition =
            new MessageTemplate(
                Qualify("TemplateDef"), Namespace,
                new[]
                    {
                        new StaticTemplateReference(TemplateName),
                        Unicodeopt("AuxId"),
                        U32Opt("TemplateId"),
                        new StaticTemplateReference(TypeRef), U32("Reset"),
                        new StaticTemplateReference(Other),
                        new Sequence(Qualify("Instructions"), new Field[] {DynamicTemplateReference.Instance}, false)
                    });

        public static readonly MessageTemplate GroupInstr =
            new MessageTemplate(
                Qualify("GroupInstr"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(FieldBase),
                        new StaticTemplateReference(TypeRef),
                        new Sequence(Qualify("Instructions"),
                                     new Field[] {DynamicTemplateReference.Instance},
                                     false)
                    });

        public static readonly MessageTemplate SequenceInstr =
            new MessageTemplate(
                Qualify("SequenceInstr"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(FieldBase),
                        new StaticTemplateReference(TypeRef),
                        new Group(
                            Qualify("Length"), Namespace,
                            new Field[]
                                {
                                    new Group(Qualify("Name"), Namespace,
                                              new Field[] {new StaticTemplateReference(NsNameWithAuxId)}, true),
                                    new Group(Qualify("Operator"), Namespace,
                                              new Field[] {DynamicTemplateReference.Instance}, true),
                                    new Scalar(Qualify("InitialValue"), FastType.U32, Operator.None, null, true),
                                    new StaticTemplateReference(Other)
                                }, true),
                        new Sequence(Qualify("Instructions"), new Field[] {DynamicTemplateReference.Instance}, false)
                    });

        public static readonly MessageTemplate Text =
            new MessageTemplate(
                Qualify("Text"), Namespace,
                new[]
                    {
                        new Scalar(Qualify("Value"), FastType.Unicode, Operator.None, ScalarValue.Undefined, false)
                    });

        public static readonly MessageTemplate CompDecimalInstr =
            new MessageTemplate(
                Qualify("CompositeDecimalInstr"), Namespace,
                new Field[]
                    {
                        new StaticTemplateReference(FieldBase),
                        new Group(
                            Qualify("Exponent"), Namespace,
                            new Field[]
                                {
                                    new Group(
                                        Qualify("Operator"), Namespace, new Field[] {DynamicTemplateReference.Instance},
                                        false),
                                    new Scalar(
                                        Qualify("InitialValue"), FastType.I32, Operator.None, ScalarValue.Undefined,
                                        true),
                                    new StaticTemplateReference(Other)
                                }, true),
                        new Group(
                            Qualify("Mantissa"), Namespace,
                            new Field[]
                                {
                                    new Group(
                                        Qualify("Operator"), Namespace,
                                        new Field[] {DynamicTemplateReference.Instance}, false),
                                    new Scalar(
                                        Qualify("InitialValue"), FastType.I32, Operator.None, ScalarValue.Undefined,
                                        true),
                                    new StaticTemplateReference(Other)
                                }, true)
                    });

        #endregion

        private static readonly Message Reset = new ResetMessageObj(FastResetTemplate);
        public static readonly Message DynTempRefMessage = new Message(DynTempRefInstr);
        private static readonly Message Close = CreateAlertMessage(DynError.Close);

        private static readonly ITemplateRegistry TemplateRegistry;

        private static readonly IMessageHandler ResetHandler = new ResetMessageHandler();
        private static readonly ISessionMessageHandler AlertHandler = new AlertSessionMessageHandler();

        private static readonly QName ResetProperty = Qualify("reset");

        private static readonly Dictionary<MessageTemplate, ISessionMessageHandler> MessageHandlers =
            new Dictionary<MessageTemplate, ISessionMessageHandler>();

        private readonly ConversionContext _initialContext = CreateInitialContext();

        static SessionControlProtocol11()
        {
            FastResetTemplate.AddAttribute(ResetProperty, "yes");

            TemplateRegistry = new BasicTemplateRegistry
                                   {
                                       {HelloTemplateId, HelloTemplate},
                                       {AlertTemplateId, AlertTemplate},
                                       {ResetTemplateId, FastResetTemplate},
                                       {TemplateDeclId, TemplateDeclaration},
                                       {TemplateDefId, TemplateDefinition},
                                       {Int32InstrId, Int32Instr},
                                       {Uint32InstrId, Uint32Instr},
                                       {Int64InstrId, Int64Instr},
                                       {Uint64InstrId, Uint64Instr},
                                       {DecimalInstrId, DecimalInstr},
                                       {CompDecimalInstrId, CompDecimalInstr},
                                       {AsciiInstrId, AsciiInstr},
                                       {UnicodeInstrId, UnicodeInstr},
                                       {ByteVectorInstrId, ByteVectorInstr},
                                       {StatTempRefInstrId, StatTempRefInstr},
                                       {DynTempRefInstrId, DynTempRefInstr},
                                       {SequenceInstrId, SequenceInstr},
                                       {GroupInstrId, GroupInstr},
                                       {ConstantOpId, ConstantOp},
                                       {DefaultOpId, DefaultOp},
                                       {CopyOpId, CopyOp},
                                       {IncrementOpId, IncrementOp},
                                       {DeltaOpId, DeltaOp},
                                       {TailOpId, TailOp},
                                       {ForeignInstrId, ForeignInstr},
                                       {ElementId, Element},
                                       {TextId, Text}
                                   };

            // TODO: move this into a unit test
            foreach (MessageTemplate t in TemplateRegistry.Templates)
                VerifyChildNamespaces(t);
        }

        public SessionControlProtocol11()
        {
#warning Overrides static field (not thread safe) with each new instance??? Seems very wrong.

            MessageHandlers[AlertTemplate] = AlertHandler;
            MessageHandlers[TemplateDefinition] = new ProtocolDefinitionSessionMessageHandler(this);
            MessageHandlers[TemplateDeclaration] = new ProtocolDeclarationSessionMessageHandler(this);
        }

        public override Message ResetMessage
        {
            get { return Reset; }
        }

        public override Message CloseMessage
        {
            get { return Close; }
        }

        private static void VerifyChildNamespaces(Group value)
        {
            if (value.ChildNamespace != Namespace)
                throw new InvalidOperationException(
                    string.Format(
                        "All MessageTemplate or Group objects must have ChildNamespace set to {0}. Verify {1}.",
                        Namespace, value.Name));

            foreach (Field fld in value.Fields)
            {
                var grp = fld as Group;
                if (grp != null)
                    VerifyChildNamespaces(grp);
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

        protected virtual QName GetQName(Message message)
        {
            string name = message.GetString("Name");
            string ns = message.GetString("Ns");
            return new QName(name, ns);
        }

        public override void ConfigureSession(Session session)
        {
            RegisterSessionTemplates(session.MessageInputStream.TemplateRegistry);
            RegisterSessionTemplates(session.MessageOutputStream.TemplateRegistry);
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

        public override void OnError(Session session, DynError code, string message)
        {
            session.MessageOutputStream.WriteMessage(CreateAlertMessage(code));
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
            var message = new Message(HelloTemplate);
            message.SetString(1, senderName);
            message.SetString(2, SessionConstants.VendorId);
            return message;
        }

        public static Message CreateAlertMessage(DynError code)
        {
            ErrorInfoAttribute attr = code.GetErrorInfo();
            var alert = new Message(AlertTemplate);
            alert.SetInteger(1, (int) attr.Severity);
            alert.SetInteger(2, (int) code);
            alert.SetString(4, attr.Description);
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

        public override bool SupportsTemplateExchange
        {
            get { return true; }
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
            IFieldValue retNs;
            if (templateDef.TryGetValue("Ns", out retNs) && retNs != null)
                tempnamespace = retNs.ToString();
            Field[] fields = GroupConverter.ParseFieldInstructions(templateDef, registry, _initialContext);
            var group = new MessageTemplate(new QName(name, tempnamespace), fields);
            IFieldValue retTypeRef;
            if (templateDef.TryGetValue("TypeRef", out retTypeRef) && retTypeRef != null)
            {
                var typeRef = (GroupValue) retTypeRef;
                string typeRefName = typeRef.GetString("Name");
                string typeRefNs = ""; // context.getNamespace();
                IFieldValue retNs2;
                if (typeRef.TryGetValue("Ns", out retNs2) && retNs2 != null)
                    typeRefNs = retNs2.ToString();
                group.TypeReference = new QName(typeRefName, typeRefNs);
            }
            IFieldValue retAuxId;
            if (templateDef.TryGetValue("AuxId", out retAuxId) && retAuxId != null)
            {
                group.Id = retAuxId.ToString();
            }

            return group;
        }

        private static Field U32(string name)
        {
            return new Scalar(Qualify(name), FastType.U32, Operator.None, null, false);
        }

        private static Field Dict(string name, bool optional, string dictionary)
        {
            var scalar = new Scalar(Qualify(name), FastType.Unicode, Operator.Copy, null, optional)
                             {Dictionary = dictionary};
            return scalar;
        }

        private static QName Qualify(string name)
        {
            return new QName(name, Namespace);
        }

        private static Field Unicodeopt(string name)
        {
            return new Scalar(Qualify(name), FastType.Unicode, Operator.None, null, true);
        }

        private static Field Unicode(string name)
        {
            return new Scalar(Qualify(name), FastType.Unicode, Operator.None, null, false);
        }

        private static Field U32Opt(string name)
        {
            return new Scalar(Qualify(name), FastType.U32, Operator.None, null, true);
        }

        #region Nested type: AlertSessionMessageHandler

        public sealed class AlertSessionMessageHandler : ISessionMessageHandler
        {
            #region ISessionMessageHandler Members

            public void HandleMessage(Session session, Message message)
            {
                var alertCode = (DynError) message.GetInt(2);
                if (alertCode == DynError.Close)
                {
                    session.Close(alertCode);
                }
                else
                {
                    session.ErrorHandler.OnError(null, alertCode, message.GetString(4));
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

        #region Nested type: ProtocolDefinitionSessionMessageHandler

        private sealed class ProtocolDefinitionSessionMessageHandler : ISessionMessageHandler
        {
            private SessionControlProtocol11 _enclosingInstance;

            public ProtocolDefinitionSessionMessageHandler(SessionControlProtocol11 enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }

            #region ISessionMessageHandler Members

            public void HandleMessage(Session session, Message message)
            {
                MessageTemplate template = _enclosingInstance.CreateTemplateFromMessage(
                    message, session.MessageInputStream.TemplateRegistry);
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