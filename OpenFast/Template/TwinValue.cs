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

namespace OpenFAST.Template
{
    [Serializable]
    public class TwinValue : ScalarValue, IEquatable<TwinValue>
    {
        public ScalarValue First;
        public ScalarValue Second;

        public TwinValue(ScalarValue first, ScalarValue second)
        {
            First = first;
            Second = second;
        }

        public override string ToString()
        {
            return First + ", " + Second;
        }

        #region Equals

        public bool Equals(TwinValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.First, First) && Equals(other.Second, Second);
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
                return (First.GetHashCode()*397) ^ Second.GetHashCode();
            }
        }

        #endregion
    }
}