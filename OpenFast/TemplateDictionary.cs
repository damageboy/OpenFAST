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
using Group = OpenFAST.Template.Group;
using System.Text;

namespace OpenFAST
{
	public sealed class TemplateDictionary : Dictionary
	{
        internal System.Collections.Generic.Dictionary<Group, System.Collections.Generic.Dictionary<QName, ScalarValue>> table = new System.Collections.Generic.Dictionary<Group, System.Collections.Generic.Dictionary<QName, ScalarValue>>();
		
		public ScalarValue Lookup(Group template, QName key, QName applicationType)
		{
		    if (table.ContainsKey(template))
		    {
		        return (table[template]).ContainsKey(key) ? table[template][key] : ScalarValue.UNDEFINED;
		    }
		    return ScalarValue.UNDEFINED;
		}
		
		public void  Reset()
		{
			table.Clear();
		}
		
		public void  Store(Group group, QName applicationType, QName key, ScalarValue valueToEncode)
		{
			if (!table.ContainsKey(group))
			{
				table[group] = new System.Collections.Generic.Dictionary<QName,ScalarValue>();
			}
			
			table[group][key] = valueToEncode;
		}
		
		public override string ToString()
		{
			var builder = new StringBuilder();
            foreach (var template in table.Keys)
            {
                builder.Append("Dictionary: Template=" + template);
                System.Collections.IDictionary templateMap = table[template];
                System.Collections.IEnumerator keyIterator = new SupportClass.HashSetSupport(templateMap.Keys).GetEnumerator();
                while (keyIterator.MoveNext())
                {
                    var key = keyIterator.Current;
                    builder.Append(key).Append("=").Append(templateMap[key]).Append("\n");
                }
            }
			return builder.ToString();
		}
	}
}