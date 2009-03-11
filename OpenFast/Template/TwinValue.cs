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

namespace OpenFAST.Template
{
	
	[Serializable]
	public class TwinValue:ScalarValue
	{
	    public ScalarValue first;
		public ScalarValue second;
		
		public TwinValue(ScalarValue first, ScalarValue second)
		{
			this.first = first;
			this.second = second;
		}
		
		public  override bool Equals(Object obj)
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
			return first + ", " + second;
		}
	}
}