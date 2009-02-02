using System;
using BitVector = OpenFAST.BitVector;
using BitVectorValue = OpenFAST.BitVectorValue;
using ScalarValue = OpenFAST.ScalarValue;
using OpenFAST;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public sealed class BitVectorType:TypeCodec
	{
		public ScalarValue DefaultValue
		{
			get
			{
				return new BitVectorValue(new BitVector(0));
			}
			
		}
		private const long serialVersionUID = 1L;
		
		internal BitVectorType()
		{
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			return ((BitVectorValue) value_Renamed).value_Renamed.Bytes;
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			System.IO.MemoryStream buffer = new System.IO.MemoryStream();
			int byt;
			do 
			{
				try
				{
					byt = in_Renamed.ReadByte();
					
					if (byt < 0)
					{
						return null;
					}
				}
				catch (System.IO.IOException e)
				{
					throw new RuntimeException(e);
				}
				
				buffer.WriteByte((System.Byte) byt);
			}
			while ((byt & 0x80) == 0);
			
			return new BitVectorValue(new BitVector(buffer.ToArray()));
		}
		
		public ScalarValue FromString(string value_Renamed)
		{
			return null;
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