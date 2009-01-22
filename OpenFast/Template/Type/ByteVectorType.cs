using System;
using ByteVectorValue = OpenFAST.ByteVectorValue;
using ScalarValue = OpenFAST.ScalarValue;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;

namespace OpenFAST.Template.Type
{
	[Serializable]
	sealed class ByteVectorType:SimpleType
	{
		override public ScalarValue DefaultValue
		{
			get
			{
				return new ByteVectorValue(new sbyte[]{});
			}
			
		}
		private const long serialVersionUID = 1L;
		
		internal ByteVectorType():base("byteVector", TypeCodec.BYTE_VECTOR, TypeCodec.NULLABLE_BYTE_VECTOR_TYPE)
		{
		}
		
		public override ScalarValue GetVal(string value_Renamed)
		{
			return new ByteVectorValue(SupportClass.ToSByteArray(SupportClass.ToByteArray(value_Renamed)));
		}
		
		public override bool IsValueOf(ScalarValue previousValue)
		{
			return previousValue is ByteVectorValue;
		}
	}
}