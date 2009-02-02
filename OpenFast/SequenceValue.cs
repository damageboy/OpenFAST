using System;
using Sequence = OpenFAST.Template.Sequence;
using System.Text;
using System.Collections.Generic;

namespace OpenFAST
{
	[Serializable]
	public sealed class SequenceValue : FieldValue
	{
		public int Length
		{
			get
			{
				return elements.Count;
			}
			
		}
		public Sequence Sequence
		{
			get
			{
				return sequence;
			}
			
		}
		public GroupValue[] Values
		{
			get
			{
				return this.elements.ToArray();
			}
			
		}
		private const long serialVersionUID = 1L;
		private List<GroupValue> elements = new List<GroupValue>();
		private Sequence sequence;
		
		public SequenceValue(Sequence sequence)
		{
			if (sequence == null)
			{
				throw new System.NullReferenceException();
			}
			
			this.sequence = sequence;
		}
		
		public System.Collections.IEnumerator Iterator()
		{
			return elements.GetEnumerator();
		}
		
		public void  Add(GroupValue value_Renamed)
		{
			elements.Add(value_Renamed);
		}
		
		public void  Add(FieldValue[] values)
		{
			elements.Add(new GroupValue(sequence.Group, values));
		}
		
		public  override bool Equals(System.Object other)
		{
			if (other == this)
			{
				return true;
			}
			
			if ((other == null) || !(other is SequenceValue))
			{
				return false;
			}
			
			return equals((SequenceValue) other);
		}
		
		private bool equals(SequenceValue other)
		{
			if (Length != other.Length)
			{
				return false;
			}
			
			for (int i = 0; i < Length; i++)
			{
				if (!elements[i].Equals(other.elements[i]))
				{
					return false;
				}
			}
			
			return true;
		}
		
		public override int GetHashCode()
		{
			return elements.GetHashCode() * 37 + sequence.GetHashCode();
		}
		
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			System.Collections.IEnumerator iter = elements.GetEnumerator();
			builder.Append("[ ");
			
			while (iter.MoveNext())
			{
				GroupValue value_Renamed = (GroupValue) iter.Current;
				builder.Append('[').Append(value_Renamed).Append("] ");
			}
			
			builder.Append("]");
			
			return builder.ToString();
		}
		
		public GroupValue this[int index]
		{
            get
            {
                return (GroupValue)elements[index];
            }
		}
		
		public FieldValue Copy()
		{
			SequenceValue value_Renamed = new SequenceValue(this.sequence);
			for (int i = 0; i < elements.Count; i++)
			{
				value_Renamed.Add((GroupValue) ((GroupValue) elements[i]).Copy());
			}
			return value_Renamed;
		}
	}
}