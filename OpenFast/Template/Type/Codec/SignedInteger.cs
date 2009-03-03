using System;
using NumericValue = OpenFAST.NumericValue;
using ScalarValue = OpenFAST.ScalarValue;
using OpenFAST;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public sealed class SignedInteger:IntegerCodec
	{
		private const long serialVersionUID = 1L;
		
		internal SignedInteger()
		{
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			long longValue = ((NumericValue) value_Renamed).ToLong();
			int size = GetSignedIntegerSize(longValue);
			byte[] encoding = new byte[size];
			
			for (int factor = 0; factor < size; factor++)
			{
				int bitMask = (factor == (size - 1))?0x3f:0x7f;
				encoding[size - factor - 1] = (byte) ((longValue >> (factor * 7)) & bitMask);
			}
			
			// Get the sign bit from the long value and set it on the first byte
			// 01000000 00000000 ... 00000000
			//  ^----SIGN BIT
			encoding[0] |= (byte) ((0x40 & (longValue >> 57)));
			
			return encoding;
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			long value_Renamed = 0;
			
			try
			{
				uint byt =(uint) in_Renamed.ReadByte();
				
				if ((byt & 0x40) > 0)
				{
					value_Renamed = - 1;
				}
				
				value_Renamed = (value_Renamed << 7) | (byt & 0x7f);
				
				while ((byt & STOP_BIT) == 0)
				{
                    byt = (uint)in_Renamed.ReadByte();
					value_Renamed = (value_Renamed << 7) | (byt & 0x7f);
				}
			}
			catch (System.IO.IOException e)
			{
				throw new RuntimeException(e);
			}
			
			return CreateValue(value_Renamed);
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