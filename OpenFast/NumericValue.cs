using System;

namespace OpenFAST
{
	
	[Serializable]
	public abstract class NumericValue:ScalarValue
	{
		public abstract NumericValue Increment();
		
		public abstract NumericValue Decrement();
		
		public abstract NumericValue Subtract(NumericValue priorValue);
		
		public abstract NumericValue Add(NumericValue addend);
		
		public abstract bool Equals(int value_Renamed);
		
		public abstract override long ToLong();
		
		public abstract override int ToInt();
	}
}