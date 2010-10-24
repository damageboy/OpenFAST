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
    public sealed class LongValue : NumericValue, IEquatable<LongValue>
    {
        private readonly long _value;

        public LongValue(long value)
        {
            _value = value;
        }

        public long Value
        {
            get { return _value; }
        }

        #region Equals (optimized for empty parent class)

        public bool Equals(LongValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._value == _value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var t = obj as LongValue;
            if (t==null) return false;
            return t._value == _value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

#warning This class used to perform Equals with any NumericValue: _value == otherValue.ToLong(), which is not correct. Should it be equatable to any other NumericValue types?

        #endregion

        public override bool EqualsValue(string defaultValue)
        {
            return Int32.Parse(defaultValue) == _value;
        }

        public override NumericValue Increment()
        {
            return new LongValue(_value + 1);
        }

        public override NumericValue Decrement()
        {
            return new LongValue(_value - 1);
        }

        public override string ToString()
        {
            return Convert.ToString(_value);
        }

        public override NumericValue Subtract(NumericValue subend)
        {
            return new LongValue(_value - subend.ToLong());
        }

        public override NumericValue Add(NumericValue addend)
        {
            return new LongValue(_value + addend.ToLong());
        }

        public string Serialize()
        {
            return Convert.ToString(_value);
        }

        public override bool EqualsInt(int value)
        {
            return value == _value;
        }

        public override long ToLong()
        {
            return _value;
        }

        public override int ToInt()
        {
            return (int) _value;
        }
    }
}