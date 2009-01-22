using System;
using DateValue = OpenFAST.DateValue;
using ScalarValue = OpenFAST.ScalarValue;
using LongValue = OpenFAST.Template.LongValue;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public class EpochTimestamp:TypeCodec
	{
		
		private const long serialVersionUID = 1L;
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			System.DateTime tempAux = new System.DateTime(TypeCodec.INTEGER.Decode(in_Renamed).ToLong());
			return new DateValue(ref tempAux);
		}
		
		public override sbyte[] EncodeValue(ScalarValue value_Renamed)
		{
			return TypeCodec.INTEGER.EncodeValue(new LongValue(value_Renamed.ToLong()));
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