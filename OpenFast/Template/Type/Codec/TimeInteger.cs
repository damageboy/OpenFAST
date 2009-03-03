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
using DateValue = OpenFAST.DateValue;
using IntegerValue = OpenFAST.IntegerValue;
using ScalarValue = OpenFAST.ScalarValue;
using Util = OpenFAST.util.Util;

namespace OpenFAST.Template.Type.Codec
{
	
	[Serializable]
	public sealed class TimeInteger:TypeCodec
	{
		private const long serialVersionUID = 1L;
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			int intValue = ((IntegerValue) TypeCodec.UINT.Decode(in_Renamed)).value_Renamed;
			int hour = intValue / 10000000;
			intValue -= hour * 10000000;
			int minute = intValue / 100000;
			intValue -= minute * 100000;
			int second = intValue / 1000;
			intValue -= second * 1000;
			int millisecond = intValue % 1000;
			System.DateTime tempAux = new System.DateTime(hour * 3600000 + minute * 60000 + second * 1000 + millisecond);
			return new DateValue(ref tempAux);
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			System.DateTime date = ((DateValue) value_Renamed).value_Renamed;
			int intValue = Util.TimeToInt(ref date);
			return TypeCodec.UINT.Encode(new IntegerValue(intValue));
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