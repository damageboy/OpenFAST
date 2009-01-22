using System;
using ScalarValue = OpenFAST.ScalarValue;
using Scalar = OpenFAST.Template.Scalar;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.operator_Renamed
{
	[Serializable]
	sealed class NoneOperatorCodec:AlwaysPresentOperatorCodec
	{
		
		private const long serialVersionUID = 1L;
		
		internal NoneOperatorCodec(Operator operator_Renamed, FASTType[] types):base(operator_Renamed, types)
		{
		}
		
		public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field)
		{
			if (value_Renamed == null)
			{
				return ScalarValue.NULL;
			}
			
			return value_Renamed;
		}
		
		public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue, Scalar field)
		{
			return newValue;
		}
		
		public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
		{
			throw new System.SystemException("This method should never be called.");
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