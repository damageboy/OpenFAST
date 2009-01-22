using System;

namespace OpenFAST
{
	[Serializable]
	public class BitVectorValue:ScalarValue
	{
		private const long serialVersionUID = 1L;
		public BitVector value_Renamed;
		
		public BitVectorValue(BitVector value_Renamed)
		{
			this.value_Renamed = value_Renamed;
		}
		
		public  override bool Equals(System.Object obj)
		{
			if ((obj == null) || !(obj is BitVectorValue))
			{
				return false;
			}
			
			return Equals((BitVectorValue) obj);
		}
		
		public bool Equals(BitVectorValue other)
		{
			return other.value_Renamed.Equals(this.value_Renamed);
		}
		
		public override int GetHashCode()
		{
			return value_Renamed.GetHashCode();
		}
	}
}