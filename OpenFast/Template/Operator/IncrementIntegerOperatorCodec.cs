using System;
using NumericValue = OpenFAST.NumericValue;
using ScalarValue = OpenFAST.ScalarValue;
using Scalar = OpenFAST.Template.Scalar;
using IntegerType = OpenFAST.Template.Type.IntegerType;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.operator_Renamed
{
	[Serializable]
	sealed class IncrementIntegerOperatorCodec:OperatorCodec
	{
		private const long serialVersionUID = 1L;
		
		internal IncrementIntegerOperatorCodec(Operator operator_Renamed, FASTType[] types):base(operator_Renamed, types)
		{
		}
		public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field)
		{
			if (priorValue == null)
			{
				return value_Renamed;
			}
			if (value_Renamed == null)
			{
				if (field.Optional)
				{
					if (priorValue == ScalarValue.UNDEFINED && field.DefaultValue.Undefined)
					{
						return null;
					}
					return ScalarValue.NULL;
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
			if (priorValue.Undefined)
			{
				if (value_Renamed.Equals(field.DefaultValue))
				{
					return null;
				}
				else
				{
					return value_Renamed;
				}
			}
			if (!value_Renamed.Equals(((NumericValue) priorValue).Increment()))
			{
				return value_Renamed;
			}
			return null;
		}
		public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue, Scalar field)
		{
			return newValue;
		}
		public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
		{
			if (previousValue == null)
				return null;
			if (previousValue.Undefined)
			{
				if (field.DefaultValue.Undefined)
				{
					if (field.Optional)
					{
						return null;
					}
					else
					{
						throw new System.SystemException("Field with operator increment must send a value if no previous value existed.");
					}
				}
				else
				{
					return field.DefaultValue;
				}
			}
			return ((NumericValue) previousValue).Increment();
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