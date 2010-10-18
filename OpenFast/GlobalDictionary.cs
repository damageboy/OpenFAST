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
using System.Text;
using OpenFAST.Template;

namespace OpenFAST
{
    public sealed class GlobalDictionary : IDictionary
    {
        private readonly Dictionary<QName, ScalarValue> _table =
            new Dictionary<QName, ScalarValue>();

        #region Dictionary Members

        public ScalarValue Lookup(Group template, QName key, QName applicationType)
        {
            ScalarValue value;
            return _table.TryGetValue(key, out value) ? value : ScalarValue.Undefined;
        }

        public void Store(Group group, QName applicationType, QName key, ScalarValue value)
        {
            _table[key] = value;
        }

        public void Reset()
        {
            _table.Clear();
        }

        #endregion

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (QName key in _table.Keys)
            {
                builder.Append("Dictionary: Global");
                builder.Append(key).Append("=").Append(_table[key]).Append("\n");
            }
            return builder.ToString();
        }
    }
}