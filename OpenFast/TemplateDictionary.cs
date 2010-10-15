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
using System.Collections.Generic;
using System.Text;
using OpenFAST.Template;

namespace OpenFAST
{
    public sealed class TemplateDictionary : IDictionary
    {
        private readonly Dictionary<Group, Dictionary<QName, ScalarValue>> _table =
            new Dictionary<Group, Dictionary<QName, ScalarValue>>();

        #region Dictionary Members

        public ScalarValue Lookup(Group template, QName key, QName applicationType)
        {
            if (_table.ContainsKey(template))
            {
                return (_table[template]).ContainsKey(key) ? _table[template][key] : ScalarValue.UNDEFINED;
            }
            return ScalarValue.UNDEFINED;
        }

        public void Reset()
        {
            _table.Clear();
        }

        public void Store(Group group, QName applicationType, QName key, ScalarValue valueToEncode)
        {
            if (!_table.ContainsKey(group))
            {
                _table[group] = new Dictionary<QName, ScalarValue>();
            }

            _table[group][key] = valueToEncode;
        }

        #endregion

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (Group template in _table.Keys)
            {
                builder.Append("Dictionary: Template=" + template);
                var tmplMap = _table[template];

                foreach (var kv in tmplMap)
                    builder.Append(kv.Key).Append("=").Append(kv.Value).Append("\n");
            }
            return builder.ToString();
        }
    }
}