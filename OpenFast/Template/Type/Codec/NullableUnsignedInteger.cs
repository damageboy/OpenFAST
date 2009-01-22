using System;
using IntegerValue = OpenFAST.IntegerValue;
using NumericValue = OpenFAST.NumericValue;
using ScalarValue = OpenFAST.ScalarValue;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public sealed class NullableUnsignedInteger:IntegerCodec
	{
		override public bool Nullable
		{
			get
			{
				return true;
			}
			
		}
		private const long serialVersionUID = 1L;
		
		internal NullableUnsignedInteger()
		{
		}
		
		public override sbyte[] EncodeValue(ScalarValue v)
		{
			if (v.Null)
			{
				return TypeCodec.NULL_VALUE_ENCODING;
			}
			
			return TypeCodec.UINT.EncodeValue(((NumericValue) v).Increment());
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			NumericValue value_Renamed = (NumericValue) TypeCodec.UINT.Decode(in_Renamed);
			
			if (value_Renamed.Equals(0))
			{
				return null;
			}
			
			return value_Renamed.Decrement();
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