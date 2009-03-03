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
using OpenFAST.Template.Type;

namespace OpenFAST
{
	[Serializable]
	public sealed class IntegerValue:NumericValue
	{
		private const long serialVersionUID = 1L;

        public int value_Renamed;
		
		public IntegerValue(int value_Renamed)
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
			return value_Renamed;
		}
		
		public override bool EqualsValue(string defaultValue)
		{
			return System.Int32.Parse(defaultValue) == value_Renamed;
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
				return new LongValue(this.value_Renamed - subend.ToLong());
			}
			
			return new IntegerValue(this.value_Renamed - subend.ToInt());
		}
		
		public override NumericValue Add(NumericValue addend)
		{
			if (addend is LongValue)
			{
				return addend.Add(this);
			}
			
			return new IntegerValue(this.value_Renamed + addend.ToInt());
		}
		
		public string Serialize()
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
			return value_Renamed;
		}
		
		public override string ToString()
		{
			return System.Convert.ToString(value_Renamed);
		}
		
		public override byte ToByte()
		{
			if (value_Renamed > System.SByte.MaxValue || value_Renamed <  System.SByte.MinValue)
				Global.HandleError(OpenFAST.Error.FastConstants.R4_NUMERIC_VALUE_TOO_LARGE, "The value \"" + value_Renamed + "\" is too large for a byte.");
			return (byte) value_Renamed;
		}
		
		public override short ToShort()
		{
			if (value_Renamed > System.Int16.MaxValue || value_Renamed < System.Int16.MinValue)
				Global.HandleError(OpenFAST.Error.FastConstants.R4_NUMERIC_VALUE_TOO_LARGE, "The value \"" + value_Renamed + "\" is too large for a short.");
			return (short) value_Renamed;
		}
		
		public override double ToDouble()
		{
			return value_Renamed;
		}
		
		public override System.Decimal ToBigDecimal()
		{
			return new System.Decimal(value_Renamed);
		}
	}
}