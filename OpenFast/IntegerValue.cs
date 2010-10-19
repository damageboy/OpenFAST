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
using OpenFAST.Error;
using OpenFAST.Template;

namespace OpenFAST
{
    [Serializable]
    public sealed class IntegerValue : NumericValue
    {
        private readonly int _value;

        public IntegerValue(int value)
        {
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is NumericValue))
            {
                return false;
            }

            return Equals((ScalarValue) obj);
        }

        private bool Equals(ScalarValue otherValue)
        {
            return _value == otherValue.ToLong();
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public override bool EqualsValue(string defaultValue)
        {
            return Int32.Parse(defaultValue) == _value;
        }

        public override NumericValue Increment()
        {
            return new IntegerValue(_value + 1);
        }

        public override NumericValue Decrement()
        {
            return new IntegerValue(_value - 1);
        }

        public override NumericValue Subtract(NumericValue subend)
        {
            if (subend is LongValue)
            {
                return new LongValue(_value - subend.ToLong());
            }

            return new IntegerValue(_value - subend.ToInt());
        }

        public override NumericValue Add(NumericValue addend)
        {
            if (addend is LongValue)
            {
                return addend.Add(this);
            }

            return new IntegerValue(_value + addend.ToInt());
        }

        public string Serialize()
        {
            return Convert.ToString(_value);
        }

        public override bool Equals(int valueRenamed)
        {
            return valueRenamed == _value;
        }

        public override long ToLong()
        {
            return _value;
        }

        public override int ToInt()
        {
            return _value;
        }

        public override string ToString()
        {
            return Convert.ToString(_value);
        }

        public override byte ToByte()
        {
            if (_value > SByte.MaxValue || _value < SByte.MinValue)
                Global.HandleError(FastConstants.R4_NUMERIC_VALUE_TOO_LARGE,
                                   "The value \"" + _value + "\" is too large for a byte.");
            return (byte) _value;
        }

        public override short ToShort()
        {
            if (_value > Int16.MaxValue || _value < Int16.MinValue)
                Global.HandleError(FastConstants.R4_NUMERIC_VALUE_TOO_LARGE,
                                   "The value \"" + _value + "\" is too large for a short.");
            return (short) _value;
        }

        public override double ToDouble()
        {
            return _value;
        }

        public override Decimal ToBigDecimal()
        {
            return new Decimal(_value);
        }
    }
}