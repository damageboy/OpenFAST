using System;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;

namespace OpenFAST.Template.Type
{
	[Serializable]
	public sealed class UnsignedIntegerType:IntegerType
	{
		private const long serialVersionUID = 1L;
		
		public UnsignedIntegerType(int numberBits, long maxValue):base("uInt" + numberBits, 0, maxValue, TypeCodec.UINT, TypeCodec.NULLABLE_UNSIGNED_INTEGER)
		{
		}

		public override TypeCodec GetCodec(Operator operator_Renamed, bool optional)
		{
			if (operator_Renamed.Equals(Operator.DELTA))
				if (optional)
					return TypeCodec.NULLABLE_INTEGER;
				else
					return TypeCodec.INTEGER;
			return base.GetCodec(operator_Renamed, optional);
		}
	}
}