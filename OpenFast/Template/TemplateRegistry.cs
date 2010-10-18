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

namespace OpenFAST.Template
{
    public struct TemplateRegistryFields
    {
        public static readonly ITemplateRegistry Null;

        static TemplateRegistryFields()
        {
            Null = new NullTemplateRegistry();
        }
    }

    public interface ITemplateRegistry
    {
        MessageTemplate[] Templates { get; }
        MessageTemplate this[int id] { get; }
        MessageTemplate this[string name] { get; }
        MessageTemplate this[QName templateName] { get; }

        void RegisterAll(ITemplateRegistry registry);
        void Register(int id, MessageTemplate template);
        void Register(int id, string name);
        void Register(int id, QName templateName);

        void Define(MessageTemplate template);

        void Remove(string name);
        void Remove(QName templateName);
        void Remove(MessageTemplate template);
        void Remove(int id);

        int GetId(string name);
        int GetId(QName templateName);
        int GetId(MessageTemplate template);

        bool IsDefined(string templateName);
        bool IsDefined(QName templateName);

        bool IsRegistered(string name);
        bool IsRegistered(QName templateName);
        bool IsRegistered(int id);
        bool IsRegistered(MessageTemplate template);
        
        bool TryGetValue(string name, out MessageTemplate template);
        bool TryGetValue(QName templateName, out MessageTemplate template);

        bool TryGetId(string name, out int id);
        bool TryGetId(QName templateName, out int id);
        bool TryGetId(MessageTemplate template, out int id);

        void AddTemplateRegisteredListener(ITemplateRegisteredListener templateRegisteredListener);
        void RemoveTemplateRegisteredListener(ITemplateRegisteredListener templateRegisteredListener);

        ICollection<QName> Names();
    }
}