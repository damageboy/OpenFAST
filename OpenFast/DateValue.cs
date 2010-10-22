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
    [Serializable]
    public sealed class DateValue : ScalarValue, IEquatable<DateValue>
    {
        private readonly DateTime _value;

        public DateValue(DateTime date)
        {
            _value = date;
        }

        public DateTime Value
        {
            get { return _value; }
        }

        public override long ToLong()
        {
            return _value.Ticks;
        }

        public override string ToString()
        {
            return _value.ToString("r");
        }

        #region Equals (optimized for empty parent class)

        public bool Equals(DateValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._value.Equals(_value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            DateValue t = obj as DateValue;
            if (t==null) return false;
            return t._value.Equals(_value);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        #endregion
    }
}