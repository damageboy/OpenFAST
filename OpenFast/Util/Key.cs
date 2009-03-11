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
namespace OpenFAST.util
{
	
	
	public sealed class Key
	{
		private readonly System.Object[] keys;
		
		public Key(System.Object key1, System.Object key2):this(new[]{key1, key2})
		{
		}
		
		public Key(System.Object key1, System.Object key2, System.Object key3):this(new[]{key1, key2, key3})
		{
		}
		
		public Key(System.Object[] keys)
		{
			this.keys = keys;
			CheckNull();
		}
		
		private void  CheckNull()
		{
			for (int i = 0; i < keys.Length; i++)
				if (keys[i] == null)
					throw new System.NullReferenceException();
		}
		
		public  override bool Equals(System.Object obj)
		{
			if (this == obj)
				return true;
			if ((obj == null) || !(obj is Key))
				return false;
			
			var other = ((Key) obj);
			if (other.keys.Length != keys.Length)
				return false;
			for (int i = 0; i < keys.Length; i++)
				if (!other.keys[i].Equals(keys[i]))
					return false;
			return true;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			for (int i = 0; i < keys.Length; i++)
				hashCode += keys[i].GetHashCode() * (37 ^ i);
			return hashCode;
		}
		
		public override string ToString()
		{
            return "";
		}
	}
}