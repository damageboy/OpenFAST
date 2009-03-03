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