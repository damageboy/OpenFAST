using System;
using Group = OpenFAST.Template.Group;
using System.Text;

namespace OpenFAST
{
	public sealed class TemplateDictionary : Dictionary
	{
        internal System.Collections.Generic.Dictionary<Group, System.Collections.Generic.Dictionary<QName, ScalarValue>> table = new System.Collections.Generic.Dictionary<Group, System.Collections.Generic.Dictionary<QName, ScalarValue>>();
		
		public ScalarValue Lookup(Group template, QName key, QName applicationType)
		{
			if (!table.ContainsKey(template))
			{
				return ScalarValue.UNDEFINED;
			}
			
			if ((table[template]).ContainsKey(key))
			{
				return table[template][key];
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
			StringBuilder builder = new StringBuilder();
            foreach (Group template in table.Keys)
            {
                builder.Append("Dictionary: Template=" + template.ToString());
                System.Collections.IDictionary templateMap = (System.Collections.IDictionary)table[template];
                System.Collections.IEnumerator keyIterator = new SupportClass.HashSetSupport(templateMap.Keys).GetEnumerator();
                while (keyIterator.MoveNext())
                {
                    System.Object key = keyIterator.Current;
                    builder.Append(key).Append("=").Append(templateMap[key]).Append("\n");
                }
            }
			return builder.ToString();
		}
	}
}