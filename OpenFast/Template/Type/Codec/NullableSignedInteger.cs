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
using IntegerValue = OpenFAST.IntegerValue;
using NumericValue = OpenFAST.NumericValue;
using ScalarValue = OpenFAST.ScalarValue;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public sealed class NullableSignedInteger:IntegerCodec
	{
		override public bool Nullable
		{
			get
			{
				return true;
			}
			
		}
		private const long serialVersionUID = 1L;
		
		internal NullableSignedInteger()
		{
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			if (value_Renamed.Null)
			{
				return TypeCodec.NULL_VALUE_ENCODING;
			}
			
			NumericValue intValue = (NumericValue) value_Renamed;
			
			if (intValue.ToLong() >= 0)
			{
				return TypeCodec.INTEGER.EncodeValue(intValue.Increment());
			}
			else
			{
				return TypeCodec.INTEGER.EncodeValue(intValue);
			}
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			NumericValue numericValue = ((NumericValue) TypeCodec.INTEGER.Decode(in_Renamed));
			long value_Renamed = numericValue.ToLong();
			
			if (value_Renamed == 0)
			{
				return null;
			}
			
			if (value_Renamed > 0)
			{
				return numericValue.Decrement();
			}
			
			return numericValue;
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