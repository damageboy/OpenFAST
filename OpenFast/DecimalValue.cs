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

namespace OpenFAST
{
	
	[Serializable]
	public sealed class DecimalValue:NumericValue
	{
		override public bool Null
		{
			get
			{
				return false;
			}
			
		}
		private long Value
		{
			get
			{
				return mantissa * ((long) System.Math.Pow(10, exponent));
			}
			
		}
		private const long serialVersionUID = 1L;
		
		public int exponent;

        public long mantissa;
		
		public DecimalValue(double value_Renamed)
		{
			if (value_Renamed == 0.0)
			{
				this.exponent = 0;
				this.mantissa = 0;
				
				return ;
			}
			
			System.Decimal decimalValue = (Decimal)(value_Renamed);
            int exponent = SupportClass.BigDecimal_Scale(decimalValue);
            long mantissa = SupportClass.BigDecimal_UnScaledValue(decimalValue);
			
			while (((mantissa % 10) == 0) && (mantissa != 0))
			{
				mantissa /= 10;
				exponent -= 1;
			}
			
			this.mantissa = mantissa;
			this.exponent = - exponent;
		}
		
		public DecimalValue(long mantissa, int exponent)
		{
			this.mantissa = mantissa;
			this.exponent = exponent;
		}
		
		public DecimalValue(System.Decimal bigDecimal)
		{
            this.mantissa = SupportClass.BigDecimal_UnScaledValue(bigDecimal);
            this.exponent = SupportClass.BigDecimal_Scale(bigDecimal);
		}
		
		public override NumericValue Increment()
		{
			return null;
		}
		
		public override NumericValue Decrement()
		{
			return null;
		}
		
		public  override bool Equals(System.Object obj)
		{
			if ((obj == null) || !(obj is DecimalValue))
			{
				return false;
			}
			
			return equals((DecimalValue) obj);
		}
		
		public bool equals(DecimalValue other)
		{
			return other.mantissa == this.mantissa && other.exponent == this.exponent;
		}
		
		public override NumericValue Subtract(NumericValue subtrahend)
		{
			return new DecimalValue(System.Decimal.Subtract(ToBigDecimal(), ((DecimalValue) subtrahend).ToBigDecimal()));
		}
		
		public override NumericValue Add(NumericValue addend)
		{
			return new DecimalValue(System.Decimal.Add(ToBigDecimal(), ((DecimalValue) addend).ToBigDecimal()));
		}
		
		public string Serialize()
		{
			return ToString();
		}
		
		public override bool Equals(int value_Renamed)
		{
			return false;
		}
		
		public override long ToLong()
		{
			if (exponent < 0)
				Global.HandleError(OpenFAST.Error.FastConstants.R5_DECIMAL_CANT_CONVERT_TO_INT, "");
			return (long) (Value);
		}
		
		public override int ToInt()
		{
			long value_Renamed = Value;
			if (exponent < 0 || (value_Renamed) > System.Int32.MaxValue)
				Global.HandleError(OpenFAST.Error.FastConstants.R5_DECIMAL_CANT_CONVERT_TO_INT, "");
			return (int) (value_Renamed);
		}
		
		public override short ToShort()
		{
			long value_Renamed = Value;
			if (exponent < 0 || (value_Renamed) > System.Int16.MaxValue)
				Global.HandleError(OpenFAST.Error.FastConstants.R5_DECIMAL_CANT_CONVERT_TO_INT, "");
			return (short) (value_Renamed);
		}
		
		public override byte ToByte()
		{
			long value_Renamed = Value;
			if (exponent < 0 || (value_Renamed) > (byte) System.SByte.MaxValue)
				Global.HandleError(OpenFAST.Error.FastConstants.R5_DECIMAL_CANT_CONVERT_TO_INT, "");
			return (byte) (value_Renamed);
		}
		
		public override double ToDouble()
		{
			return mantissa * System.Math.Pow(10.0, exponent);
		}
		
		public override System.Decimal ToBigDecimal()
		{
			return (decimal)((double)mantissa/System.Math.Pow(10, - exponent));
		}
		
		public override string ToString()
		{
			return ToBigDecimal().ToString();
		}
		
		public override int GetHashCode()
		{
			return exponent * 37 + (int) mantissa;
		}
	}
}