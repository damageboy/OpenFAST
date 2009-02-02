using System;
using ScalarValue = OpenFAST.ScalarValue;
using OpenFAST;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public sealed class UnsignedInteger:IntegerCodec
	{
		private const long serialVersionUID = 1L;
		
		internal UnsignedInteger()
		{
		}
		
		public override byte[] EncodeValue(ScalarValue scalarValue)
		{
			long value_Renamed = scalarValue.ToLong();
			int size = GetUnsignedIntegerSize(value_Renamed);
			byte[] encoded = new byte[size];
			
			for (int factor = 0; factor < size; factor++)
			{
				encoded[size - factor - 1] = (byte) ((value_Renamed >> (factor * 7)) & 0x7f);
			}
			
			return encoded;
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			long value_Renamed = 0;
			uint byt;
			
			try
			{
				do 
				{
					byt =(uint) in_Renamed.ReadByte();
					value_Renamed = (value_Renamed << 7) | (byt & 0x7f);
				}
				while ((byt & 0x80) == 0);
			}
			catch (System.IO.IOException e)
			{
				throw new RuntimeException(e);
			}
			
			return CreateValue(value_Renamed);
		}
		
		public  override bool Equals(System.Object obj)
		{
			return obj != null && GetType() == obj.GetType();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}