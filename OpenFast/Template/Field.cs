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

namespace OpenFAST.Template
{
	
	[Serializable]
	public abstract class Field
	{

		virtual public string Name
		{
			get
			{
				return name.Name;
			}
			
		}
		virtual public QName QName
		{
			get
			{
				return name;
			}
			
		}

		virtual public bool Optional
		{
			get
			{
				return optional;
			}
			
		}
		
		virtual public QName Key
		{
			get
			{
				return key;
			}
			
			set
			{
				key = value;
			}
			
		}
		
		virtual public string Id
		{
			get
			{
                if (id == null)
                    return "";
				return id;
			}
			
			set
			{
				id = value;
			}
			
		}
		public abstract System.Type ValueType{get;}
		public abstract string TypeName{get;}
		protected internal QName name;
		protected internal QName key;
		protected internal bool optional;
		protected internal string id;
		private System.Collections.Generic.Dictionary<QName,string> attributes;

	    protected Field(QName name, bool optional)
		{
			this.name = name;
			key = name;
			this.optional = optional;
		}


	    protected Field(QName name, QName key, bool optional)
		{
			this.name = name;
			this.key = key;
			this.optional = optional;
		}


	    protected Field(string name, string key, bool optional, string id)
		{
			this.name = new QName(name);
			this.key = new QName(key);
			this.optional = optional;
			this.id = id;
		}
		
		public virtual bool HasAttribute(QName attributeName)
		{
			return attributes != null && attributes.ContainsKey(attributeName);
		}
		
		public virtual void  AddAttribute(QName qname, string value_Renamed)
		{
			if (attributes == null)
			{
				attributes = new System.Collections.Generic.Dictionary<QName,string>();
			}
			attributes[qname] = value_Renamed;
		}
		
		public virtual string GetAttribute(QName qname)
		{
			return attributes[qname];
		}
		
		protected internal virtual bool IsPresent(BitVectorReader presenceMapReader)
		{
			return (!UsesPresenceMapBit()) || presenceMapReader.Read();
		}
		
		public abstract byte[] Encode(FieldValue value_Renamed, Group encodeTemplate, Context context, BitVectorBuilder presenceMapBuilder);
		
		public abstract FieldValue Decode(System.IO.Stream in_Renamed, Group decodeTemplate, Context context, BitVectorReader presenceMapReader);
		
		public abstract bool UsesPresenceMapBit();
		
		public abstract bool IsPresenceMapBitSet(byte[] encoding, FieldValue fieldValue);
		
		public abstract FieldValue CreateValue(string value_Renamed);
	}
}