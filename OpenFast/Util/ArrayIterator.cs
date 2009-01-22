using System;

namespace OpenFAST.util
{
	public sealed class ArrayIterator : System.Collections.IEnumerator
	{
		public System.Object Current
		{
			get
			{
				return array[position++];
			}
			
		}
		private int position;

		private System.Object[] array;
		
		public ArrayIterator(System.Object[] array)
		{
			this.array = array;
		}
		
		public bool MoveNext()
		{
			return position < array.Length;
		}

		public void  Remove()
		{
			throw new System.NotSupportedException();
		}

		public void  Reset()
		{
		}
	}
}