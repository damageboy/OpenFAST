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
using OpenFAST.Utility;

namespace OpenFAST.Template
{
    public sealed class BasicTemplateRegistry : AbstractTemplateRegistry
    {
        private readonly Dictionary<QName, MessageTemplate> _defNameMap = new Dictionary<QName, MessageTemplate>();
        private readonly HashSet<MessageTemplate> _defTemplates = new HashSet<MessageTemplate>();
        private readonly Dictionary<int, MessageTemplate> _regIdMap = new Dictionary<int, MessageTemplate>();
        private readonly Dictionary<MessageTemplate, int> _regTemplateMap = new Dictionary<MessageTemplate, int>();

        public override MessageTemplate[] Templates
        {
            get { return Util.ToArray(_regTemplateMap.Keys); }
        }

        public override void Add(int templateId, MessageTemplate template)
        {
            Define(template);
            _regIdMap[templateId] = template;
            _regTemplateMap[template] = templateId;
            NotifyTemplateRegistered(template, templateId);
        }

        public override void Add(int templateId, QName templateName)
        {
            MessageTemplate template;
            if (!_defNameMap.TryGetValue(templateName, out template))
                throw new ArgumentOutOfRangeException("templateName", templateName, "The template is not defined.");

            _regTemplateMap[template] = templateId;
            _regIdMap[templateId] = template;
            NotifyTemplateRegistered(template, templateId);
        }

        public override bool TryAdd(int templateId, QName templateName)
        {
            MessageTemplate template;
            if (!_defNameMap.TryGetValue(templateName, out template))
                return false;

            _regTemplateMap[template] = templateId;
            _regIdMap[templateId] = template;
            NotifyTemplateRegistered(template, templateId);
            return true;
        }

        public override void Define(MessageTemplate template)
        {
            if (!_defTemplates.Contains(template))
            {
                _defNameMap[template.QName] = template;
                _defTemplates.Add(template);
            }
        }

        public override int GetId(QName templateName)
        {
            int id;
            TryGetId(templateName, out id);
            return id;
        }

        public override MessageTemplate this[int templateId]
        {
            get
            {
                MessageTemplate value;
                _regIdMap.TryGetValue(templateId, out value);
                return value;
            }
        }

        public override MessageTemplate this[QName templateName]
        {
            get { return _defNameMap[templateName]; }
        }

        public override int GetId(MessageTemplate template)
        {
            int value;
            if (_regTemplateMap.TryGetValue(template, out value))
                return value;
            return -1;
        }

        public override bool IsDefined(QName templateName)
        {
            return _defNameMap.ContainsKey(templateName);
        }

        public override bool IsRegistered(QName templateName)
        {
            return _defNameMap.ContainsKey(templateName);
        }

        public override bool IsRegistered(int templateId)
        {
            return _regIdMap.ContainsKey(templateId);
        }

        public override bool IsRegistered(MessageTemplate template)
        {
            return _regTemplateMap.ContainsKey(template);
        }

        public override bool TryGetTemplate(int id, out MessageTemplate template)
        {
            return _regIdMap.TryGetValue(id, out template);
        }

        public override bool TryGetTemplate(QName templateName, out MessageTemplate template)
        {
            return _defNameMap.TryGetValue(templateName, out template);
        }

        public override bool TryGetId(QName templateName, out int templateId)
        {
            MessageTemplate tmpl;
            if (_defNameMap.TryGetValue(templateName, out tmpl) && _regTemplateMap.TryGetValue(tmpl, out templateId))
                return true;
            templateId = -1;
            return false;
        }

        public override bool TryGetId(MessageTemplate template, out int templateId)
        {
            if (_regTemplateMap.TryGetValue(template, out templateId))
                return true;
            templateId = -1;
            return false;
        }

        public override void Remove(QName templateName)
        {
            object tempObject = _defNameMap[templateName];
            _defNameMap.Remove(templateName);
            var template = (MessageTemplate) tempObject;
            int id = _regTemplateMap[template];
            _regTemplateMap.Remove(template);
            _regIdMap.Remove(id);
            _defTemplates.Remove(template);
        }

        //[Obsolete("dont call this method")]
        public override void Remove(MessageTemplate template)
        {
            int id = _regTemplateMap[template];
            _regTemplateMap.Remove(template);
            _defNameMap.Remove(template.QName);
            //wrong approach, what if the hashcode is matched for the string.... because its an algo in QNameclass GetHashCode() dont use it.
            _regIdMap.Remove(id);
        }

        public override void Remove(int templateId)
        {
            MessageTemplate template = _regIdMap[templateId];
            _regIdMap.Remove(templateId);
            _regTemplateMap.Remove(template);
            _defNameMap.Remove(template.QName);
        }

        public override void RegisterAll(ITemplateRegistry registry)
        {
            if (registry == null)
                return;

            MessageTemplate[] templatesp = registry.Templates;
            if (templatesp != null)
                foreach (MessageTemplate t in templatesp)
                    Add(registry.GetId(t), t);
        }

        public override ICollection<QName> Names()
        {
            return _defNameMap.Keys;
        }

        public override IEnumerator<KeyValuePair<int, MessageTemplate>> GetEnumerator()
        {
            return _regIdMap.GetEnumerator();
        }
    }
}