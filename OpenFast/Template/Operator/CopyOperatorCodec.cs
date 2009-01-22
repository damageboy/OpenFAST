using System;
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using Scalar = OpenFAST.Template.Scalar;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.operator_Renamed
{
	[Serializable]
	public sealed class CopyOperatorCodec:OptionallyPresentOperatorCodec
	{
		private const long serialVersionUID = 1L;
		
		internal CopyOperatorCodec():base(Operator.COPY, FASTType.ALL_TYPES())
		{
		}
		
		protected internal override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, ScalarValue defaultValue)
		{
			if ((priorValue == ScalarValue.UNDEFINED) && value_Renamed.Equals(defaultValue))
			{
				return null;
			}
			
			return (value_Renamed.Equals(priorValue))?null:value_Renamed;
		}
		
		protected internal override ScalarValue GetInitialValue(Scalar field)
		{
			if (!field.DefaultValue.Undefined)
			{
				return field.DefaultValue;
			}
			
			if (field.Optional)
			{
				return null;
			}
			
			Global.HandleError(OpenFAST.Error.FastConstants.D5_NO_DEFAULT_VALUE, "No default value for " + field);
			
			return null;
		}

		protected internal override ScalarValue GetEmptyValue(ScalarValue priorValue)
		{
			return priorValue;
		}
		
		public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue priorValue, Scalar field)
		{
			return newValue;
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