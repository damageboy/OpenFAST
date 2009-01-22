using System;
using ScalarValue = OpenFAST.ScalarValue;

namespace OpenFAST.Template
{
	
	[Serializable]
	public class TwinValue:ScalarValue
	{
		private const long serialVersionUID = 1L;
		public ScalarValue first;
		public ScalarValue second;
		
		public TwinValue(ScalarValue first, ScalarValue second)
		{
			this.first = first;
			this.second = second;
		}
		
		public  override bool Equals(System.Object obj)
		{
			if ((obj == null) || !(obj is TwinValue))
			{
				return false;
			}
			
			return Equals((TwinValue) obj);
		}

		private bool Equals(TwinValue other)
		{
			return (first.Equals(other.first) && second.Equals(other.second));
		}
		
		public override int GetHashCode()
		{
			return first.GetHashCode() * 37 + second.GetHashCode();
		}
		
		public override string ToString()
		{
			return first.ToString() + ", " + second.ToString();
		}
	}
}