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
	public sealed class ByteVectorValue:ScalarValue
	{
		override public byte[] Bytes
		{
			get
			{
				return value_Renamed;
			}
			
		}
		private const long serialVersionUID = 1L;

		public byte[] value_Renamed;
		
		public ByteVectorValue(byte[] value_Renamed)
		{
			this.value_Renamed = value_Renamed;
		}
		
		public override string ToString()
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder(value_Renamed.Length * 2);
			for (int i = 0; i < value_Renamed.Length; i++)
			{
				string hex = System.Convert.ToString(value_Renamed[i], 16);
				if (hex.Length == 1)
					builder.Append('0');
				builder.Append(hex);
			}
			return builder.ToString();
		}
		
		public  override bool Equals(System.Object obj)
		{
			if ((obj == null) || !(obj is ByteVectorValue))
			{
				return false;
			}
			
			return Equals((ByteVectorValue) obj);
		}
		
		public bool Equals(ByteVectorValue other)
		{
			if (this.value_Renamed.Length != other.value_Renamed.Length)
			{
				return false;
			}
			
			for (int i = 0; i < this.value_Renamed.Length; i++)
				if (this.value_Renamed[i] != other.value_Renamed[i])
				{
					return false;
				}
			
			return true;
		}
		
		public override int GetHashCode()
		{
			return value_Renamed.GetHashCode();
		}
	}
}