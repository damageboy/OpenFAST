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
using Group = OpenFAST.Template.Group;
using System.Text;

namespace OpenFAST
{
	public sealed class GlobalDictionary : Dictionary
	{
        internal System.Collections.Generic.Dictionary<QName, ScalarValue> table = new System.Collections.Generic.Dictionary<QName, ScalarValue>();
		
		public ScalarValue Lookup(Group template, QName key, QName applicationType)
		{
			if (!table.ContainsKey(key))
			{
				return ScalarValue.UNDEFINED;
			}
			
			return  table[key];
		}
		
		public void  Store(Group group, QName applicationType, QName key, ScalarValue value_Renamed)
		{
			table[key] = value_Renamed;
		}
		
		public void  Reset()
		{
			table.Clear();
		}
		
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			System.Collections.IEnumerator keyIterator = new SupportClass.HashSetSupport(table.Keys).GetEnumerator();
			while (keyIterator.MoveNext())
			{
				QName key = (QName) keyIterator.Current;
				builder.Append("Dictionary: Global");
				builder.Append(key).Append("=").Append(table[key]).Append("\n");
			}
			return builder.ToString();
		}
	}
}