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
using System.IO;
using System.Xml;
using OpenFAST.Error;
using OpenFAST.Template.Types;

namespace OpenFAST.Template.Loader
{
    public sealed class XmlMessageTemplateLoader : IMessageTemplateLoader
    {
        internal const string TemplateDefinitionNs = "http://www.fixprotocol.org/ns/fast/td/1.1";

        internal static readonly StaticError IoError = StaticError.IoError;
        internal static readonly StaticError XmlParsingError = StaticError.XmlParsingError;
        internal static readonly StaticError InvalidType = StaticError.InvalidType;

        private readonly ParsingContext _initialContext;

        private bool _loadTemplateIdFromAuxId;

        public XmlMessageTemplateLoader()
        {
            _initialContext = CreateInitialContext();
        }

        public bool LoadTemplateIdFromAuxId
        {
            get { return _loadTemplateIdFromAuxId; }
            set { _loadTemplateIdFromAuxId = value; }
        }

        #region IMessageTemplateLoader Members

        public ITemplateRegistry TemplateRegistry
        {
            get { return _initialContext.TemplateRegistry; }
            set { _initialContext.TemplateRegistry = value; }
        }

        public MessageTemplate[] Load(Stream source)
        {
            XmlDocument document = ParseXml(source);

            if (document == null)
                return new MessageTemplate[0];

            XmlElement root = document.DocumentElement;

            var templateParser = new TemplateParser(_loadTemplateIdFromAuxId);
            if (root != null)
            {
                if (root.Name.Equals("template"))
                {
                    return new[] {(MessageTemplate) templateParser.Parse(root, _initialContext)};
                }
                if (root.Name.Equals("templates"))
                {
                    var context = new ParsingContext(root, _initialContext);

                    XmlNodeList templateTags = root.GetElementsByTagName("template");
                    var templates = new MessageTemplate[templateTags.Count];
                    for (int i = 0; i < templateTags.Count; i++)
                    {
                        var templateTag = (XmlElement) templateTags.Item(i);
                        templates[i] = (MessageTemplate) templateParser.Parse(templateTag, context);
                    }
                    return templates;
                }
                _initialContext.ErrorHandler.OnError(
                    null, StaticError.InvalidXml, "Invalid root node {0}, 'template' or 'templates' expected.",
                    root.Name);
            }
            return new MessageTemplate[] {};
        }

        #endregion

        public IErrorHandler ErrorHandler
        {
            get { return _initialContext.ErrorHandler; }
            set { _initialContext.ErrorHandler = value; }
        }

        public Dictionary<string, FastType> TypeMap
        {
            get { return _initialContext.TypeMap; }
            set { _initialContext.TypeMap = value; }
        }

        public static ParsingContext CreateInitialContext()
        {
            var initialContext = new ParsingContext
                                     {
                                         ErrorHandler = ErrorHandlerFields.Default,
                                         TemplateRegistry = new BasicTemplateRegistry(),
                                         TypeMap = FastType.RegisteredTypeMap,
                                         FieldParsers = new List<IFieldParser>()
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

        public void AddFieldParser(IFieldParser fieldParser)
        {
            _initialContext.FieldParsers.Add(fieldParser);
        }

        private XmlDocument ParseXml(Stream templateStream)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(templateStream);
                return doc;
            }
            catch (IOException e)
            {
                _initialContext.ErrorHandler.OnError(e, IoError, "Error occurred while trying to read xml template: {0}",
                                                     e.Message);
            }
            catch (Exception e)
            {
                _initialContext.ErrorHandler.OnError(e, XmlParsingError,
                                                     "Error occurred while parsing xml template: {0}", e.Message);
            }

            return null;
        }
    }
}