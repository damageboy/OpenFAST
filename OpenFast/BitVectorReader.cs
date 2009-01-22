using System;

namespace OpenFAST
{
	public class BitVectorReader
	{
		public sealed class NullBitVectorReader:BitVectorReader
		{
			internal NullBitVectorReader(OpenFAST.BitVector Param1):base(Param1)
			{
			}
			public override bool Read()
			{
				throw new System.SystemException();
			}
			
			public override bool HasMoreBitsSet()
			{
				return false;
			}
		}
		public sealed class InfiniteBitVectorReader:BitVectorReader
		{
			internal InfiniteBitVectorReader(OpenFAST.BitVector Param1):base(Param1)
			{
			}
			public override bool Read()
			{
				return true;
			}
		}
		virtual public BitVector BitVector
		{
			get
			{
				return vector;
			}
			
		}
		virtual public int Index
		{
			get
			{
				return index;
			}
			
		}
		
		public static readonly BitVectorReader NULL;
		
		public static readonly BitVectorReader INFINITE_TRUE;
		
		private BitVector vector;
		private int index = 0;
		
		public BitVectorReader(BitVector vector)
		{
			this.vector = vector;
		}
		
		public virtual bool Read()
		{
			return vector.IsSet(index++);
		}
		
		public virtual bool HasMoreBitsSet()
		{
			return vector.IndexOfLastSet() > index;
		}
		
		public override string ToString()
		{
			return vector.ToString();
		}
		
		public virtual bool Peek()
		{
			return vector.IsSet(index);
		}
		static BitVectorReader()
		{
			NULL = new NullBitVectorReader(null);
			INFINITE_TRUE = new InfiniteBitVectorReader(null);
		}
	}
}