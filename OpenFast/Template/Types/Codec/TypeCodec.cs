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
    public abstract class TypeCodec
    {
        protected const byte StopBit = 0x80;
        internal static readonly byte[] NullValueEncoding = new[] {StopBit};

        // Codec Definitions
        public static readonly TypeCodec Uint = new UnsignedInteger();
        public static readonly TypeCodec Integer = new SignedInteger();
        public static readonly TypeCodec Ascii = new AsciiString();
        public static readonly TypeCodec Unicode = new UnicodeString();
        public static readonly TypeCodec BitVector = new BitVectorType();
        public static readonly TypeCodec ByteVector = new ByteVectorType();
        public static readonly TypeCodec SfScaledNumber = new SingleFieldDecimal();
        public static readonly TypeCodec StringDelta = new StringDelta();

        public static readonly TypeCodec NullableUnsignedInteger = new NullableUnsignedInteger();
        public static readonly TypeCodec NullableInteger = new NullableSignedInteger();
        public static readonly TypeCodec NullableAscii = new NullableAsciiString();
        public static readonly TypeCodec NullableUnicode = new NullableUnicodeString();
        public static readonly TypeCodec NullableByteVectorType = new NullableByteVector();
        public static readonly TypeCodec NullableSfScaledNumber = new NullableSingleFieldDecimal();
        public static readonly TypeCodec NullableStringDelta = new NullableStringDelta();

        // DATE CODECS
        public static readonly TypeCodec DateString = new DateString("yyyyMMdd");
        public static readonly TypeCodec DateInteger = new DateInteger();
        public static readonly TypeCodec TimestampString = new DateString("yyyyMMddhhmmssSSS");
        public static readonly TypeCodec TimestampInteger = new TimestampInteger();
        public static readonly TypeCodec EpochTimestamp = new EpochTimestamp();
        public static readonly TypeCodec TimeString = new DateString("hhmmssSSS");
        public static readonly TypeCodec TimeInteger = new TimeInteger();
        public static readonly TypeCodec TimeInMs = new MillisecondsSinceMidnight();

        public virtual bool IsNullable
        {
            get { return false; }
        }

        public abstract byte[] EncodeValue(ScalarValue value);
        public abstract ScalarValue Decode(Stream inStream);

        public virtual byte[] Encode(ScalarValue value)
        {
            byte[] encoding = EncodeValue(value);
            encoding[encoding.Length - 1] |= StopBit; // add stop bit;
            return encoding;
        }
    }
}