using System;
using MessageTemplate = OpenFAST.Template.MessageTemplate;

namespace OpenFAST
{
	
	[Serializable]
	public class Message:GroupValue
	{
		override public int FieldCount
		{
			get
			{
				return values.Length;
			}
			
		}
		virtual public MessageTemplate Template
		{
			get
			{
				return template;
			}
			
		}
		private const long serialVersionUID = 1L;

        private MessageTemplate template;
		
		public Message(MessageTemplate template, FieldValue[] fieldValues):base(template, fieldValues)
		{
			this.template = template;
		}
		public Message(MessageTemplate template):this(template, InitializeFieldValues(template.FieldCount))
		{
		}
		private static FieldValue[] InitializeFieldValues(int fieldCount)
		{
			FieldValue[] fields = new FieldValue[fieldCount];
			return fields;
		}
		public  override bool Equals(System.Object obj)
		{
			if ((obj == null) || !(obj is Message))
			{
				return false;
			}
			return equals((Message) obj);
		}
		public virtual bool equals(Message message)
		{
			if (this.FieldCount != message.FieldCount)
				return false;
			for (int i = 1; i < message.FieldCount; i++)
				if (message.GetValue(i) == null)
				{
					if (this.GetValue(i) == null)
					{
						continue;
					}
					else
					{
						return false;
					}
				}
				else if (!message.GetValue(i).Equals(this.GetValue(i)))
				{
					return false;
				}
			return true;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode() + template.GetHashCode();
		}
		public override FieldValue Copy()
		{
			FieldValue[] copies = new FieldValue[values.Length];
			for (int i = 0; i < copies.Length; i++)
			{
				copies[i] = values[i].Copy();
			}
			return new Message(template, this.values);
		}
	}
}