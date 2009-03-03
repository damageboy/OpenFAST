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
using BitVector = OpenFAST.BitVector;
using BitVectorBuilder = OpenFAST.BitVectorBuilder;
using BitVectorReader = OpenFAST.BitVectorReader;
using BitVectorValue = OpenFAST.BitVectorValue;
using Context = OpenFAST.Context;
using FieldValue = OpenFAST.FieldValue;
using Global = OpenFAST.Global;
using GroupValue = OpenFAST.GroupValue;
using QName = OpenFAST.QName;
using FastException = OpenFAST.Error.FastException;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;
using OpenFAST;
using System.Collections.Generic;

namespace OpenFAST.Template
{
	
	[Serializable]
	public class Group:Field
	{
		private int MaxPresenceMapSize
		{
			get
			{
				return fields.Length * 2;
			}
			
		}

		virtual public int FieldCount
		{
			get
			{
				return fields.Length;
			}
			
		}
		
		override public System.Type ValueType
		{
			get
			{
				return typeof(GroupValue);
			}
			
		}
		
		override public string TypeName
		{
			get
			{
				return "group";
			}
			
		}
		
		virtual public Field[] Fields
		{
			get
			{
				return fields;
			}
			
		}

		
		virtual public QName TypeReference
		{
			get
			{
				return typeReference;
			}
			
			set
			{
				this.typeReference = value;
			}
			
		}
		virtual public string ChildNamespace
		{
			get
			{
				return childNamespace;
			}
			
			set
			{
				this.childNamespace = value;
			}
			
		}
		virtual public StaticTemplateReference[] StaticTemplateReferences
		{
			get
			{
				return staticTemplateReferences;
			}
			
		}
		virtual public Field[] FieldDefinitions
		{
			get
			{
				return fieldDefinitions;
			}
			
		}
		private const long serialVersionUID = 1L;
		
		private QName typeReference = null;
		
		protected internal string childNamespace = "";
		protected internal Field[] fields;
		protected internal System.Collections.IDictionary fieldIndexMap;
		protected internal System.Collections.IDictionary fieldIdMap;
		protected internal System.Collections.IDictionary fieldNameMap;
		protected internal bool usesPresenceMap_Renamed_Field;
		protected internal StaticTemplateReference[] staticTemplateReferences;
		protected internal Field[] fieldDefinitions;
		protected internal System.Collections.IDictionary introspectiveFieldMap;
		
		public Group(string name, Field[] fields, bool optional):this(new QName(name), fields, optional)
		{
		}
		
