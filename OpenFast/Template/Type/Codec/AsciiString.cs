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
	sealed class AsciiString:TypeCodec
	{
	    private const string ZERO_TERMINATOR = "\u0000";

		private static readonly byte[] ZERO_PREAMBLE = new byte[]{0, 0};

	    public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			if ((value_Renamed == null) || value_Renamed.Null)
			{
				throw new SystemException("Only nullable strings can represent null values.");
			}
			
			string string_Renamed = value_Renamed.ToString();
			
			
	        if (string_Renamed != null)
	        {
                if (string_Renamed.Length == 0)
                {
                    return NULL_VALUE_ENCODING;
                }
	            if (string_Renamed.StartsWith(ZERO_TERMINATOR))
	            {
	                return ZERO_PREAMBLE;
	            }
                return System.Text.Encoding.UTF8.GetBytes(string_Renamed);
	        }
            return NULL_VALUE_ENCODING;
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			var buffer = new System.IO.MemoryStream();
			byte byt;
			
			try
			{
				do 
				{
                    byt = (byte)in_Renamed.ReadByte();
					buffer.WriteByte( byt);
				}
				while ((byt & STOP_BIT) == 0);
			}
			catch (System.IO.IOException e)
			{
				throw new RuntimeException(e);
			}
			
			byte[] bytes = buffer.ToArray();
			bytes[bytes.Length - 1] &=  (0x7f);
			
			if (bytes[0] == 0)
			{
				if (!ByteUtil.IsEmpty(bytes))
					Global.HandleError(Error.FastConstants.R9_STRING_OVERLONG, null);
				if (bytes.Length > 1 && bytes[1] == 0)
					return new StringValue("\u0000");
				return new StringValue("");
			}

            return new StringValue(System.Text.Encoding.UTF8.GetString(bytes));
		}
		
		public static ScalarValue FromString(string value_Renamed)
		{
			return new StringValue(value_Renamed);
		}
		
		public  override bool Equals(Object obj)
		{
            return obj != null && obj.GetType() == GetType();//POINTP
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}