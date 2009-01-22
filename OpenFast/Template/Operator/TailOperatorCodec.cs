using System;
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using StringValue = OpenFAST.StringValue;
using Scalar = OpenFAST.Template.Scalar;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.operator_Renamed
{
	[Serializable]
	sealed class TailOperatorCodec:OperatorCodec
	{
		private const long serialVersionUID = 1L;
		
		internal TailOperatorCodec(Operator operator_Renamed, FASTType[] types):base(operator_Renamed, types)
		{
		}
		
		public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field)
		{
			if (value_Renamed == null)
			{
				if (priorValue == null)
					return null;
				if (priorValue.Undefined && field.DefaultValue.Undefined)
					return null;
				return ScalarValue.NULL;
			}
			
			if (priorValue == null)
			{
				return value_Renamed;
			}
			
			if (priorValue.Undefined)
			{
				priorValue = field.BaseValue;
			}
			
			int index = 0;
			
			sbyte[] val = value_Renamed.Bytes;
			sbyte[] prior = priorValue.Bytes;
			
			if (val.Length > prior.Length)
				return value_Renamed;
			if (val.Length < prior.Length)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.D3_CANT_ENCODE_VALUE, "The value " + val + " cannot be encoded by a tail operator with previous value " + priorValue);
			}
			
			while (index < val.Length && val[index] == prior[index])
				index++;
			if (val.Length == index)
				return null;
			return (ScalarValue) field.CreateValue(new string(SupportClass.ToCharArray(val), index, val.Length - index));
		}
		
		public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue, Scalar field)
		{
			StringValue base_Renamed;
			
			if ((previousValue == null) && !field.Optional)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "");
				return null;
			}
			else if ((previousValue == null) || previousValue.Undefined)
			{
				base_Renamed = (StringValue) field.BaseValue;
			}
			else
			{
				base_Renamed = (StringValue) previousValue;
			}
			
			if ((newValue == null) || newValue.Null)
			{
				if (field.Optional)
				{
					return null;
				}
				else
				{
					throw new System.ArgumentException("");
				}
			}
			
			string delta = ((StringValue) newValue).value_Renamed;
			int length = System.Math.Max(base_Renamed.value_Renamed.Length - delta.Length, 0);
			string root = base_Renamed.value_Renamed.Substring(0, (length) - (0));
			
			return new StringValue(root + delta);
		}
		
		public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
		{
			ScalarValue value_Renamed = previousValue;
			if (value_Renamed != null && value_Renamed.Undefined)
				value_Renamed = (field.DefaultValue.Undefined)?null:field.DefaultValue;
			if (value_Renamed == null && !field.Optional)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field " + field + " was not present.");
			}
			return value_Renamed;
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