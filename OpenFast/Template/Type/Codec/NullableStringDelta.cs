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
	public sealed class NullableStringDelta:TypeCodec
	{

		public static ScalarValue DefaultValue
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

	    public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			ScalarValue subtractionLength = NULLABLE_INTEGER.Decode(in_Renamed);
			if (subtractionLength == null)
				return null;
			
			ScalarValue difference = ASCII.Decode(in_Renamed);
			
			return new TwinValue(subtractionLength, difference);
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			if (value_Renamed.Null)
				return NULL_VALUE_ENCODING;
			
			var diff = (TwinValue) value_Renamed;
			byte[] subtractionLength = NULLABLE_INTEGER.Encode(diff.first);
			byte[] difference = ASCII.Encode(diff.second);
			var encoded = new byte[subtractionLength.Length + difference.Length];
			Array.Copy(subtractionLength, 0, encoded, 0, subtractionLength.Length);
			Array.Copy(difference, 0, encoded, subtractionLength.Length, difference.Length);
			
			return encoded;
		}
		
		public static ScalarValue FromString(string value_Renamed)
		{
			return new StringValue(value_Renamed);
		}
		
		public  override bool Equals(Object obj)
		{
			return obj != null && obj.GetType() == GetType();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}