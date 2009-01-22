using System;
using DecimalValue = OpenFAST.DecimalValue;
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using Scalar = OpenFAST.Template.Scalar;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.operator_Renamed
{
	[Serializable]
	public sealed class DeltaDecimalOperatorCodec:AlwaysPresentOperatorCodec
	{
		private const long serialVersionUID = 1L;
		
		internal DeltaDecimalOperatorCodec():base(Operator.DELTA, new FASTType[]{FASTType.DECIMAL})
		{
		}
		
		public override ScalarValue GetValueToEncode(ScalarValue val, ScalarValue priorVal, Scalar field)
		{
			if (priorVal == null)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field " + field + " must have a priorValue defined.");
				return null;
			}
			
			if (val == null)
			{
				if (field.Optional)
				{
					return ScalarValue.NULL;
				}
				else
				{
					Global.HandleError(OpenFAST.Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "");
					return null;
				}
			}
			
			if (priorVal.Undefined && field.DefaultValue.Undefined)
			{
				return val;
			}
			
			DecimalValue priorValue = priorVal.Undefined?(DecimalValue) field.DefaultValue:(DecimalValue) priorVal;
			DecimalValue value_Renamed = (DecimalValue) val;
			
			return new DecimalValue(value_Renamed.mantissa - priorValue.mantissa, value_Renamed.exponent - priorValue.exponent);
		}
		
		public override ScalarValue DecodeValue(ScalarValue val, ScalarValue priorVal, Scalar field)
		{
			if (priorVal == null)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field " + field + " must have a priorValue defined.");
				return null;
			}
			
			if (val == null)
			{
				return null;
			}
			
			DecimalValue priorValue = null;
			
			if (priorVal.Undefined)
			{
				if (field.DefaultValue.Undefined)
				{
					priorValue = (DecimalValue) field.BaseValue;
				}
				else if (val == null)
				{
					if (field.Optional)
					{
						return ScalarValue.NULL;
					}
					else
					{
						throw new System.SystemException("Field cannot be null.");
					}
				}
				else
				{
					priorValue = (DecimalValue) field.DefaultValue;
				}
			}
			else
			{
				priorValue = (DecimalValue) priorVal;
			}
			
			DecimalValue value_Renamed = (DecimalValue) val;
			
			return new DecimalValue(value_Renamed.mantissa + priorValue.mantissa, value_Renamed.exponent + priorValue.exponent);
		}
		
		public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
		{
			if (field.DefaultValue.Undefined)
			{
				if (field.Optional)
				{
					return ScalarValue.NULL;
				}
				else if (previousValue.Undefined)
				{
					throw new System.SystemException("Mandatory fields without a previous value or default value must be present.");
				}
				else
				{
					return previousValue;
				}
			}
			else
			{
				return field.DefaultValue;
			}
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