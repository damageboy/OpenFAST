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
using ByteVectorValue = OpenFAST.ByteVectorValue;
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using FastException = OpenFAST.Error.FastException;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	sealed class UnicodeString:NotStopBitEncodedTypeCodec
	{

		public ScalarValue DefaultValue
		{
			get
			{
				return new StringValue("");
			}
			
		}
		private const long serialVersionUID = 1L;
		
		internal UnicodeString()
		{
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
				byte[] utf8encoding = System.Text.Encoding.UTF8.GetBytes(((StringValue) value_Renamed).value_Renamed);
				return TypeCodec.BYTE_VECTOR.Encode(new ByteVectorValue(utf8encoding));

		}
		

		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			ByteVectorValue value_Renamed = (ByteVectorValue) TypeCodec.BYTE_VECTOR.Decode(in_Renamed);

				return new StringValue(System.Text.Encoding.UTF8.GetString(value_Renamed.value_Renamed));

		}

		public ScalarValue FromString(string value_Renamed)
		{
			return new StringValue(value_Renamed);
		}
	}
}