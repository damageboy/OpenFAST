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