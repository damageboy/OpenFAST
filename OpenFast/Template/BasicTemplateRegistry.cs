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
using System.Collections.Generic;
using OpenFAST.util;

namespace OpenFAST.Template
{
    public sealed class BasicTemplateRegistry : AbstractTemplateRegistry
    {
        private readonly Dictionary<int, MessageTemplate> _idMap = new Dictionary<int, MessageTemplate>();
        private readonly Dictionary<QName, MessageTemplate> _nameMap = new Dictionary<QName, MessageTemplate>();
        private readonly Dictionary<MessageTemplate, int> _templateMap = new Dictionary<MessageTemplate, int>();

#warning todo: don't think its needed, unless the order of additions must be preserved
        private readonly List<MessageTemplate> _templates = new List<MessageTemplate>();

        public override MessageTemplate[] Templates
        {
            get { return Util.ToArray(_templateMap.Keys); }
        }

        public override void Register(int id, MessageTemplate template)
        {
            Define(template);
            int tid = id;
            _idMap[tid] = template;
            _templateMap[template] = tid;
            NotifyTemplateRegistered(template, id);
        }

        public override void Register(int id, QName name)
        {
            if (!_nameMap.ContainsKey(name))
                throw new ArgumentOutOfRangeException("name", name, "The template is not defined.");
            int tid = id;
            MessageTemplate template = _nameMap[name];
            _templateMap[template] = tid;
            _idMap[tid] = template;
            NotifyTemplateRegistered(template, id);
        }

        public override void Define(MessageTemplate template)
        {
            if (!_templates.Contains(template))
            {
                _nameMap[template.QName] = template;
                _templates.Add(template);
            }
        }

        public override int GetId(QName name)
        {
            MessageTemplate template = _nameMap[name];
            if (template == null || !_templateMap.ContainsKey(template))
                return - 1;
            return _templateMap[template];
        }

        public override MessageTemplate this[int templateId]
        {
            get { return _idMap[templateId]; }
        }

        public override MessageTemplate this[QName name]
        {
            get { return _nameMap[name]; }
        }

        public override int GetId(MessageTemplate template)
        {
            if (!IsRegistered(template))
                return - 1;
            return _templateMap[template];
        }

        public override bool IsRegistered(QName name)
        {
            return _nameMap.ContainsKey(name);
        }

        public override bool IsRegistered(int templateId)
        {
            return _idMap.ContainsKey(templateId);
        }

        public override bool IsRegistered(MessageTemplate template)
        {
            return _templateMap.ContainsKey(template);
        }

        public override bool IsDefined(QName name)
        {
            return _nameMap.ContainsKey(name);
        }

        public override void Remove(QName name)
        {
            object tempObject = _nameMap[name];
            _nameMap.Remove(name);
            var template = (MessageTemplate) tempObject;
            int id = _templateMap[template];
            _templateMap.Remove(template);
            _idMap.Remove(id);
            _templates.Remove(template);
        }

        //[Obsolete("dont call this method")]
        public override void Remove(MessageTemplate template)
        {
            int id = _templateMap[template];
            _templateMap.Remove(template);
            _nameMap.Remove(template.QName);
            //wrong approach, what if the hashcode is matched for the string.... because its an algo in QNameclass GetHashCode() dont use it.
            _idMap.Remove(id);
        }

        public override void Remove(int id)
        {
            MessageTemplate template = _idMap[id];
            _idMap.Remove(id);
            _templateMap.Remove(template);
            _nameMap.Remove(template.QName);
        }

        public override void RegisterAll(ITemplateRegistry registry)
        {
            if (registry == null) return;
            MessageTemplate[] templatesp = registry.Templates;
            if (templatesp == null) return;
            for (int i = 0; i < templatesp.Length; i++)
            {
                Register(registry.GetId(templatesp[i]), templatesp[i]);
            }
        }

        public override ICollection<QName> Names()
        {
            return _nameMap.Keys;
        }
    }
}