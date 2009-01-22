using System;
using DecimalValue = OpenFAST.DecimalValue;
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;

namespace OpenFAST.Template.Type
{
	[Serializable]
	sealed class DecimalType:SimpleType
	{
		override public ScalarValue DefaultValue
		{
			get
			{
				return new DecimalValue(0.0);
			}
			
		}
		private const long serialVersionUID = 1L;
		
		internal DecimalType():base("decimal", TypeCodec.SF_SCALED_NUMBER, TypeCodec.NULLABLE_SF_SCALED_NUMBER)
		{
		}
		
		public override TypeCodec GetCodec(Operator operator_Renamed, bool optional)
		{
			return base.GetCodec(operator_Renamed, optional);
		}

        public override ScalarValue GetVal(string value_Renamed)
		{
			try
			{
				return new DecimalValue(System.Double.Parse(value_Renamed));
			}
			catch (System.FormatException)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.S3_INITIAL_VALUE_INCOMP, "The value \"" + value_Renamed + "\" is not compatible with type " + this);
				return null;
			}
		}
		
		public override bool IsValueOf(ScalarValue previousValue)
		{
			return previousValue is DecimalValue;
		}
	}
}