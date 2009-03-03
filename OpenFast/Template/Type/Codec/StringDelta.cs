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
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using TwinValue = OpenFAST.Template.TwinValue;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public sealed class StringDelta:TypeCodec
	{
		public ScalarValue DefaultValue
		{
			get
			{
				return new StringValue("");
			}
			
		}
		private const long serialVersionUID = 1L;
		
		public StringDelta()
		{
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			ScalarValue subtractionLength = TypeCodec.INTEGER.Decode(in_Renamed);
			ScalarValue difference = TypeCodec.ASCII.Decode(in_Renamed);
			
			return new TwinValue(subtractionLength, difference);
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			if ((value_Renamed == null) || (value_Renamed == ScalarValue.NULL))
			{
				throw new System.SystemException("Cannot have null values for non-nullable string delta");
			}
			
			TwinValue diff = (TwinValue) value_Renamed;
			byte[] subtractionLength = TypeCodec.INTEGER.Encode(diff.first);
			byte[] difference = TypeCodec.ASCII.Encode(diff.second);
			byte[] encoded = new byte[subtractionLength.Length + difference.Length];
			Array.Copy(subtractionLength, 0, encoded, 0, subtractionLength.Length);
			Array.Copy(difference, 0, encoded, subtractionLength.Length, difference.Length);
			
			return encoded;
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