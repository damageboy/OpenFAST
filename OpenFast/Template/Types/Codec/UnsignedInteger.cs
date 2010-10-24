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
using System.IO;

namespace OpenFAST.Template.Types.Codec
{
    [Serializable]
    public sealed class UnsignedInteger : IntegerCodec
    {
        internal UnsignedInteger()
        {
        }

        public override byte[] EncodeValue(ScalarValue scalarValue)
        {
            long value = scalarValue.ToLong();
            int size = GetUnsignedIntegerSize(value);
            var encoded = new byte[size];

            for (int factor = 0; factor < size; factor++)
            {
                encoded[size - factor - 1] = (byte) ((value >> (factor*7)) & 0x7f);
            }

            return encoded;
        }

        public override ScalarValue Decode(Stream inStream)
        {
            long value = 0;
            uint byt;
            try
            {
                do
                {
                    byt = (uint) inStream.ReadByte();
                    value = (value << 7) | (byt & 0x7f);
                } while ((byt & StopBit) == 0);
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }

            return CreateValue(value);
        }

        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}