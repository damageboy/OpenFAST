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

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public abstract class TypeCodec
	{
		virtual public bool Nullable
		{
			get
			{
				return false;
			}
			
		}
		protected internal static byte STOP_BIT = 0x80;
		internal static readonly byte[] NULL_VALUE_ENCODING = new[]{STOP_BIT};
		
		// Codec Definitions
		public static readonly TypeCodec UINT = new UnsignedInteger();
		public static readonly TypeCodec INTEGER = new SignedInteger();
		public static readonly TypeCodec ASCII = new AsciiString();
		public static readonly TypeCodec UNICODE = new UnicodeString();
		public static readonly TypeCodec BIT_VECTOR = new BitVectorType();
		public static readonly TypeCodec BYTE_VECTOR = new ByteVectorType();
		public static readonly TypeCodec SF_SCALED_NUMBER = new SingleFieldDecimal();
		public static readonly TypeCodec STRING_DELTA = new StringDelta();
		
		public static readonly TypeCodec NULLABLE_UNSIGNED_INTEGER = new NullableUnsignedInteger();
		public static readonly TypeCodec NULLABLE_INTEGER = new NullableSignedInteger();
		public static readonly TypeCodec NULLABLE_ASCII = new NullableAsciiString();
		public static readonly TypeCodec NULLABLE_UNICODE = new NullableUnicodeString();
		public static readonly TypeCodec NULLABLE_BYTE_VECTOR_TYPE = new NullableByteVector();
		public static readonly TypeCodec NULLABLE_SF_SCALED_NUMBER = new NullableSingleFieldDecimal();
		public static readonly TypeCodec NULLABLE_STRING_DELTA = new NullableStringDelta();
		
		// DATE CODECS
		public static readonly TypeCodec DATE_STRING = new DateString("yyyyMMdd");
		public static readonly TypeCodec DATE_INTEGER = new DateInteger();
		public static readonly TypeCodec TIMESTAMP_STRING = new DateString("yyyyMMddhhmmssSSS");
		public static readonly TypeCodec TIMESTAMP_INTEGER = new TimestampInteger();
		public static readonly TypeCodec EPOCH_TIMESTAMP = new EpochTimestamp();
		public static readonly TypeCodec TIME_STRING = new DateString("hhmmssSSS");
		public static readonly TypeCodec TIME_INTEGER = new TimeInteger();
		public static readonly TypeCodec TIME_IN_MS = new MillisecondsSinceMidnight();
		
		public abstract byte[] EncodeValue(ScalarValue value_Renamed);
		public abstract ScalarValue Decode(System.IO.Stream in_Renamed);
		
		public virtual byte[] Encode(ScalarValue value_Renamed)
		{
			byte[] encoding = EncodeValue(value_Renamed);
            encoding[encoding.Length - 1] |= STOP_BIT; // add stop bit;
			return encoding;
		}
	}
}