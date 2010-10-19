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
using OpenFAST.Error;

namespace OpenFAST.Template.Type.Codec
{
    [Serializable]
    internal sealed class NullableAsciiString : TypeCodec
    {
        private static readonly byte[] NullableEmptyString = new byte[] {0x00, 0x00};
        private static readonly byte[] ZeroEncoding = new byte[] {0x00, 0x00, 0x00};

        public static ScalarValue DefaultValue
        {
            get { return new StringValue(""); }
        }

        public override bool IsNullable
        {
            get { return true; }
        }

        public override byte[] EncodeValue(ScalarValue value)
        {
            if (value.IsNull)
            {
                return NULL_VALUE_ENCODING;
            }

            string str = ((StringValue) value).Value;

            if (str != null)
            {
                if ((str.Length == 0))
                {
                    return NullableEmptyString;
                }

                return str.StartsWith("\u0000") ? ZeroEncoding : Encoding.UTF8.GetBytes(str);
            }
            return ZeroEncoding;
        }

        public override ScalarValue Decode(Stream inStream)
        {
            var buffer = new MemoryStream();

            try
            {
                int byt;
                do
                {
                    byt = inStream.ReadByte();
                    buffer.WriteByte((byte) byt);
                } while ((byt & STOP_BIT) == 0);
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }

            byte[] bytes = buffer.ToArray();
            bytes[bytes.Length - 1] &= (0x7f);

            if (bytes[0] == 0)
            {
                if (!ByteUtil.IsEmpty(bytes))
                    Global.HandleError(FastConstants.R9_STRING_OVERLONG, null);
                if ((bytes.Length == 1))
                {
                    return null;
                }
                if (bytes.Length == 2 && bytes[1] == 0)
                {
                    return new StringValue("");
                }
                if (bytes.Length == 3 && bytes[2] == 0)
                {
                    return new StringValue("\u0000");
                }
            }

            return new StringValue(Encoding.UTF8.GetString(bytes));
        }

        public static ScalarValue FromString(string value)
        {
            return new StringValue(value);
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