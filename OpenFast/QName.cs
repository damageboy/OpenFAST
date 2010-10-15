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
        public static readonly QName NULL = new QName("", "");

        private readonly string name;
        private readonly string namespace_Renamed;

        public QName(string name) : this(name, "")
        {
        }

        public QName(string name, string namespace_Renamed)
        {
            if (name == null)
                throw new NullReferenceException();
            this.name = name;
            this.namespace_Renamed = namespace_Renamed ?? "";
        }

        public string Namespace
        {
            get { return namespace_Renamed; }
        }

        //must be less then or equal to 31 bytes
        public string Name
        {
            get { return name; }
        }

        #region IEquatable<QName> Members

        public bool Equals(QName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.namespace_Renamed, namespace_Renamed) && Equals(other.name, name);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (QName)) return false;
            return Equals((QName) obj);
        }

        public override int GetHashCode() //efficient algo to quick search
        {
            unchecked
            {
                return (namespace_Renamed.GetHashCode()*397) ^ name.GetHashCode();
            }
        }

        public override string ToString()
        {
            if (namespace_Renamed == "")
                return name;
            return name + "[" + namespace_Renamed + "]";
        }
    }
}