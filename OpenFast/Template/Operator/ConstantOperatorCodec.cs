using System;
using BitVectorBuilder = OpenFAST.BitVectorBuilder;
using FieldValue = OpenFAST.FieldValue;
using ScalarValue = OpenFAST.ScalarValue;
using Scalar = OpenFAST.Template.Scalar;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.operator_Renamed
{
	[Serializable]
	sealed class ConstantOperatorCodec:OperatorCodec
	{
		private const long serialVersionUID = 1L;
		
		internal ConstantOperatorCodec(Operator operator_Renamed, FASTType[] types):base(operator_Renamed, types)
		{
		}

		public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field, BitVectorBuilder presenceMapBuilder)
		{
			if (field.Optional)
				presenceMapBuilder.OnValueSkipOnNull = value_Renamed;
			return null; // Never encode constant value.
		}
		public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue, Scalar field)
		{
			return field.DefaultValue;
		}
		public override bool IsPresenceMapBitSet(byte[] encoding, FieldValue fieldValue)
		{
			return fieldValue != null;
		}
		public override bool ShouldDecodeType()
		{
			return false;
		}

		public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
		{
			if (!field.Optional)
			{
				return field.DefaultValue;
			}
			return null;
		}

		public override bool UsesPresenceMapBit(bool optional)
		{
			return optional;
		}
		public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field)
		{
			throw new System.NotSupportedException();
		}
		public override bool CanEncode(ScalarValue value_Renamed, Scalar field)
		{
			if (field.Optional && value_Renamed == null)
				return true;
			return field.DefaultValue.Equals(value_Renamed);
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