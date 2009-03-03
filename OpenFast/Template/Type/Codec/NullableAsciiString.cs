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
using ByteUtil = OpenFAST.ByteUtil;
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using OpenFAST;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	sealed class NullableAsciiString:TypeCodec
	{
		public ScalarValue DefaultValue
		{
			get
			{
				return new StringValue("");
			}
			
		}

		override public bool Nullable
		{
			get
			{
				return true;
			}
			
		}
		private const long serialVersionUID = 1L;
		private static readonly byte[] NULLABLE_EMPTY_STRING = new byte[]{(byte) (0x00), (byte) (0x00)};
		private static readonly byte[] ZERO_ENCODING = new byte[]{(byte) (0x00), (byte) (0x00), (byte) (0x00)};
		
		internal NullableAsciiString()
		{
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			if (value_Renamed.Null)
			{
				return TypeCodec.NULL_VALUE_ENCODING;
			}
			
			string string_Renamed = ((StringValue) value_Renamed).value_Renamed;
			
			if ((string_Renamed != null) && (string_Renamed.Length == 0))
			{
				return NULLABLE_EMPTY_STRING;
			}
			
			if (string_Renamed.StartsWith("\u0000"))
				return ZERO_ENCODING;
			return SupportClass.ToByteArray(string_Renamed);
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			System.IO.MemoryStream buffer = new System.IO.MemoryStream();
			int byt;
			
			try
			{
				do 
				{
					byt = in_Renamed.ReadByte();
					buffer.WriteByte((System.Byte) byt);
				}
				while ((byt & STOP_BIT) == 0);
			}
			catch (System.IO.IOException e)
			{
				throw new RuntimeException(e);
			}
			
			byte[] bytes = buffer.ToArray();
			bytes[bytes.Length - 1] &= (byte) (0x7f);
			
			if (bytes[0] == 0)
			{
				if (!ByteUtil.IsEmpty(bytes))
					Global.HandleError(OpenFAST.Error.FastConstants.R9_STRING_OVERLONG, null);
				if ((bytes.Length == 1))
				{
					return null;
				}
				else if (bytes.Length == 2 && bytes[1] == 0)
				{
					return new StringValue("");
				}
				else if (bytes.Length == 3 && bytes[2] == 0)
				{
					return new StringValue("\u0000");
				}
			}
			
			return new StringValue(new string(SupportClass.ToCharArray(bytes)));
		}
		
		public ScalarValue FromString(string value_Renamed)
		{
			return new StringValue(value_Renamed);
		}
		
		public  override bool Equals(System.Object obj)
		{
			return obj != null && obj.GetType() == GetType();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}