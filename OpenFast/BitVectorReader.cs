/*

The contents of this file are subject to the Mozilla Public License
Version 1.1 (the "License"); you may not use this file except in
compliance with the License. You may obtain a copy of the License at
http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS"
basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
License for the specific language governing rights and limitations
under the License.

The Original Code is OpenFAST.

The Initial Developer of the Original Code is The LaSalle Technology
Group, LLC.  Portions created by Shariq Muhammad
are Copyright (C) Shariq Muhammad. All Rights Reserved.

Contributor(s): Shariq Muhammad <shariq.muhammad@gmail.com>

*/
namespace OpenFAST
{
	public class BitVectorReader
	{
		public sealed class NullBitVectorReader:BitVectorReader
		{
			internal NullBitVectorReader(BitVector Param1):base(Param1)
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
			internal InfiniteBitVectorReader(BitVector Param1):base(Param1)
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
		
		private readonly BitVector vector;
		private int index;
		
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