		public Group(QName name, Field[] fields, bool optional):base(name, optional)
		{
            List<Field> expandedFields = new List<Field>();
            List<StaticTemplateReference> staticTemplateReferences = new List<StaticTemplateReference>();
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i] is StaticTemplateReference)
				{
                    StaticTemplateReference currentTemplate = (StaticTemplateReference)fields[i];
                    Field[] referenceFields = currentTemplate.Template.Fields;
					for (int j = 1; j < referenceFields.Length; j++)
						expandedFields.Add(referenceFields[j]);
                    staticTemplateReferences.Add(currentTemplate);
				}
				else
				{
					expandedFields.Add(fields[i]);
				}
			}
            this.fields = expandedFields.ToArray();
			this.fieldDefinitions = fields;
			this.fieldIndexMap = ConstructFieldIndexMap(this.fields);
			this.fieldNameMap = ConstructFieldNameMap(this.fields);
			this.fieldIdMap = ConstructFieldIdMap(this.fields);
			this.introspectiveFieldMap = ConstructInstrospectiveFields(this.fields);
			this.usesPresenceMap_Renamed_Field = DeterminePresenceMapUsage(this.fields);
            this.staticTemplateReferences = staticTemplateReferences.ToArray();
		}
		
		// BAD ABSTRACTION
		private static System.Collections.IDictionary ConstructInstrospectiveFields(Field[] fields)
		{
			System.Collections.IDictionary map = new System.Collections.Hashtable();
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i] is Scalar)
				{
					if (fields[i].HasAttribute(OpenFAST.Error.FastConstants.LENGTH_FIELD))
					{
						map[fields[i].GetAttribute(OpenFAST.Error.FastConstants.LENGTH_FIELD)] = fields[i];
					}
				}
			}

			return map;
		}
		
		
		private static bool DeterminePresenceMapUsage(Field[] fields)
		{
			for (int i = 0; i < fields.Length; i++)
				if (fields[i].UsesPresenceMapBit())
					return true;
			return false;
		}
		
		
		public override byte[] Encode(FieldValue value_Renamed, Group template, Context context, BitVectorBuilder presenceMapBuilder)
		{
			byte[] encoding = Encode(value_Renamed, template, context);
			if (optional)
			{
				if (encoding.Length != 0)
					presenceMapBuilder.set_Renamed();
				else
					presenceMapBuilder.Skip();
			}
			return encoding;
		}
		
		
		public virtual byte[] Encode(FieldValue value_Renamed, Group template, Context context)
		{
			if (value_Renamed == null)
			{
				return new byte[]{};
			}
			
			GroupValue groupValue = (GroupValue) value_Renamed;
			if (context.TraceEnabled)
			{
				context.GetEncodeTrace().GroupStart(this);
			}
			BitVectorBuilder presenceMapBuilder = new BitVectorBuilder(template.MaxPresenceMapSize);
			try
			{
				byte[][] fieldEncodings = new byte[fields.Length][];
				
				for (int fieldIndex = 0; fieldIndex < fields.Length; fieldIndex++)
				{
					FieldValue fieldValue = groupValue.GetValue(fieldIndex);
					Field field = GetField(fieldIndex);
					if (!field.Optional && fieldValue == null)
					{
						Global.HandleError(OpenFAST.Error.FastConstants.GENERAL_ERROR, "Mandatory field " + field + " is null");
					}
					byte[] encoding = field.Encode(fieldValue, template, context, presenceMapBuilder);
					fieldEncodings[fieldIndex] = encoding;
				}
				System.IO.MemoryStream buffer = new System.IO.MemoryStream();
				
				if (UsesPresenceMap())
				{
					byte[] pmap = presenceMapBuilder.BitVector.TruncatedBytes;
					if (context.TraceEnabled)
						context.GetEncodeTrace().Pmap(pmap);
					byte[] temp_byteArray;
					temp_byteArray = pmap;
					buffer.Write(temp_byteArray, 0, temp_byteArray.Length);
				}
				for (int i = 0; i < fieldEncodings.Length; i++)
				{
					if (fieldEncodings[i] != null)
					{
						byte[] temp_byteArray2;
						temp_byteArray2 = fieldEncodings[i];
						buffer.Write(temp_byteArray2, 0, temp_byteArray2.Length);
					}
				}
				if (context.TraceEnabled)
					context.GetEncodeTrace().GroupEnd();
				return buffer.ToArray();
			}
			catch (System.IO.IOException e)
			{
				throw new RuntimeException(e);
			}
		}
		
		
		public override FieldValue Decode(System.IO.Stream in_Renamed, Group group, Context context, BitVectorReader pmapReader)
		{
			try
			{
				if (!UsesPresenceMapBit() || pmapReader.Read())
				{
					if (context.TraceEnabled)
					{
						context.DecodeTrace.GroupStart(this);
					}
					GroupValue groupValue = new GroupValue(this, DecodeFieldValues(in_Renamed, group, context));
					if (context.TraceEnabled)
						context.DecodeTrace.GroupEnd();
					return groupValue;
				}
				else
					return null;
			}
			catch (FastException e)
			{
				throw new FastException("Error occurred while decoding " + this, e.Code, e);
			}
		}
		
		
		protected internal virtual FieldValue[] DecodeFieldValues(System.IO.Stream in_Renamed, Group template, Context context)
		{
			if (UsesPresenceMap())
			{
				BitVector pmap = ((BitVectorValue) TypeCodec.BIT_VECTOR.Decode(in_Renamed)).value_Renamed;
				if (context.TraceEnabled)
					context.DecodeTrace.Pmap(pmap.Bytes);
				if (pmap.Overlong)
				{
					Global.HandleError(OpenFAST.Error.FastConstants.R7_PMAP_OVERLONG, "The presence map " + pmap + " for the group " + this + " is overlong.");
				}
				return DecodeFieldValues(in_Renamed, template, new BitVectorReader(pmap), context);
			}
			else
			{
				return DecodeFieldValues(in_Renamed, template, BitVectorReader.NULL, context);
			}
		}
		
		public virtual FieldValue[] DecodeFieldValues(System.IO.Stream in_Renamed, Group template, BitVectorReader pmapReader, Context context)
		{
			FieldValue[] values = new FieldValue[fields.Length];
			int start = this is MessageTemplate?1:0;
			for (int fieldIndex = start; fieldIndex < fields.Length; fieldIndex++)
			{
				Field field = GetField(fieldIndex);
				values[fieldIndex] = field.Decode(in_Renamed, template, context, pmapReader);
			}
			if (pmapReader.HasMoreBitsSet())
			{
				Global.HandleError(OpenFAST.Error.FastConstants.R8_PMAP_TOO_MANY_BITS, "The presence map " + pmapReader + " has too many bits for the group " + this);
			}
			
			return values;
		}
		
		
		public override bool IsPresenceMapBitSet(byte[] encoding, FieldValue fieldValue)
		{
			return encoding.Length != 0;
		}
		
		
		public override bool UsesPresenceMapBit()
		{
			return optional;
		}
		
		public virtual bool UsesPresenceMap()
		{
			return usesPresenceMap_Renamed_Field;
		}
		
		
		public virtual Field GetField(int index)
		{
			return fields[index];
		}
		
		
		public override FieldValue CreateValue(string value_Renamed)
		{
			return new GroupValue(this, new FieldValue[fields.Length]);
		}
		
		
		public virtual Field GetField(string fieldName)
		{
			return (Field) fieldNameMap[new QName(fieldName, childNamespace)];
		}
		
		public virtual Field GetField(QName name)
		{
			return (Field) fieldNameMap[name];
		}
		
		
		private static System.Collections.IDictionary ConstructFieldNameMap(Field[] fields)
		{
			System.Collections.IDictionary map = new System.Collections.Hashtable();
			
			for (int i = 0; i < fields.Length; i++)
				map[fields[i].QName] = fields[i];
			
			return map;
		}
		
		private static System.Collections.IDictionary ConstructFieldIdMap(Field[] fields)
		{
			System.Collections.IDictionary map = new System.Collections.Hashtable();
			
			for (int i = 0; i < fields.Length; i++)
				map[fields[i].Id] = fields[i];
			
			return map;
		}
		
		
		private static System.Collections.IDictionary ConstructFieldIndexMap(Field[] fields)
		{
			System.Collections.IDictionary map = new System.Collections.Hashtable();
			
			for (int i = 0; i < fields.Length; i++)
				map[fields[i]] = (System.Int32) i;
			
			return map;
		}
		
		public virtual int GetFieldIndex(string fieldName)
		{
			return ((System.Int32) fieldIndexMap[GetField(fieldName)]);
		}
		
		public virtual int GetFieldIndex(Field field)
		{
			return ((System.Int32) fieldIndexMap[field]);
		}
		
		public virtual Sequence GetSequence(string fieldName)
		{
			return (Sequence) GetField(fieldName);
		}
		
		public virtual Scalar GetScalar(string fieldName)
		{
			return (Scalar) GetField(fieldName);
		}
		
		public virtual Scalar GetScalar(int index)
		{
			return (Scalar) GetField(index);
		}
		
		public virtual Group GetGroup(string fieldName)
		{
			return (Group) GetField(fieldName);
		}
		
		public virtual bool HasField(string fieldName)
		{
			return fieldNameMap.Contains(new QName(fieldName, childNamespace));
		}
		
		public virtual bool HasField(QName fieldName)
		{
			return fieldNameMap.Contains(fieldName);
		}
		
		public virtual bool HasTypeReference()
		{
			return typeReference != null;
		}
		
		public override string ToString()
		{
			return name.Name;
		}
		
		public override int GetHashCode()
		{
			int prime = 31;
			int result = 1;
			result = prime * result + Group.hashCode(fields);
			result = prime * result + name.GetHashCode();
			result = prime * result + ((typeReference == null)?0:typeReference.GetHashCode());
			return result;
		}
		
		public  override bool Equals(System.Object obj)
		{
			if (this == obj)
				return true;
			if (obj == null || GetType() != obj.GetType())
				return false;

			Group other = (Group) obj;
			if (other.fields.Length != fields.Length)
				return false;
			if (!other.name.Equals(name))
				return false;
			for (int i = 0; i < fields.Length; i++)
				if (!fields[i].Equals(other.fields[i]))
					return false;
			return true;
		}
		
		private static int hashCode(System.Object[] array)
		{
			int prime = 31;
			if (array == null)
				return 0;
			int result = 1;
			for (int index = 0; index < array.Length; index++)
			{
				result = prime * result + (array[index] == null?0:array[index].GetHashCode());
			}
			return result;
		}
		
		public virtual bool HasFieldWithId(string id)
		{
			return fieldIdMap.Contains(id);
		}
		
		public virtual Field GetFieldById(string id)
		{
			return (Field) fieldIdMap[id];
		}
		
		public virtual StaticTemplateReference GetStaticTemplateReference(string name)
		{
			return GetStaticTemplateReference(new QName(name, this.name.Namespace));
		}
		
		public virtual StaticTemplateReference GetStaticTemplateReference(QName name)
		{
			for (int i = 0; i < staticTemplateReferences.Length; i++)
			{
				if (staticTemplateReferences[i].QName.Equals(name))
					return staticTemplateReferences[i];
			}
			return null;
		}
		
		public virtual bool HasIntrospectiveField(string fieldName)
		{
			return introspectiveFieldMap.Contains(fieldName);
		}
		
		public virtual Scalar GetIntrospectiveField(string fieldName)
		{
			return (Scalar) introspectiveFieldMap[fieldName];
		}
	}
}