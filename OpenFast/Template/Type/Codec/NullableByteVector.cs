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
using Global = OpenFAST.Global;
using IntegerValue = OpenFAST.IntegerValue;
using ScalarValue = OpenFAST.ScalarValue;

namespace OpenFAST.Template.Type.Codec
{
	
	[Serializable]
	public sealed class NullableByteVector:NotStopBitEncodedTypeCodec
	{

		public ScalarValue DefaultValue
		{
			get
			{
				return new ByteVectorValue(new byte[]{});
			}
			
		}
		
		private const long serialVersionUID = 1L;
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			ScalarValue decode = TypeCodec.NULLABLE_UNSIGNED_INTEGER.Decode(in_Renamed);
			if (decode == null)
				return null;
			int length = ((ScalarValue) decode).ToInt();
			byte[] encoding = new byte[length];
			
			for (int i = 0; i < length; i++)
				try
				{
					encoding[i] = (byte) in_Renamed.ReadByte();
				}
				catch (System.IO.IOException e)
				{
					Global.HandleError(OpenFAST.Error.FastConstants.IO_ERROR, "An error occurred while decoding a nullable byte vector.", e);
				}
			return new ByteVectorValue(encoding);
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			if (value_Renamed.Null)
				return TypeCodec.NULLABLE_UNSIGNED_INTEGER.EncodeValue(ScalarValue.NULL);
			ByteVectorValue byteVectorValue = (ByteVectorValue) value_Renamed;
			int lengthSize = IntegerCodec.GetUnsignedIntegerSize(byteVectorValue.value_Renamed.Length);
			byte[] encoding = new byte[byteVectorValue.value_Renamed.Length + lengthSize];
			byte[] length = TypeCodec.NULLABLE_UNSIGNED_INTEGER.Encode(new IntegerValue(byteVectorValue.value_Renamed.Length));
			Array.Copy(length, 0, encoding, 0, lengthSize);
			Array.Copy(byteVectorValue.value_Renamed, 0, encoding, lengthSize, byteVectorValue.value_Renamed.Length);
			return encoding;
		}
		
		public ScalarValue FromString(string value_Renamed)
		{
			return new ByteVectorValue(SupportClass.ToByteArray(value_Renamed));
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