using System;
using ByteVectorValue = OpenFAST.ByteVectorValue;
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using FastException = OpenFAST.Error.FastException;

namespace OpenFAST.Template.Type.Codec
{
	[Serializable]
	sealed class UnicodeString:NotStopBitEncodedTypeCodec
	{

		public ScalarValue DefaultValue
		{
			get
			{
				return new StringValue("");
			}
			
		}
		private const long serialVersionUID = 1L;
		
		internal UnicodeString()
		{
		}
		
		public override sbyte[] EncodeValue(ScalarValue value_Renamed)
		{
				sbyte[] utf8encoding = SupportClass.ToSByteArray(System.Text.Encoding.UTF8.GetBytes(((StringValue) value_Renamed).value_Renamed));
				return TypeCodec.BYTE_VECTOR.Encode(new ByteVectorValue(utf8encoding));

		}
		

		public override ScalarValue Decode(System.IO.Stream in_Renamed)
		{
			ByteVectorValue value_Renamed = (ByteVectorValue) TypeCodec.BYTE_VECTOR.Decode(in_Renamed);

				return new StringValue(System.Text.Encoding.UTF8.GetString(SupportClass.ToByteArray(value_Renamed.value_Renamed)));

		}

		public ScalarValue FromString(string value_Renamed)
		{
			return new StringValue(value_Renamed);
		}
	}
}