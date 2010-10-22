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
using OpenFAST.Utility;

namespace OpenFAST
{
    public sealed class BitVector : IEquatable<BitVector>
    {
        private const int ValueBitsSet = 0x7F;
        private const int StopBit = 0x80;

        private readonly byte[] _bytes;

        public BitVector(int size) : this(new byte[((size - 1)/7) + 1])
        {
        }

        public BitVector(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            _bytes = bytes;
            bytes[bytes.Length - 1] |= StopBit;
        }

        public byte[] Bytes
        {
            get { return _bytes; }
        }

        public byte[] TruncatedBytes
        {
            get
            {
                int index = _bytes.Length - 1;

                for (; index > 0 && (_bytes[index] & ValueBitsSet) == 0; index--)
                {
                }

                if (index == (_bytes.Length - 1))
                    return _bytes;

                var truncated = new byte[index + 1];
                Array.Copy(_bytes, truncated, index + 1);

                truncated[truncated.Length - 1] |= StopBit;

                return truncated;
            }
        }

        public int Size
        {
            get { return _bytes.Length*7; }
        }

        public bool IsOverlong
        {
            get { return (_bytes.Length > 1) && ((_bytes[_bytes.Length - 1] & ValueBitsSet) == 0); }
        }

        public void Set(int fieldIndex)
        {
            _bytes[fieldIndex/7] |= (byte) ((1 << (6 - (fieldIndex%7))));
        }

        public bool IsSet(int fieldIndex)
        {
            if (fieldIndex >= _bytes.Length*7)
                return false;
            return (_bytes[fieldIndex/7] & (1 << (6 - (fieldIndex%7)))) > 0;
        }

        public override string ToString()
        {
            return "BitVector [" + ByteUtil.ConvertByteArrayToBitString(_bytes) + "]";
        }

        public int IndexOfLastSet()
        {
            int index = _bytes.Length*7 - 1;
            while (index >= 0 && !(((_bytes[index/7] & (1 << (6 - (index%7)))) > 0)))
                index--;
            return index;
        }

        #region Equals

        public bool Equals(BitVector other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Util.ArrayEquals(other._bytes, _bytes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            BitVector t = obj as BitVector;
            if ( t == null ) return false;
            return Util.ArrayEquals(t._bytes, _bytes);
        }

        public override int GetHashCode()
        {
            return Util.GetValTypeCollectionHashCode(_bytes);
        }

        #endregion
    }
}