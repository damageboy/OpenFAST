using System;
using Group = OpenFAST.Template.Group;
using System.Text;

namespace OpenFAST
{
	public sealed class ApplicationTypeDictionary : Dictionary
	{
        private System.Collections.Generic.Dictionary<QName, System.Collections.Generic.Dictionary<QName, ScalarValue>> dictionary = new System.Collections.Generic.Dictionary<QName, System.Collections.Generic.Dictionary<QName, ScalarValue>>();
		
		public ScalarValue Lookup(Group template, QName key, QName applicationType)
		{
			if (dictionary.ContainsKey(template.TypeReference))
			{
				System.Collections.IDictionary applicationTypeMap = (System.Collections.IDictionary) dictionary[template.TypeReference];
				if (applicationTypeMap.Contains(key))
					return (ScalarValue) applicationTypeMap[key];
			}
			return ScalarValue.UNDEFINED;
		}
		
		public void  Reset()
		{
            dictionary = new System.Collections.Generic.Dictionary<QName, System.Collections.Generic.Dictionary<QName, ScalarValue>>();
		}
		
		public void  Store(Group group, QName applicationType, QName key, ScalarValue value_Renamed)
		{
			if (!dictionary.ContainsKey(group.TypeReference))
			{
                dictionary[group.TypeReference] = new System.Collections.Generic.Dictionary<QName, ScalarValue>();
			}
			dictionary[group.TypeReference][key]=value_Renamed;
		}
		
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
            foreach (QName type in dictionary.Keys)
            {
                builder.Append("Dictionary: Type=" + type.ToString());
                System.Collections.IDictionary templateMap = (System.Collections.IDictionary)dictionary[type];
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