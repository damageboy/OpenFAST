using System;
using Global = OpenFAST.Global;
using IntegerValue = OpenFAST.IntegerValue;
using ScalarValue = OpenFAST.ScalarValue;
using LongValue = OpenFAST.Template.LongValue;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;
using Util = OpenFAST.util.Util;

namespace OpenFAST.Template.Type
{
	[Serializable]
	public abstract class IntegerType:SimpleType
	{
		override public ScalarValue DefaultValue
		{
			get
			{
				return new IntegerValue(0);
			}
			
		}
		
		protected internal long minValue;
		protected internal long maxValue;
		
		public IntegerType(string typeName, long minValue, long maxValue, TypeCodec codec, TypeCodec nullableCodec):base(typeName, codec, nullableCodec)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
		}

        public override ScalarValue GetVal(string value_Renamed)
		{
			long longValue;
			try
			{
				longValue = System.Int64.Parse(value_Renamed);
			}
			catch (System.FormatException)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.S3_INITIAL_VALUE_INCOMP, "The value \"" + value_Renamed + "\" is not compatable with type " + this);
				return null;
			}
			if (Util.IsBiggerThanInt(longValue))
			{
				return new LongValue(longValue);
			}
			return new IntegerValue((int) longValue);
		}

		public override bool IsValueOf(ScalarValue previousValue)
		{
			return previousValue is IntegerValue || previousValue is LongValue;
		}

		public override void  ValidateValue(ScalarValue value_Renamed)
		{
			if (value_Renamed == null || value_Renamed.Undefined)
				return ;
			if (value_Renamed.ToLong() > maxValue || value_Renamed.ToLong() < minValue)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.D2_INT_OUT_OF_RANGE, "The value " + value_Renamed + " is out of range for type " + this);
			}
		}
	}
}