using System;

namespace OpenFAST
{
	
	[Serializable]
	public sealed class DateValue:ScalarValue
	{
		private const long serialVersionUID = 1L;
		public System.DateTime value_Renamed;
		
		public DateValue(ref System.DateTime date)
		{
			this.value_Renamed = date;
		}
		
		public override long ToLong()
		{
			return value_Renamed.Ticks;
		}
		
		public override string ToString()
		{
			return value_Renamed.ToString("r");
		}
		
		public  override bool Equals(System.Object other)
		{
			if (other == this)
				return true;
			if (other == null || !(other is DateValue))
				return false;
			return Equals((DateValue) other);
		}
		
		private bool Equals(DateValue other)
		{
			return other.value_Renamed.Equals(value_Renamed);
		}
		
		public override int GetHashCode()
		{
			return value_Renamed.GetHashCode();
		}
	}
}