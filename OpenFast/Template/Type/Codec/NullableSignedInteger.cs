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
		
		public override sbyte[] EncodeValue(ScalarValue value_Renamed)
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