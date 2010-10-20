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
    internal sealed class AsciiString : TypeCodec
    {
        private const int BufferInitSize = 1000;

        private const string ZeroTerminator = "\u0000";
        [ThreadStatic] private static byte[] _buffer;

        private static readonly byte[] ZeroPreamble = new byte[] {0, 0};

        public override byte[] EncodeValue(ScalarValue value)
        {
            if (value == null || value.IsNull)
                throw new ArgumentNullException("value", "Only nullable strings can represent null values.");

            string str = value.ToString();
#warning Bug? ToString should always have a non-null value
            if (str != null)
            {
                if (str.Length == 0)
                {
                    return NULL_VALUE_ENCODING;
                }
                if (str.StartsWith(ZeroTerminator))
                {
                    return ZeroPreamble;
                }
                return Encoding.UTF8.GetBytes(str);
            }

            return NULL_VALUE_ENCODING;
        }

        public override ScalarValue Decode(Stream inStream)
        {
            // TODO: This code looks almost identical to Decode of the NullableAsciiString. Merge?
            try
            {
                byte[] buf = _buffer;
                if (buf == null)
                    _buffer = buf = new byte[BufferInitSize];

                int ind = 0;

                while (true)
                {
                    var b = (byte) inStream.ReadByte();

                    if ((b & STOP_BIT) == 0)
                    {
                        buf[ind++] = b;
                        if (ind >= buf.Length)
                        {
                            var newBuf = new byte[buf.Length*2];
                            Array.Copy(buf, newBuf, ind);
                            _buffer = buf = newBuf;
                        }
                    }
                    else
                    {
                        buf[ind++] = (byte) (b & 0x7f);
                        break;
                    }
                }

                if (buf[0] == 0)
                {
                    // BUG? optimize with "else if"?
                    if (!ByteUtil.IsEmpty(buf, ind))
                        Global.HandleError(FastConstants.R9_STRING_OVERLONG, null);
                    if (ind > 1 && buf[1] == 0)
                        return new StringValue("\u0000");
                    return new StringValue("");
                }

                return new StringValue(Encoding.UTF8.GetString(buf, 0, ind));
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }
        }

        public static ScalarValue FromString(string value)
        {
            return new StringValue(value);
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