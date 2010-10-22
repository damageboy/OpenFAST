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
using System.Text;

namespace OpenFAST.Template.Type.Codec
{
    [Serializable]
    internal sealed class ByteVectorType : TypeCodec
    {
        public override byte[] Encode(ScalarValue value)
        {
            byte[] bytes = value.Bytes;
            int lengthSize = IntegerCodec.GetUnsignedIntegerSize(bytes.Length);
            var encoding = new byte[bytes.Length + lengthSize];
            byte[] length = Uint.Encode(new IntegerValue(bytes.Length));
            Array.Copy(length, 0, encoding, 0, lengthSize);
            Array.Copy(bytes, 0, encoding, lengthSize, bytes.Length);
            return encoding;
        }

        public override ScalarValue Decode(Stream inStream)
        {
            try
            {
                int length = ((IntegerValue) Uint.Decode(inStream)).Value;
                var encoding = new byte[length];
                for (int i = 0; i < length; i++)
                    encoding[i] = (byte) inStream.ReadByte();
                return new ByteVectorValue(encoding);
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }
        }

        public override byte[] EncodeValue(ScalarValue value)
        {
            throw new NotSupportedException();
        }

        public static ScalarValue FromString(string value)
        {
            return new ByteVectorValue(Encoding.UTF8.GetBytes(value));
        }

        public override bool Equals(Object obj)
        {
            return obj != null && obj.GetType() == GetType(); //POINTP
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}