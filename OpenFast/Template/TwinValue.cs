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

namespace OpenFAST.Template
{
    [Serializable]
    public sealed class TwinValue : ScalarValue, IEquatable<TwinValue>
    {
        private readonly ScalarValue _first;
        private readonly ScalarValue _second;

        public TwinValue(ScalarValue first, ScalarValue second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            _first = first;
            _second = second;
        }

        public ScalarValue First
        {
            get { return _first; }
        }

        public ScalarValue Second
        {
            get { return _second; }
        }

        public override string ToString()
        {
            return _first + ", " + _second;
        }

        #region Equals (optimized for empty parent class)

        public bool Equals(TwinValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._first, _first) && Equals(other._second, _second);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TwinValue)) return false;
            return Equals((TwinValue) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_first.GetHashCode()*397) ^ _second.GetHashCode();
            }
        }

        #endregion
    }
}