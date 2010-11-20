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
using System.Collections;
using System.Collections.Generic;

namespace OpenFAST.Template
{
    public abstract class AbstractTemplateRegistry : ITemplateRegistry
    {
        #region ITemplateRegistry Members

        public abstract MessageTemplate[] Templates { get; }

        public virtual MessageTemplate this[string name]
        {
            get { return this[new QName(name)]; }
        }

        public virtual int GetId(string name)
        {
            return GetId(new QName(name));
        }

        public virtual bool IsDefined(string name)
        {
            return IsDefined(new QName(name));
        }

        public abstract bool IsDefined(QName templateName);

        public virtual bool IsRegistered(string name)
        {
            return IsRegistered(new QName(name));
        }

        public virtual void Add(int templateId, string name)
        {
            Add(templateId, new QName(name));
        }

        public virtual void Remove(string name)
        {
            Remove(new QName(name));
        }

        public abstract bool TryGetTemplate(int id, out MessageTemplate template);

        public bool TryGetTemplate(string name, out MessageTemplate template)
        {
            return TryGetTemplate(new QName(name), out template);
        }

        public bool TryGetId(string name, out int templateId)
        {
            return TryGetId(new QName(name), out templateId);
        }

        public abstract void Define(MessageTemplate template);
        public abstract MessageTemplate this[int templateId] { get; }
        public abstract int GetId(QName templateName);
        public abstract void Remove(QName templateName);
        public abstract MessageTemplate this[QName templateName] { get; }
        public abstract bool IsRegistered(int templateId);
        public abstract void Add(int templateId, MessageTemplate template);
        public abstract void Remove(MessageTemplate template);
        public abstract bool IsRegistered(MessageTemplate template);
        public abstract bool IsRegistered(QName templateName);
        public abstract void RegisterAll(ITemplateRegistry registry);
        public abstract void Remove(int templateId);
        public abstract int GetId(MessageTemplate template);
        public abstract void Add(int templateId, QName templateName);
        public abstract bool TryAdd(int templateId, QName templateName);
        public abstract bool TryGetTemplate(QName templateName, out MessageTemplate template);
        public abstract bool TryGetId(QName templateName, out int templateId);
        public abstract bool TryGetId(MessageTemplate template, out int templateId);
        public abstract ICollection<QName> Names();
        public event TemplateNotificationDelegate OnTemplateRegistered;

        #endregion

        protected void NotifyTemplateRegistered(MessageTemplate template, int templateId)
        {
            var evt = OnTemplateRegistered;
            if (evt != null)
                evt(this, template, templateId);
        }

        public abstract IEnumerator<KeyValuePair<int, MessageTemplate>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}