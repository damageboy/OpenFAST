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
using System.Linq;
using System.Text;
using OpenFAST.Template;
using OpenFAST.Utility;

namespace OpenFAST
{
    public sealed class ApplicationTypeDictionary : IDictionary
    {
        private readonly Dictionary<Tuple<QName, QName>, ScalarValue> _dictionary =
            new Dictionary<Tuple<QName, QName>, ScalarValue>();

        #region IDictionary Members

        public ScalarValue Lookup(Group template, QName key, QName applicationType)
        {
            ScalarValue value;
            if (_dictionary.TryGetValue(Tuple.Create(template.TypeReference, key), out value))
                return value;

            return ScalarValue.Undefined;
        }

        public void Reset()
        {
            _dictionary.Clear();
        }

        public void Store(Group group, QName applicationType, QName key, ScalarValue value)
        {
            _dictionary[Tuple.Create(group.TypeReference, key)] = value;
        }

        #endregion

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var kv in _dictionary.GroupBy(i => i.Key.Item1))
            {
                builder.Append("Dictionary: Type=").Append(kv.Key);

                foreach (var kv2 in kv)
                    builder.Append(kv2.Key.Item2).Append("=").Append(kv2.Value).Append("\n");
            }

            return builder.ToString();
        }
    }
}