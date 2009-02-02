using System;
using ByteVectorValue = OpenFAST.ByteVectorValue;
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using FastException = OpenFAST.Error.FastException;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	public sealed class NullableUnicodeString:NotStopBitEncodedTypeCodec
	{
		public ScalarValue DefaultValue
		{
			get
			{
				return new StringValue("");
			}
			
		}
		private const long serialVersionUID = 1L;
		
		internal NullableUnicodeString()
		{
		}
		
		public override byte[] EncodeValue(ScalarValue value_Renamed)
		{
			if (value_Renamed.Null)
				return TypeCodec.NULLABLE_BYTE_VECTOR_TYPE.EncodeValue(ScalarValue.NULL);

            byte[] utf8encoding = System.Text.Encoding.UTF8.GetBytes(((StringValue) value_Renamed).value_Renamed);
			return TypeCodec.NULLABLE_BYTE_VECTOR_TYPE.Encode(new ByteVectorValue(utf8encoding));

		}
		
		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			ScalarValue decodedValue = TypeCodec.NULLABLE_BYTE_VECTOR_TYPE.Decode(in_Renamed);
			if (decodedValue == null)
				return null;
			ByteVectorValue value_Renamed = (ByteVectorValue) decodedValue;
			return new StringValue(System.Text.Encoding.UTF8.GetString(value_Renamed.value_Renamed));

		}

		public ScalarValue FromString(string value_Renamed)
		{
			return new StringValue(value_Renamed);
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