using System;
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using Scalar = OpenFAST.Template.Scalar;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.operator_Renamed
{
	[Serializable]
	public abstract class OptionallyPresentOperatorCodec:OperatorCodec
	{
		protected internal OptionallyPresentOperatorCodec(Operator operator_Renamed, FASTType[] types):base(operator_Renamed, types)
		{
		}
		
		public override ScalarValue DecodeEmptyValue(ScalarValue priorValue, Scalar field)
		{
			if (priorValue == ScalarValue.UNDEFINED)
			{
				return GetInitialValue(field);
			}
			
			return GetEmptyValue(priorValue);
		}
		
		public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field)
		{
			if (value_Renamed != null)
			{
				return GetValueToEncode(value_Renamed, priorValue, field.DefaultValue);
			}
			
			if (field.Optional)
			{
				if (((priorValue == ScalarValue.UNDEFINED) && !field.DefaultValue.Undefined) || ((priorValue != ScalarValue.UNDEFINED) && (priorValue != null)))
				{
					return ScalarValue.NULL;
				}
			}
			else
			{
				Global.HandleError(OpenFAST.Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field \"" + field + " is not present.");
			}
			
			return null;
		}
		
		protected internal abstract ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, ScalarValue defaultValue);
		
		protected internal abstract ScalarValue GetInitialValue(Scalar field);
		
		protected internal abstract ScalarValue GetEmptyValue(ScalarValue priorValue);
	}
}