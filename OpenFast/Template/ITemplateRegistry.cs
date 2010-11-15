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

namespace OpenFAST.Template
{
    public static class TemplateRegistryFields
    {
        public static readonly ITemplateRegistry Null = new NullTemplateRegistry();
    }

    public interface ITemplateRegistry
    {
        MessageTemplate[] Templates { get; }

        MessageTemplate GetTemplate(int id);
        MessageTemplate GetTemplate(string name);
        MessageTemplate GetTemplate(QName templateName);

        void RegisterAll(ITemplateRegistry registry);

        void Register(int templateId, MessageTemplate template);

        [Obsolete]
        void Register(int templateId, string name);

        [Obsolete]
        void Register(int templateId, QName templateName);

        bool TryRegister(int templateId, QName templateName);

        void Define(MessageTemplate template);

        void Remove(string name);
        void Remove(QName templateName);
        void Remove(MessageTemplate template);
        void Remove(int templateId);

        [Obsolete]
        int GetId(string name);

        [Obsolete]
        int GetId(QName templateName);

        int GetId(MessageTemplate template);

        [Obsolete]
        bool IsDefined(string templateName);

        [Obsolete]
        bool IsDefined(QName templateName);

        [Obsolete]
        bool IsRegistered(string name);

        [Obsolete]
        bool IsRegistered(QName templateName);

        [Obsolete]
        bool IsRegistered(int id);

        bool IsRegistered(MessageTemplate template);

        bool TryGetTemplate(int id, out MessageTemplate template);
        bool TryGetTemplate(string name, out MessageTemplate template);
        bool TryGetTemplate(QName templateName, out MessageTemplate template);

        bool TryGetId(string name, out int templateId);
        bool TryGetId(QName templateName, out int templateId);
        bool TryGetId(MessageTemplate template, out int templateId);

        void AddTemplateRegisteredListener(ITemplateRegisteredListener templateRegisteredListener);
        void RemoveTemplateRegisteredListener(ITemplateRegisteredListener templateRegisteredListener);

        ICollection<QName> Names();
    }
}