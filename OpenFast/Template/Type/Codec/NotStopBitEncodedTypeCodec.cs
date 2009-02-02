using System;
using ScalarValue = OpenFAST.ScalarValue;

namespace OpenFAST.Template.Type.Codec
{
	
	[Serializable]
	public abstract class NotStopBitEncodedTypeCodec:TypeCodec
	{
		
		public override byte[] Encode(ScalarValue value_Renamed)
		{
			return EncodeValue(value_Renamed);
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