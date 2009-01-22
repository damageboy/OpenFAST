using System;
using ScalarValue = OpenFAST.ScalarValue;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;

namespace OpenFAST.Template.Type
{
	[Serializable]
	public abstract class SimpleType:FASTType
	{
		private TypeCodec codec;
		private TypeCodec nullableCodec;
		
		public SimpleType(string typeName, TypeCodec codec, TypeCodec nullableCodec):base(typeName)
		{
			this.codec = codec;
			this.nullableCodec = nullableCodec;
		}
		

		public override TypeCodec GetCodec(Operator operator_Renamed, bool optional)
		{
			if (optional)
				return nullableCodec;
			return codec;
		}
		

		public override ScalarValue GetValue(string value_Renamed)
		{
			if (value_Renamed == null)
				return null;
			return GetVal(value_Renamed);
		}
		
		public abstract ScalarValue GetVal(string value_Renamed);
	}
}