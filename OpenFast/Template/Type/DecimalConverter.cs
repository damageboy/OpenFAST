using System;
using DecimalValue = OpenFAST.DecimalValue;
using FieldValue = OpenFAST.FieldValue;
using IntegerValue = OpenFAST.IntegerValue;
using ScalarValue = OpenFAST.ScalarValue;
using ComposedValueConverter = OpenFAST.Template.ComposedValueConverter;
using LongValue = OpenFAST.Template.LongValue;

namespace OpenFAST.Template.Type
{
	[Serializable]
	public class DecimalConverter : ComposedValueConverter
	{
		
		private static readonly FieldValue[] NULL_SET = new FieldValue[]{null, null};
		
		private static readonly FieldValue[] UNDEFINED_SET = new FieldValue[]{ScalarValue.UNDEFINED, ScalarValue.UNDEFINED};
		
		public virtual FieldValue[] Split(FieldValue value_Renamed)
		{
			if (value_Renamed == null)
				return NULL_SET;
			else if (value_Renamed == ScalarValue.UNDEFINED)
				return UNDEFINED_SET;
			DecimalValue decimal_Renamed = (DecimalValue) value_Renamed;
			return new FieldValue[]{new IntegerValue(decimal_Renamed.exponent), new LongValue(decimal_Renamed.mantissa)};
		}
		
		public virtual FieldValue Compose(FieldValue[] values)
		{
			if (values[0] == null)
				return null;
			if (values[0] == ScalarValue.UNDEFINED)
				return ScalarValue.UNDEFINED;
			return new DecimalValue(((ScalarValue) values[1]).ToLong(), ((IntegerValue) values[0]).value_Renamed);
		}
	}
}