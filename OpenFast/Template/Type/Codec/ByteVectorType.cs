using System;
using ByteVectorValue = OpenFAST.ByteVectorValue;
using IntegerValue = OpenFAST.IntegerValue;
using ScalarValue = OpenFAST.ScalarValue;
using OpenFAST;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	sealed class ByteVectorType:TypeCodec
	{
		private const long serialVersionUID = 1L;
		
		internal ByteVectorType()
		{
		}

		public override byte[] Encode(ScalarValue value_Renamed)
		{
			byte[] bytes = value_Renamed.Bytes;
			int lengthSize = IntegerCodec.GetUnsignedIntegerSize(bytes.Length);
			byte[] encoding = new byte[bytes.Length + lengthSize];
			byte[] length = TypeCodec.UINT.Encode(new IntegerValue(bytes.Length));
			Array.Copy(length, 0, encoding, 0, lengthSize);
			Array.Copy(bytes, 0, encoding, lengthSize, bytes.Length);
			return encoding;
		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			int length = ((IntegerValue) TypeCodec.UINT.Decode(in_Renamed)).value_Renamed;
			byte[] encoding = new byte[length];
			for (int i = 0; i < length; i++)
				try
				{
					encoding[i] = (byte) in_Renamed.ReadByte();
				}
				catch (System.IO.IOException e)
				{
					throw new RuntimeException(e);
				}
			return new ByteVectorValue(encoding);
		}
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			throw new System.NotSupportedException();
		}
		
		public ScalarValue FromString(string value_Renamed)
		{
			return new ByteVectorValue(SupportClass.ToByteArray(value_Renamed));
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