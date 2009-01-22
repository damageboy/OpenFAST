using System;
using Group = OpenFAST.Template.Group;
using System.Text;

namespace OpenFAST
{
	
	//Optimized By SHARIQ
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