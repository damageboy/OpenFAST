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

	    private readonly MessageTemplate template;
		
		public Message(MessageTemplate template, FieldValue[] fieldValues):base(template, fieldValues)
		{
			this.template = template;
		}
		public Message(MessageTemplate template):this(template, InitializeFieldValues(template.FieldCount))
		{
		}
		private static FieldValue[] InitializeFieldValues(int fieldCount)
		{
			var fields = new FieldValue[fieldCount];
			return fields;
		}
		public  override bool Equals(object obj)
		{
			if ((obj == null) || !(obj is Message))
			{
				return false;
			}
			return equals((Message) obj);
		}
		public virtual bool equals(Message message)
		{
			if (FieldCount != message.FieldCount)
				return false;
			for (int i = 1; i < message.FieldCount; i++)
				if (message.GetValue(i) == null)
				{
				    if (GetValue(i) == null)
					{
						continue;
					}
				    return false;
				}
				else if (!message.GetValue(i).Equals(GetValue(i)))
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
			var copies = new FieldValue[values.Length];
			for (int i = 0; i < copies.Length; i++)
			{
				copies[i] = values[i].Copy();
			}
			return new Message(template, values);
		}
	}
}