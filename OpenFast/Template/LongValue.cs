using System;
using NumericValue = OpenFAST.NumericValue;

namespace OpenFAST.Template
{
	[Serializable]
	public class LongValue:NumericValue
	{
		private const long serialVersionUID = 1L;
		public long value_Renamed;
		
		public LongValue(long value_Renamed)
		{
			this.value_Renamed = value_Renamed;
		}
		
		public  override bool Equals(System.Object obj)
		{
			if ((obj == null) || !(obj is NumericValue))
			{
				return false;
			}
			
			return Equals((NumericValue) obj);
		}
		
		private bool Equals(NumericValue otherValue)
		{
			return value_Renamed == otherValue.ToLong();
		}
		
		public override int GetHashCode()
		{
			return (int) value_Renamed;
		}
		
		public override bool EqualsValue(string defaultValue)
		{
			return System.Int32.Parse(defaultValue) == value_Renamed;
		}
		
		public override NumericValue Increment()
		{
			return new LongValue(value_Renamed + 1);
		}
		
		public override NumericValue Decrement()
		{
			return new LongValue(value_Renamed - 1);
		}
		
		public override string ToString()
		{
			return System.Convert.ToString(value_Renamed);
		}
		
		public override NumericValue Subtract(NumericValue subend)
		{
			return new LongValue(this.value_Renamed - subend.ToLong());
		}
		
		public override NumericValue Add(NumericValue addend)
		{
			return new LongValue(this.value_Renamed + addend.ToLong());
		}
		
		public virtual string Serialize()
		{
			return System.Convert.ToString(value_Renamed);
		}
		
		public override bool Equals(int value_Renamed)
		{
			return value_Renamed == this.value_Renamed;
		}
		
		public override long ToLong()
		{
			return value_Renamed;
		}
		
		public override int ToInt()
		{
			return (int) value_Renamed;
		}
	}
}