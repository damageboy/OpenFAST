using System;
using Global = OpenFAST.Global;
using NumericValue = OpenFAST.NumericValue;
using ScalarValue = OpenFAST.ScalarValue;
using Scalar = OpenFAST.Template.Scalar;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.operator_Renamed
{
	[Serializable]
	sealed class DeltaIntegerOperatorCodec:AlwaysPresentOperatorCodec
	{
		private const long serialVersionUID = 1L;
		
		internal DeltaIntegerOperatorCodec(Operator operator_Renamed, FASTType[] types):base(operator_Renamed, types)
		{
		}
		
		public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field)
		{
			if (priorValue == null)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field " + field + " must have a priorValue defined.");
				return null;
			}
			
			if (value_Renamed == null)
			{
				if (field.Optional)
				{
					return ScalarValue.NULL;
				}
				else
				{
					throw new System.ArgumentException("Mandatory fields can't be null.");
				}
			}
			
			if (priorValue.Undefined)
			{
				priorValue = field.BaseValue;
			}
			
			return ((NumericValue) value_Renamed).Subtract((NumericValue) priorValue);
		}
		
		public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue, Scalar field)
		{
			if (previousValue == null)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field " + field + " must have a priorValue defined.");
				return null;
			}
			
			if ((newValue == null) || newValue.Null)
			{
				return null;
			}
			
			if (previousValue.Undefined)
			{
				if (field.DefaultValue.Undefined)
				{
					previousValue = field.BaseValue;
				}
				else
				{
					previousValue = field.DefaultValue;
				}
			}
			
			return ((NumericValue) newValue).Add((NumericValue) previousValue);
		}
		
		public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
		{
			if (previousValue.Undefined)
			{
				if (field.DefaultValue.Undefined)
				{
					if (field.Optional)
					{
						return ScalarValue.UNDEFINED;
					}
					else
					{
						Global.HandleError(OpenFAST.Error.FastConstants.D5_NO_DEFAULT_VALUE, "");
					}
				}
				else
				{
					return field.DefaultValue;
				}
			}
			
			return previousValue;
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