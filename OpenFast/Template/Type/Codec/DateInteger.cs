using System;
using DateValue = OpenFAST.DateValue;
using IntegerValue = OpenFAST.IntegerValue;
using ScalarValue = OpenFAST.ScalarValue;
using Util = OpenFAST.util.Util;

namespace OpenFAST.Template.Type.Codec
{
	
	[Serializable]
	public class DateInteger:TypeCodec
	{
		private const long serialVersionUID = 1L;
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			long longValue = ((ScalarValue) TypeCodec.UINT.Decode(in_Renamed)).ToLong();
			int year = (int) (longValue / 10000);
			int month = (int) ((longValue - (year * 10000)) / 100);
			int day = (int) (longValue % 100);
			System.DateTime tempAux = Util.Date(year, month, day);
			return new DateValue(ref tempAux);
		}
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			System.DateTime date = ((DateValue) value_Renamed).value_Renamed;
			int intValue = Util.DateToInt(ref date);
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