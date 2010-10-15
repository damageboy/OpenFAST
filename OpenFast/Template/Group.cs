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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OpenFAST.Error;
using OpenFAST.Template.Type.Codec;

namespace OpenFAST.Template
{
    [Serializable]
    public class Group : Field
    {
        protected internal string childNamespace = "";
        protected internal Field[] fieldDefinitions;
        protected internal IDictionary fieldIdMap;
        protected internal IDictionary fieldIndexMap;
        protected internal IDictionary fieldNameMap;
        protected internal Field[] fields;
        protected internal IDictionary introspectiveFieldMap;
        protected internal StaticTemplateReference[] staticTemplateReferences;
        private QName typeReference;
        protected internal bool usesPresenceMap_Renamed_Field;

        public Group(string name, Field[] fields, bool optional) : this(new QName(name), fields, optional)
        {
        }

        public Group(QName name, Field[] fields, bool optional) : base(name, optional)
        {
            var expandedFields = new List<Field>();
            var references = new List<StaticTemplateReference>();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i] is StaticTemplateReference)
                {
                    var currentTemplate = (StaticTemplateReference) fields[i];
                    Field[] referenceFields = currentTemplate.Template.Fields;
                    for (int j = 1; j < referenceFields.Length; j++)
                        expandedFields.Add(referenceFields[j]);
                    references.Add(currentTemplate);
                }
                else
                {
                    expandedFields.Add(fields[i]);
                }
            }
            this.fields = expandedFields.ToArray();
            fieldDefinitions = fields;
            fieldIndexMap = ConstructFieldIndexMap(this.fields);
            fieldNameMap = ConstructFieldNameMap(this.fields);
            fieldIdMap = ConstructFieldIdMap(this.fields);
            introspectiveFieldMap = ConstructInstrospectiveFields(this.fields);
            usesPresenceMap_Renamed_Field = DeterminePresenceMapUsage(this.fields);
            staticTemplateReferences = references.ToArray();
        }

        private int MaxPresenceMapSize
        {
            get { return fields.Length*2; }
        }

        public virtual int FieldCount
        {
            get { return fields.Length; }
        }

        public override System.Type ValueType
        {
            get { return typeof (GroupValue); }
        }

        public override string TypeName
        {
            get { return "group"; }
        }

        public virtual Field[] Fields
        {
            get { return fields; }
        }


        public virtual QName TypeReference
        {
            get { return typeReference; }

            set { typeReference = value; }
        }

        public virtual string ChildNamespace
        {
            get { return childNamespace; }

            set { childNamespace = value; }
        }

        public virtual StaticTemplateReference[] StaticTemplateReferences
        {
            get { return staticTemplateReferences; }
        }

        public virtual Field[] FieldDefinitions
        {
            get { return fieldDefinitions; }
        }

        // BAD ABSTRACTION
        private static IDictionary ConstructInstrospectiveFields(Field[] fields)
        {
            IDictionary map = new Hashtable();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i] is Scalar)
                {
                    if (fields[i].HasAttribute(FastConstants.LENGTH_FIELD))
                    {
                        map[fields[i].GetAttribute(FastConstants.LENGTH_FIELD)] = fields[i];
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


        public override byte[] Encode(FieldValue value_Renamed, Group encodeTemplate, Context context,
                                      BitVectorBuilder presenceMapBuilder)
        {
            byte[] encoding = Encode(value_Renamed, encodeTemplate, context);
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
                return new byte[] {};
            }

            var groupValue = (GroupValue) value_Renamed;
            if (context.TraceEnabled)
            {
                context.GetEncodeTrace().GroupStart(this);
            }
            var presenceMapBuilder = new BitVectorBuilder(template.MaxPresenceMapSize);
            try
            {
                var fieldEncodings = new byte[fields.Length][];

                for (int fieldIndex = 0; fieldIndex < fields.Length; fieldIndex++)
                {
                    FieldValue fieldValue = groupValue.GetValue(fieldIndex);
                    Field field = GetField(fieldIndex);
                    if (!field.Optional && fieldValue == null)
                    {
                        Global.HandleError(FastConstants.GENERAL_ERROR, "Mandatory field " + field + " is null");
                    }
                    byte[] encoding = field.Encode(fieldValue, template, context, presenceMapBuilder);
                    fieldEncodings[fieldIndex] = encoding;
                }
                var buffer = new MemoryStream();

                if (UsesPresenceMap())
                {
                    byte[] pmap = presenceMapBuilder.BitVector.TruncatedBytes;
                    if (context.TraceEnabled)
                        context.GetEncodeTrace().Pmap(pmap);
                    byte[] temp_byteArray = pmap;
                    buffer.Write(temp_byteArray, 0, temp_byteArray.Length);
                }
                for (int i = 0; i < fieldEncodings.Length; i++)
                {
                    if (fieldEncodings[i] != null)
                    {
                        byte[] temp_byteArray2 = fieldEncodings[i];
                        buffer.Write(temp_byteArray2, 0, temp_byteArray2.Length);
                    }
                }
                if (context.TraceEnabled)
                    context.GetEncodeTrace().GroupEnd();
                return buffer.ToArray();
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }
        }


        public override FieldValue Decode(Stream in_Renamed, Group decodeTemplate, Context context,
                                          BitVectorReader pmapReader)
        {
            try
            {
                if (!UsesPresenceMapBit() || pmapReader.Read())
                {
                    if (context.TraceEnabled)
                    {
                        context.DecodeTrace.GroupStart(this);
                    }
                    var groupValue = new GroupValue(this, DecodeFieldValues(in_Renamed, decodeTemplate, context));
                    if (context.TraceEnabled)
                        context.DecodeTrace.GroupEnd();
                    return groupValue;
                }
                return null;
            }
            catch (FastException e)
            {
                throw new FastException("Error occurred while decoding " + this, e.Code, e);
            }
        }


        protected internal virtual FieldValue[] DecodeFieldValues(Stream in_Renamed, Group template,
                                                                  Context context)
        {
            if (!UsesPresenceMap())
            {
                return DecodeFieldValues(in_Renamed, template, BitVectorReader.NULL, context);
            }
            BitVector pmap = ((BitVectorValue) TypeCodec.BIT_VECTOR.Decode(in_Renamed)).value_Renamed;
            if (context.TraceEnabled)
                context.DecodeTrace.Pmap(pmap.Bytes);
            if (pmap.Overlong)
            {
                Global.HandleError(FastConstants.R7_PMAP_OVERLONG,
                                   "The presence map " + pmap + " for the group " + this + " is overlong.");
            }
            return DecodeFieldValues(in_Renamed, template, new BitVectorReader(pmap), context);
        }

        public virtual FieldValue[] DecodeFieldValues(Stream in_Renamed, Group template,
                                                      BitVectorReader pmapReader, Context context)
        {
            var values = new FieldValue[fields.Length];
            int start = this is MessageTemplate ? 1 : 0;
            for (int fieldIndex = start; fieldIndex < fields.Length; fieldIndex++)
            {
                Field field = GetField(fieldIndex);
                values[fieldIndex] = field.Decode(in_Renamed, template, context, pmapReader);
            }
            if (pmapReader.HasMoreBitsSet())
            {
                Global.HandleError(FastConstants.R8_PMAP_TOO_MANY_BITS,
                                   "The presence map " + pmapReader + " has too many bits for the group " + this);
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

        public virtual Field GetField(QName qname)
        {
            return (Field) fieldNameMap[qname];
        }


        private static IDictionary ConstructFieldNameMap(Field[] fields)
        {
            IDictionary map = new Hashtable();

            for (int i = 0; i < fields.Length; i++)
                map[fields[i].QName] = fields[i];

            return map;
        }

        private static IDictionary ConstructFieldIdMap(Field[] fields)
        {
            IDictionary map = new Hashtable();

            for (int i = 0; i < fields.Length; i++)
                map[fields[i].Id] = fields[i];

            return map;
        }


        private static IDictionary ConstructFieldIndexMap(Field[] fields)
        {
            IDictionary map = new Hashtable();

            for (int i = 0; i < fields.Length; i++)
                map[fields[i]] = i;

            return map;
        }

        public virtual int GetFieldIndex(string fieldName)
        {
            return ((Int32) fieldIndexMap[GetField(fieldName)]);
        }

        public virtual int GetFieldIndex(Field field)
        {
            return ((Int32) fieldIndexMap[field]);
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
            const int prime = 31;
            int result = 1;
            result = prime*result + hashCode(fields);
            result = prime*result + name.GetHashCode();
            result = prime*result + ((typeReference == null) ? 0 : typeReference.GetHashCode());
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (Group) obj;
            if (other.fields.Length != fields.Length)
                return false;
            if (!other.name.Equals(name))
                return false;
            for (int i = 0; i < fields.Length; i++)
                if (!fields[i].Equals(other.fields[i]))
                    return false;
            return true;
        }

        private static int hashCode(Object[] array)
        {
            const int prime = 31;
            if (array == null)
                return 0;
            int result = 1;
            for (int index = 0; index < array.Length; index++)
            {
                result = prime*result + (array[index] == null ? 0 : array[index].GetHashCode());
            }
            return result;
        }

        public virtual bool HasFieldWithId(string fid)
        {
            return fieldIdMap.Contains(fid);
        }

        public virtual Field GetFieldById(string fid)
        {
            return (Field) fieldIdMap[fid];
        }

        public virtual StaticTemplateReference GetStaticTemplateReference(string qname)
        {
            return GetStaticTemplateReference(new QName(qname, name.Namespace));
        }

        public virtual StaticTemplateReference GetStaticTemplateReference(QName qname)
        {
            for (int i = 0; i < staticTemplateReferences.Length; i++)
            {
                if (staticTemplateReferences[i].QName.Equals(qname))
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