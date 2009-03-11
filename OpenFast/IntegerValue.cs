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
using LongValue = OpenFAST.Template.LongValue;

namespace OpenFAST
{
	[Serializable]
	public sealed class IntegerValue:NumericValue
	{
	    public int value_Renamed;
		
		public IntegerValue(int value_Renamed)
		{
			this.value_Renamed = value_Renamed;
		}
		
		public  override bool Equals(object obj)
		{
			if ((obj == null) || !(obj is NumericValue))
			{
				return false;
			}

            return Equals((ScalarValue)obj);
		}
		
		private bool Equals(ScalarValue otherValue)
		{
			return value_Renamed == otherValue.ToLong();
		}
		
		public override int GetHashCode()
		{
			return value_Renamed;
		}
		
		public override bool EqualsValue(string defaultValue)
		{
			return Int32.Parse(defaultValue) == value_Renamed;
		}
		
		public override NumericValue Increment()
		{
			return new IntegerValue(value_Renamed + 1);
		}
		
		public override NumericValue Decrement()
		{
			return new IntegerValue(value_Renamed - 1);
		}
		
		public override NumericValue Subtract(NumericValue subend)
		{
			if (subend is LongValue)
			{
				return new LongValue(value_Renamed - subend.ToLong());
			}
			
			return new IntegerValue(value_Renamed - subend.ToInt());
		}
		
		public override NumericValue Add(NumericValue addend)
		{
			if (addend is LongValue)
			{
				return addend.Add(this);
			}
			
			return new IntegerValue(value_Renamed + addend.ToInt());
		}
		
		public string Serialize()
		{
			return Convert.ToString(value_Renamed);
		}
		
		public override bool Equals(int valueRenamed)
		{
			return valueRenamed == value_Renamed;
		}
		
		public override long ToLong()
		{
			return value_Renamed;
		}
		
		public override int ToInt()
		{
			return value_Renamed;
		}
		
		public override string ToString()
		{
			return Convert.ToString(value_Renamed);
		}
		
		public override byte ToByte()
		{
			if (value_Renamed > SByte.MaxValue || value_Renamed <  SByte.MinValue)
				Global.HandleError(Error.FastConstants.R4_NUMERIC_VALUE_TOO_LARGE, "The value \"" + value_Renamed + "\" is too large for a byte.");
			return (byte) value_Renamed;
		}
		
		public override short ToShort()
		{
			if (value_Renamed > Int16.MaxValue || value_Renamed < Int16.MinValue)
				Global.HandleError(Error.FastConstants.R4_NUMERIC_VALUE_TOO_LARGE, "The value \"" + value_Renamed + "\" is too large for a short.");
			return (short) value_Renamed;
		}
		
		public override double ToDouble()
		{
			return value_Renamed;
		}
		
		public override Decimal ToBigDecimal()
		{
			return new Decimal(value_Renamed);
		}
	}
}