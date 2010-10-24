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

namespace OpenFAST.Template.Types.Codec
{
    [Serializable]
    internal sealed class NullableAsciiString : TypeCodec
    {
        private const int BufferInitSize = 1000;
        private static readonly byte[] NullableEmptyString = new byte[] {0x00, 0x00};
        private static readonly byte[] ZeroEncoding = new byte[] {0x00, 0x00, 0x00};
        [ThreadStatic] private static byte[] _buffer;

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
                return NullValueEncoding;
            }

            string str = ((StringValue) value).Value;

            if (str != null)
            {
                if (str.Length == 0)
                    return NullableEmptyString;

                return str.StartsWith("\u0000") ? ZeroEncoding : Encoding.UTF8.GetBytes(str);
            }
            return ZeroEncoding;
        }

        public override ScalarValue Decode(Stream inStream)
        {
            // TODO: This code looks almost identical to Decode of the AsciiString. Merge?
            try
            {
                byte[] buf = _buffer;
                if (buf == null)
                    _buffer = buf = new byte[BufferInitSize];

                int ind = 0;

                while (true)
                {
                    var b = (byte) inStream.ReadByte();

                    if ((b & StopBit) == 0)
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
                    if (!ByteUtil.IsEmpty(buf, ind))
                    {
                        Global.ErrorHandler.OnError(null, RepError.StringOverlong, null);
                    }

                    if (ind == 1)
                        return null;

                    if (ind == 2 && buf[1] == 0)
                        return new StringValue("");

                    if (ind == 3 && buf[2] == 0)
                        return new StringValue("\u0000");
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
            return obj != null && obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}