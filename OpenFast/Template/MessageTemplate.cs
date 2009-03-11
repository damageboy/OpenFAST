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
using FastException = OpenFAST.Error.FastException;
using Operator = openfast.Template.Operator.Operator;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template
{
	
	[Serializable]
	public sealed class MessageTemplate:Group, FieldSet
	{

		public new static System.Type ValueType
		{
			get
			{
				return typeof(Message);
			}
			
		}

		public Field[] TemplateFields
		{
			get
			{
				var f = new Field[fields.Length - 1];
				Array.Copy(fields, 1, f, 0, fields.Length - 1);
				return f;
			}
			
		}

	    public MessageTemplate(QName name, Field[] fields):base(name, AddTemplateIdField(fields), false)
		{
		}
		public override bool UsesPresenceMap()
		{
			return true;
		}
		public MessageTemplate(string name, Field[] fields):this(new QName(name), fields)
		{
		}

		private static Field[] AddTemplateIdField(Field[] fields)
		{
			var newFields = new Field[fields.Length + 1];
			newFields[0] = new Scalar("templateId", FASTType.U32, Operator.COPY, ScalarValue.UNDEFINED, false);
			Array.Copy(fields, 0, newFields, 1, fields.Length);
			return newFields;
		}

		public override Field GetField(int index)
		{
			return fields[index];
		}

		public byte[] Encode(Message message, Context context)
		{
			if (!context.TemplateRegistry.IsRegistered(message.Template))
			{
				throw new FastException("Cannot encode message: The template " + message.Template + " has not been registered.", Error.FastConstants.D9_TEMPLATE_NOT_REGISTERED);
			}
			message.SetInteger(0, context.GetTemplateId(message.Template));
			return Encode(message, this, context);
		}

		public Message Decode(System.IO.Stream in_Renamed, int templateId, BitVectorReader presenceMapReader, Context context)
		{
			try
			{
				if (context.TraceEnabled)
					context.DecodeTrace.GroupStart(this);
				FieldValue[] fieldValues = DecodeFieldValues(in_Renamed, this, presenceMapReader, context);
				fieldValues[0] = new IntegerValue(templateId);
				var message = new Message(this, fieldValues);
				if (context.TraceEnabled)
					context.DecodeTrace.GroupEnd();
				return message;
			}
			catch (FastException e)
			{
				throw new FastException("An error occurred while decoding " + this, e.Code, e);
			}
		}
		public override string ToString()
		{
			return name.Name;
		}

		public override FieldValue CreateValue(string value_Renamed)
		{
			return new Message(this);
		}
		public  override bool Equals(Object obj)
		{
			if (obj == this)
				return true;
			if (obj == null || !(obj is MessageTemplate))
				return false;
			return Equals((MessageTemplate) obj);
		}

		public bool Equals(MessageTemplate other)
		{
			if (!name.Equals(other.name))
				return false;
			if (fields.Length != other.fields.Length)
				return false;
			for (int i = 0; i < fields.Length; i++)
			{
				if (!fields[i].Equals(other.fields[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode()
		{
			int hashCode = (name != null)?name.GetHashCode():0;
			for (int i = 0; i < fields.Length; i++)
				hashCode += fields[i].GetHashCode();
			return hashCode;
		}
	}
}