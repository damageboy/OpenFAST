using System;
using DateValue = OpenFAST.DateValue;
using IntegerValue = OpenFAST.IntegerValue;
using ScalarValue = OpenFAST.ScalarValue;
using Util = OpenFAST.util.Util;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public class MillisecondsSinceMidnight:TypeCodec
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