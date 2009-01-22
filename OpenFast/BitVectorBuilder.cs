using System;

namespace OpenFAST
{
	public sealed class BitVectorBuilder
	{
		public BitVector BitVector
		{
			get
			{
				return vector;
			}
			
		}
		public System.Object OnValueSkipOnNull
		{
			set
			{
				if (value == null)
					Skip();
				else
					set_Renamed();
			}
			
		}
		public int Index
		{
			get
			{
				return index;
			}
			
		}

        private BitVector vector;
		private int index = 0;
		
		public BitVectorBuilder(int size)
		{
			vector = new BitVector(size);
		}
		
		public void  set_Renamed()
		{
			vector.set_Renamed(index);
			index++;
		}
		
		public void  Skip()
		{
			index++;
		}
	}
}