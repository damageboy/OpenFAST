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

namespace OpenFAST.Template.Type.Codec
{
    [Serializable]
    public sealed class SignedInteger : IntegerCodec
    {
        internal SignedInteger()
        {
        }

        public override byte[] EncodeValue(ScalarValue value)
        {
            long longValue = value.ToLong();
            int size = GetSignedIntegerSize(longValue);
            var encoding = new byte[size];

            for (int factor = 0; factor < size; factor++)
            {
                int bitMask = (factor == (size - 1)) ? 0x3f : 0x7f;
                encoding[size - factor - 1] = (byte) ((longValue >> (factor*7)) & bitMask);
            }

            // Get the sign bit from the long value and set it on the first byte
            // 01000000 00000000 ... 00000000
            //  ^----SIGN BIT
            encoding[0] |= (byte) ((0x40 & (longValue >> 57)));

            return encoding;
        }

        public override ScalarValue Decode(Stream inStream)
        {
            long value = 0;
            uint byt;
            try
            {
                byt = (uint) inStream.ReadByte();

                if ((byt & 0x40) > 0)
                {
                    value = - 1;
                }

                value = (value << 7) | (byt & 0x7f);

                while ((byt & STOP_BIT) == 0)
                {
                    byt = (uint) inStream.ReadByte();
                    value = (value << 7) | (byt & 0x7f);
                }
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }

            return CreateValue(value);
        }

        public override bool Equals(Object obj)
        {
            return obj != null && obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}