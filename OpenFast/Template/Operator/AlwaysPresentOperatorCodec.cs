using System;
using BitVectorBuilder = OpenFAST.BitVectorBuilder;
using ScalarValue = OpenFAST.ScalarValue;
using Scalar = OpenFAST.Template.Scalar;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.operator_Renamed
{
	[Serializable]
	public abstract class AlwaysPresentOperatorCodec:OperatorCodec
	{
		protected internal AlwaysPresentOperatorCodec(Operator operator_Renamed, FASTType[] types):base(operator_Renamed, types)
		{
		}
		
		public override bool UsesPresenceMapBit(bool optional)
		{
			return false;
		}
		
		public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar scalar, BitVectorBuilder presenceMapBuilder)
		{
			return GetValueToEncode(value_Renamed, priorValue, scalar);
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