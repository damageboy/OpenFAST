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
	public sealed class MillisecondsSinceMidnight:TypeCodec
	{
		private const long serialVersionUID = 1L;
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			int millisecondsSinceMidnight = TypeCodec.INTEGER.Decode(in_Renamed).ToInt();
			System.Globalization.Calendar cal = new System.Globalization.GregorianCalendar();
			int hour = millisecondsSinceMidnight / 3600000;
			millisecondsSinceMidnight -= hour * 3600000;
			SupportClass.CalendarManager.manager.Set(cal, SupportClass.CalendarManager.HOUR_OF_DAY, hour);
			int minute = millisecondsSinceMidnight / 60000;
			millisecondsSinceMidnight -= minute * 60000;
			SupportClass.CalendarManager.manager.Set(cal, SupportClass.CalendarManager.MINUTE, minute);
			int second = millisecondsSinceMidnight / 1000;
			millisecondsSinceMidnight -= second * 1000;
			SupportClass.CalendarManager.manager.Set(cal, SupportClass.CalendarManager.SECOND, second);
			int millisecond = millisecondsSinceMidnight;
			SupportClass.CalendarManager.manager.Set(cal, SupportClass.CalendarManager.MILLISECOND, millisecond);
			System.DateTime tempAux = SupportClass.CalendarManager.manager.GetDateTime(cal);
			return new DateValue(ref tempAux);
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			System.DateTime date = ((DateValue) value_Renamed).value_Renamed;
			int millisecondsSinceMidnight = Util.MillisecondsSinceMidnight(ref date);
			return TypeCodec.INTEGER.EncodeValue(new IntegerValue(millisecondsSinceMidnight));
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