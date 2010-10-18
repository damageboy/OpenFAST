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
using System.Text;

namespace OpenFAST
{
    [Serializable]
    public sealed class ByteVectorValue : ScalarValue
    {
        private readonly byte[] _value;

        public ByteVectorValue(byte[] value)
        {
            _value = value;
        }

        public override byte[] Bytes
        {
            get { return _value; }
        }

        public byte[] Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            var builder = new StringBuilder(_value.Length*2);
            for (int i = 0; i < _value.Length; i++)
            {
                string hex = Convert.ToString(_value[i], 16);
                if (hex.Length == 1)
                    builder.Append('0');
                builder.Append(hex);
            }
            return builder.ToString();
        }

        public override bool Equals(Object obj)
        {
            if(ReferenceEquals(obj,null))
                return false;
            if (!(obj is ByteVectorValue))
            {
                return false;
            }

            return Equals((ByteVectorValue) obj);
        }

        public bool Equals(ByteVectorValue other)
        {
            if (_value.Length != other._value.Length)
            {
                return false;
            }

            for (int i = 0; i < _value.Length; i++)
                if (_value[i] != other._value[i])
                {
                    return false;
                }

            return true;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}