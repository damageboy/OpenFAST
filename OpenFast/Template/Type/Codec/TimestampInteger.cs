using System;
using DateValue = OpenFAST.DateValue;
using IntegerValue = OpenFAST.IntegerValue;
using ScalarValue = OpenFAST.ScalarValue;
using Util = OpenFAST.util.Util;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public class TimestampInteger:TypeCodec
	{
		private const long serialVersionUID = 1L;
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			int intValue = ((IntegerValue) TypeCodec.UINT.Decode(in_Renamed)).value_Renamed;
			System.DateTime tempAux = Util.ToTimestamp(intValue);
			return new DateValue(ref tempAux);
		}
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			System.DateTime date = ((DateValue) value_Renamed).value_Renamed;
			int intValue = Util.TimestampToInt(ref date);
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