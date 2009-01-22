using System;
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using Scalar = OpenFAST.Template.Scalar;
using TwinValue = OpenFAST.Template.TwinValue;
using FASTType = OpenFAST.Template.Type.FASTType;
using Util = OpenFAST.util.Util;

namespace OpenFAST.Template.operator_Renamed
{
	[Serializable]
	sealed class DeltaStringOperatorCodec:AlwaysPresentOperatorCodec
	{
		private const long serialVersionUID = 1L;
		
		internal DeltaStringOperatorCodec():base(Operator.DELTA, new FASTType[]{FASTType.ASCII, FASTType.STRING})
		{
		}
		
		public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field)
		{
			if (value_Renamed == null)
			{
				return ScalarValue.NULL;
			}
			
			if (priorValue == null)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field " + field + " must have a priorValue defined.");
				return null;
			}
			
			ScalarValue base_Renamed = (priorValue.Undefined)?field.BaseValue:priorValue;
			
			return Util.GetDifference((StringValue) value_Renamed, (StringValue) base_Renamed);
		}
		
		public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue, Scalar field)
		{
			if ((newValue == null) || newValue.Null)
			{
				return null;
			}
			
			TwinValue diffValue = (TwinValue) newValue;
			ScalarValue base_Renamed = (previousValue.Undefined)?field.BaseValue:previousValue;
			
			if (diffValue.first.ToInt() > base_Renamed.ToString().Length)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.D7_SUBTRCTN_LEN_LONG, "The string diff <" + diffValue + "> cannot be applied to the base value \"" + base_Renamed + "\" because the subtraction length is too long.");
			}
			return Util.ApplyDifference((StringValue) base_Renamed, diffValue);
		}
		
		public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
		{
			throw new System.SystemException("As of FAST v1.1 Delta values must be present in stream");
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