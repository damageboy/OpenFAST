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
	sealed class UnicodeString:NotStopBitEncodedTypeCodec
	{

		public static ScalarValue DefaultValue
		{
			get
			{
				return new StringValue("");
			}
			
		}

	    public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			var utf8encoding = System.Text.Encoding.UTF8.GetBytes(((StringValue) value_Renamed).value_Renamed);
			return BYTE_VECTOR.Encode(new ByteVectorValue(utf8encoding));
		}
		

		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			var value_Renamed = (ByteVectorValue) BYTE_VECTOR.Decode(in_Renamed);
			return new StringValue(System.Text.Encoding.UTF8.GetString(value_Renamed.value_Renamed));
		}

		public static ScalarValue FromString(string value_Renamed)
		{
			return new StringValue(value_Renamed);
		}
	}
}