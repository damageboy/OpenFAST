using System;
using BitVectorBuilder = OpenFAST.BitVectorBuilder;
using BitVectorReader = OpenFAST.BitVectorReader;
using Context = OpenFAST.Context;
using FieldValue = OpenFAST.FieldValue;
using QName = OpenFAST.QName;

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
				this.key = value;
			}
			
		}
		
		virtual public string Id
		{
			get
			{
                if (id == null)//OVERLOOK
                    return "";
				return id;
			}
			
			set
			{
				this.id = value;
			}
			
		}
		public abstract System.Type ValueType{get;}
		public abstract string TypeName{get;}
		protected internal QName name;
		protected internal QName key;
		protected internal bool optional;
		protected internal string id;
		private System.Collections.Generic.Dictionary<QName,string> attributes;
		
		public Field(QName name, bool optional)
		{
			this.name = name;
			this.key = name;
			this.optional = optional;
		}
		
		
		public Field(QName name, QName key, bool optional)
		{
			this.name = name;
			this.key = key;
			this.optional = optional;
		}
		
		
		public Field(string name, string key, bool optional, string id)
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
		
		public virtual void  AddAttribute(QName name, string value_Renamed)
		{
			if (attributes == null)
			{
				attributes = new System.Collections.Generic.Dictionary<QName,string>();
			}
			attributes[name] = value_Renamed;
		}
		
		public virtual string GetAttribute(QName name)
		{
			return (string) attributes[name];
		}
		
		protected internal virtual bool IsPresent(BitVectorReader presenceMapReader)
		{
			return (!UsesPresenceMapBit()) || presenceMapReader.Read();
		}
		
		public abstract sbyte[] Encode(FieldValue value_Renamed, Group template, Context context, BitVectorBuilder presenceMapBuilder);
		
		public abstract FieldValue Decode(System.IO.Stream in_Renamed, Group template, Context context, BitVectorReader presenceMapReader);
		
		public abstract bool UsesPresenceMapBit();
		
		public abstract bool IsPresenceMapBitSet(sbyte[] encoding, FieldValue fieldValue);
		
		public abstract FieldValue CreateValue(string value_Renamed);
	}
}