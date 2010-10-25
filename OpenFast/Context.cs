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
using System.Collections.Generic;
using OpenFAST.Debug;
using OpenFAST.Error;
using OpenFAST.Template;

namespace OpenFAST
{
    public sealed class Context
    {
        private readonly Dictionary<string, IDictionary> _dictionaries =
            new Dictionary<string, IDictionary>();

        private readonly GlobalDictionary _globalDictionary = new GlobalDictionary();
        private readonly TemplateDictionary _templateDictionary = new TemplateDictionary();
        private readonly ApplicationTypeDictionary _applicationTypeDictionary = new ApplicationTypeDictionary();

        private QName _currentApplicationType;
        private IErrorHandler _errorHandler = ErrorHandlerFields.Default;
        private ITemplateRegistry _templateRegistry = new BasicTemplateRegistry();

        public Context()
        {
            _dictionaries[DictionaryFields.Global] = _globalDictionary;
            _dictionaries[DictionaryFields.Template] = _templateDictionary;
            _dictionaries[DictionaryFields.Type] = _applicationTypeDictionary;
        }

        public int LastTemplateId { get; set; }

        public IErrorHandler ErrorHandler
        {
            set { _errorHandler = value; }
        }

        public QName CurrentApplicationType
        {
            set { _currentApplicationType = value; }
        }

        public ITemplateRegistry TemplateRegistry
        {
            get { return _templateRegistry; }
            set { _templateRegistry = value; }
        }

        public bool TraceEnabled { get; set; }
        public ITrace EncodeTrace { get; set; }
        public ITrace DecodeTrace { get; set; }

        public int GetTemplateId(MessageTemplate template)
        {
            int id;
            if (_templateRegistry.TryGetId(template, out id))
                return id;

            _errorHandler.OnError(null, DynError.TemplateNotRegistered, "The template {0} has not been registered.",
                                  template);
            return 0;
        }

        public MessageTemplate GetTemplate(int templateId)
        {
            MessageTemplate template;
            if (_templateRegistry.TryGetTemplate(templateId, out template))
                return template;
            _errorHandler.OnError(null, DynError.TemplateNotRegistered, "The template with id {0} has not been registered.", templateId);
            return null;
        }

        public void RegisterTemplate(int templateId, MessageTemplate template)
        {
            _templateRegistry.Register(templateId, template);
        }

        public ScalarValue Lookup(string dictionary, Group group, QName key)
        {
            return Lookup(GetDictionary(dictionary), group, key);
        }

        public ScalarValue Lookup(IDictionary dictionary, Group group, QName key)
        {
            if (group.HasTypeReference)
                _currentApplicationType = group.TypeReference;
            return dictionary.Lookup(group, key, _currentApplicationType);
        }

        public void Store(string dictionary, Group group, QName key, ScalarValue valueToEncode)
        {
            Store(GetDictionary(dictionary), group, key, valueToEncode);
        }

        public void Store(IDictionary dictionary, Group group, QName key, ScalarValue valueToEncode)
        {
            if (group.HasTypeReference)
                _currentApplicationType = group.TypeReference;
            dictionary.Store(group, _currentApplicationType, key, valueToEncode);
        }

        internal IDictionary GetDictionary(string dictionary)
        {
            // Speed optimization for the well-known dictionaries in case the same constant was used.
            // Make sure the string is interned usidng DictionaryFields.InternDictionaryName()
            if (ReferenceEquals(dictionary, DictionaryFields.Global)) return _globalDictionary;
            if (ReferenceEquals(dictionary, DictionaryFields.Template)) return _templateDictionary;
            if (ReferenceEquals(dictionary, DictionaryFields.Type)) return _applicationTypeDictionary;

            IDictionary value;
            if (!_dictionaries.TryGetValue(dictionary, out value))
                _dictionaries[dictionary] = value = new GlobalDictionary();
            return value;
        }

        public void Reset()
        {
            foreach (IDictionary dict in _dictionaries.Values)
                dict.Reset();
        }

        public void NewMessage(MessageTemplate template)
        {
            _currentApplicationType = (template.HasTypeReference)
                                          ? template.TypeReference
                                          : FastConstants.AnyType;
        }

        public void StartTrace()
        {
            EncodeTrace = new BasicEncodeTrace();
            DecodeTrace = new BasicDecodeTrace();
        }
    }
}