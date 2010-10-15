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

        private QName _currentApplicationType;
        private ITrace _encodeTraceInternal;
        private IErrorHandler _errorHandler = ErrorHandler_Fields.DEFAULT;
        private ITemplateRegistry _templateRegistry = new BasicTemplateRegistry();

        public Context()
        {
            _dictionaries["global"] = new GlobalDictionary();
            _dictionaries["template"] = new TemplateDictionary();
            _dictionaries["type"] = new ApplicationTypeDictionary();
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

        public ITrace DecodeTrace { get; set; }

        public int GetTemplateId(MessageTemplate template)
        {
            if (!_templateRegistry.IsRegistered(template))
            {
                _errorHandler.Error(FastConstants.D9_TEMPLATE_NOT_REGISTERED,
                                   "The template " + template + " has not been registered.");
                return 0;
            }
            return _templateRegistry.GetId(template);
        }

        public MessageTemplate GetTemplate(int templateId)
        {
            if (!_templateRegistry.IsRegistered(templateId))
            {
                _errorHandler.Error(FastConstants.D9_TEMPLATE_NOT_REGISTERED,
                                   "The template with id " + templateId + " has not been registered.");
                return null;
            }
            return _templateRegistry[templateId];
        }

        public void RegisterTemplate(int templateId, MessageTemplate template)
        {
            _templateRegistry.Register(templateId, template);
        }

        public ScalarValue Lookup(string dictionary, Group group, QName key)
        {
            if (group.HasTypeReference())
                _currentApplicationType = group.TypeReference;
            return GetDictionary(dictionary).Lookup(group, key, _currentApplicationType);
        }

        private IDictionary GetDictionary(string dictionary)
        {
            IDictionary value;
            if (!_dictionaries.TryGetValue(dictionary, out value))
                _dictionaries[dictionary] = value = new GlobalDictionary();
            return value;
        }

        public void Store(string dictionary, Group group, QName key, ScalarValue valueToEncode)
        {
            if (group.HasTypeReference())
                _currentApplicationType = group.TypeReference;
            GetDictionary(dictionary).Store(group, _currentApplicationType, key, valueToEncode);
        }

        public void Reset()
        {
            foreach (IDictionary dict in _dictionaries.Values)
            {
                dict.Reset();
            }
        }

        public void NewMessage(MessageTemplate template)
        {
            _currentApplicationType = (template.HasTypeReference())
                                         ? template.TypeReference
                                         : FastConstants.ANY_TYPE;
        }

        public void StartTrace()
        {
            SetEncodeTrace(new BasicEncodeTrace());
            DecodeTrace = new BasicDecodeTrace();
        }

        public void SetEncodeTrace(BasicEncodeTrace encodeTrace)
        {
            _encodeTraceInternal = encodeTrace;
        }

        public ITrace GetEncodeTrace()
        {
            return _encodeTraceInternal;
        }
    }
}