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
	/// <summary>
	/// Quick name.
    /// Provides efficient name searching in template registry. 
	/// </summary>
	[Serializable]
	public sealed class QName
	{
		public string Namespace
		{
			get
			{
				return namespace_Renamed;
			}
			
		}

        //must be less then or equal to 31 bytes
		public string Name
		{
			get
			{
				return name;
			}
			
		}

	    public static readonly QName NULL = new QName("", "");
		
		private readonly string namespace_Renamed;
		
		private readonly string name;
		
		public QName(string name):this(name, "")
		{
		}
		
		public QName(string name, string namespace_Renamed)
		{
			if (name == null)
				throw new NullReferenceException();
			this.name = name;
			this.namespace_Renamed = namespace_Renamed == null?"":namespace_Renamed;
		}
		
		public  override bool Equals(object obj)
		{
			if (obj == this)
				return true;
            if (obj == null || obj.GetType() != GetType())//POINTP
				return false;
			QName other = (QName) obj;
			return other.namespace_Renamed.Equals(namespace_Renamed) && other.name.Equals(name);
		}
		
		public override int GetHashCode()//efficient algo to quick search
		{
			return name.GetHashCode() + 31 * namespace_Renamed.GetHashCode();
		}
		
		public override string ToString()
		{
			if (namespace_Renamed.Equals(""))
				return name;
			return name + "[" + namespace_Renamed + "]";
		}
	}
}