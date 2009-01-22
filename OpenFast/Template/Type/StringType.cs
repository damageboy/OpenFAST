using System;
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;

namespace OpenFAST.Template.Type
{
	[Serializable]
	public class StringType:SimpleType
	{
		override public ScalarValue DefaultValue
		{
			get
			{
				return new StringValue("");
			}
			
		}
		private const long serialVersionUID = 1L;
		
		public StringType(string typeName, TypeCodec codec, TypeCodec nullableCodec):base(typeName, codec, nullableCodec)
		{
		}
		
		public override ScalarValue GetVal(string value_Renamed)
		{
			return new StringValue(value_Renamed);
		}
		
		public override TypeCodec GetCodec(Operator operator_Renamed, bool optional)
		{
			if (operator_Renamed == Operator.DELTA)
				return (optional)?TypeCodec.NULLABLE_STRING_DELTA:TypeCodec.STRING_DELTA;
			return base.GetCodec(operator_Renamed, optional);
		}
		
		public override bool IsValueOf(ScalarValue previousValue)
		{
			return previousValue is StringValue;
		}
	}
}