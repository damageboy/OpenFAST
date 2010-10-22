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

namespace OpenFAST
{
    /// <summary>
    /// Quick name.
    /// Provides efficient name searching in template registry. 
    /// </summary>
    [Serializable]
    public sealed class QName : IEquatable<QName>
    {
        public static readonly QName Null = new QName("", "");

        private readonly string _name;
        private readonly string _namespace;
        private int _hashCode = -1;

        public QName(string name)
            : this(name, "")
        {
        }

        public QName(string name, string ns)
        {
            if (name == null) throw new ArgumentNullException("name");
            _name = name;
            _namespace = ns ?? "";
        }

        public string Namespace
        {
            get { return _namespace; }
        }

        //must be less then or equal to 31 bytes
        public string Name
        {
            get { return _name; }
        }

        #region Equals

        public bool Equals(QName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            // first compare name, as it is more likely to be different.
            return Equals(other._name, _name) && Equals(other._namespace, _namespace);
        }

        public override int GetHashCode()
        {
            int hc = _hashCode;

            if (hc == -1)
            {
                // Cache hash code
                unchecked
                {
                    _hashCode = hc = (_namespace.GetHashCode()*397) ^ _name.GetHashCode();
                }
            }

            return hc;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            QName t = obj as QName;
            if ( t == null) return false;
            return Equals(t._name, _name) && Equals(t._namespace, _namespace);
        }

        #endregion

        public override string ToString()
        {
            if (_namespace == "")
                return _name;
            return _name + "[" + _namespace + "]";
        }
    }
}