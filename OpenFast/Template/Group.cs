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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenFAST.Error;
using OpenFAST.Template.Type.Codec;
using OpenFAST.util;

namespace OpenFAST.Template
{
    [Serializable]
    public class Group : Field, IEquatable<Group>
    {
        private readonly Field[] _fieldDefinitions;
        private readonly Dictionary<string, Field> _fieldIdMap;
        private readonly Dictionary<Field, int> _fieldIndexMap;
        private readonly Dictionary<QName, Field> _fieldNameMap;
        private readonly Field[] _fields;
        private readonly Dictionary<string, Field> _introspectiveFieldMap;
        private readonly StaticTemplateReference[] _staticTemplateReferences;
        private readonly bool _usesPresenceMapRenamedField;
        private string _childNamespace = "";
        private QName _typeReference;

        public Group(string name, Field[] fields, bool optional)
            : this(new QName(name), fields, optional)
        {
        }

        public Group(QName name, Field[] fields, bool optional)
            : base(name, optional)
        {
            if (fields == null) throw new ArgumentNullException("fields");

            var expandedFields = new List<Field>();
            var references = new List<StaticTemplateReference>();

            foreach (Field t in fields)
            {
                if (t is StaticTemplateReference)
                {
                    var currentTemplate = (StaticTemplateReference) t;
                    Field[] referenceFields = currentTemplate.Template.Fields;
                    for (int j = 1; j < referenceFields.Length; j++)
                        expandedFields.Add(referenceFields[j]);
                    references.Add(currentTemplate);
                }
                else
                    expandedFields.Add(t);
            }

            _fields = expandedFields.ToArray();
            _fieldDefinitions = fields;
            _fieldIndexMap = ConstructFieldIndexMap(_fields);
            _fieldNameMap = Util.ToSafeDictionary(_fields, f => f.QName);
            _fieldIdMap = Util.ToSafeDictionary(_fields, f => f.Id);
            _introspectiveFieldMap = ConstructInstrospectiveFields(_fields);
            _usesPresenceMapRenamedField = _fields.Any(t => t.UsesPresenceMapBit());
            _staticTemplateReferences = references.ToArray();
        }

        private int MaxPresenceMapSize
        {
            get { return _fields.Length*2; }
        }

        public virtual int FieldCount
        {
            get { return _fields.Length; }
        }

        public override System.Type ValueType
        {
            get { return typeof (GroupValue); }
        }

        public override string TypeName
        {
            get { return "group"; }
        }

        public Field[] Fields
        {
            get { return _fields; }
        }

        public QName TypeReference
        {
            get { return _typeReference; }
            set { _typeReference = value; }
        }

        public string ChildNamespace
        {
            get { return _childNamespace; }
            set { _childNamespace = value; }
        }

        public StaticTemplateReference[] StaticTemplateReferences
        {
            get { return _staticTemplateReferences; }
        }

        public Field[] FieldDefinitions
        {
            get { return _fieldDefinitions; }
        }

        // BAD ABSTRACTION
        private static Dictionary<string, Field> ConstructInstrospectiveFields(Field[] fields)
        {
            var map = new Dictionary<string, Field>();
            foreach (Field t in fields)
            {
                if (t is Scalar)
                {
                    string attr;
                    if (t.TryGetAttribute(FastConstants.LENGTH_FIELD, out attr))
                        map[attr] = t;
                }
            }

            return map;
        }


        public override byte[] Encode(IFieldValue value, Group encodeTemplate, Context context,
                                      BitVectorBuilder presenceMapBuilder)
        {
            byte[] encoding = Encode(value, encodeTemplate, context);
            if (Optional)
            {
                if (encoding.Length != 0)
                    presenceMapBuilder.Set();
                else
                    presenceMapBuilder.Skip();
            }
            return encoding;
        }


        public byte[] Encode(IFieldValue value, Group template, Context context)
        {
            if (value == null)
            {
                return new byte[] {};
            }

            var groupValue = (GroupValue) value;
            if (context.TraceEnabled)
            {
                context.GetEncodeTrace().GroupStart(this);
            }
            var presenceMapBuilder = new BitVectorBuilder(template.MaxPresenceMapSize);
            try
            {
                var fieldEncodings = new byte[_fields.Length][];

                for (int fieldIndex = 0; fieldIndex < _fields.Length; fieldIndex++)
                {
                    IFieldValue fieldValue = groupValue.GetValue(fieldIndex);
                    Field field = _fields[fieldIndex];
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
                    buffer.Write(pmap, 0, pmap.Length);
                }
                foreach (var t in fieldEncodings)
                {
                    if (t != null)
                    {
                        byte[] tmp = t;
                        buffer.Write(tmp, 0, tmp.Length);
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


        public override IFieldValue Decode(Stream inStream, Group decodeTemplate, Context context,
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
                    var groupValue = new GroupValue(this, DecodeFieldValues(inStream, decodeTemplate, context));
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


        protected virtual IFieldValue[] DecodeFieldValues(Stream inStream, Group template,
                                                          Context context)
        {
            if (!UsesPresenceMap())
            {
                return DecodeFieldValues(inStream, template, BitVectorReader.NULL, context);
            }
            BitVector pmap = ((BitVectorValue) TypeCodec.BIT_VECTOR.Decode(inStream)).Value;
            if (context.TraceEnabled)
                context.DecodeTrace.Pmap(pmap.Bytes);
            if (pmap.Overlong)
            {
                Global.HandleError(FastConstants.R7_PMAP_OVERLONG,
                                   "The presence map " + pmap + " for the group " + this + " is overlong.");
            }
            return DecodeFieldValues(inStream, template, new BitVectorReader(pmap), context);
        }

        public IFieldValue[] DecodeFieldValues(Stream inStream, Group template,
                                               BitVectorReader pmapReader, Context context)
        {
            var values = new IFieldValue[_fields.Length];
            int start = this is MessageTemplate ? 1 : 0;

            for (int fieldIndex = start; fieldIndex < _fields.Length; fieldIndex++)
            {
                values[fieldIndex] = _fields[fieldIndex].Decode(inStream, template, context, pmapReader);
            }

            if (pmapReader.HasMoreBitsSet())
            {
                Global.HandleError(FastConstants.R8_PMAP_TOO_MANY_BITS,
                                   "The presence map " + pmapReader + " has too many bits for the group " + this);
            }

            return values;
        }

        public override bool IsPresenceMapBitSet(byte[] encoding, IFieldValue fieldValue)
        {
            return encoding.Length != 0;
        }


        public override bool UsesPresenceMapBit()
        {
            return Optional;
        }

        public virtual bool UsesPresenceMap()
        {
            return _usesPresenceMapRenamedField;
        }

        public override IFieldValue CreateValue(string value)
        {
            return new GroupValue(this, new IFieldValue[_fields.Length]);
        }

        public Field GetField(string fieldName)
        {
            return GetField(QnameWithChildNs(fieldName));
        }

        public Field GetField(QName fieldName)
        {
            Field fld;
            if (!_fieldNameMap.TryGetValue(fieldName, out fld))
                throw new ArgumentOutOfRangeException("fieldName", fieldName,
                                                      "Field does not exist in group " + this);
            return fld;
        }

        public bool TryGetField(QName fieldName, out Field field)
        {
            return _fieldNameMap.TryGetValue(fieldName, out field);
        }

        public bool TryGetField(string fieldName, out Field field)
        {
            return _fieldNameMap.TryGetValue(QnameWithChildNs(fieldName), out field);
        }

        private QName QnameWithChildNs(string fieldName)
        {
            return new QName(fieldName, _childNamespace);
        }

        private static Dictionary<Field, int> ConstructFieldIndexMap(Field[] fields)
        {
            var map = new Dictionary<Field, int>();
            for (int i = 0; i < fields.Length; i++)
                map[fields[i]] = i;
            return map;
        }

        public int GetFieldIndex(string fieldName)
        {
            return _fieldIndexMap[GetField(fieldName)];
        }

        public virtual int GetFieldIndex(Field field)
        {
            int index;
            if (!_fieldIndexMap.TryGetValue(field, out index))
                throw new ArgumentOutOfRangeException("field", field,
                                                      "Field does not exist in group " + this);
            return index;
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
            return (Scalar) _fields[index];
        }

        public virtual Group GetGroup(string fieldName)
        {
            return (Group) GetField(fieldName);
        }

        public virtual bool HasField(string fieldName)
        {
            return _fieldNameMap.ContainsKey(QnameWithChildNs(fieldName));
        }

        public virtual bool HasField(QName fieldName)
        {
            return _fieldNameMap.ContainsKey(fieldName);
        }

        public virtual bool HasTypeReference()
        {
            return _typeReference != null;
        }

        public override string ToString()
        {
            return Name;
        }

        #region Equals

        public bool Equals(Group other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Util.ArrayEquals(other._fields, _fields);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result * 397) ^ Util.ArrayHashCode(_fields);
                return result;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Group);
        }

        #endregion

#warning Never used? Not sure why IDs are even used by this class
        public virtual bool HasFieldWithId(string fid)
        {
            return _fieldIdMap.ContainsKey(fid);
        }

#warning Never used? Not sure why IDs are even used by this class
        public virtual Field GetFieldById(string fid)
        {
            return _fieldIdMap[fid];
        }

        public virtual StaticTemplateReference GetStaticTemplateReference(string qname)
        {
            return GetStaticTemplateReference(new QName(qname, QName.Namespace));
        }

        public virtual StaticTemplateReference GetStaticTemplateReference(QName qname)
        {
            for (int i = 0; i < _staticTemplateReferences.Length; i++)
                if (_staticTemplateReferences[i].QName.Equals(qname))
                    return _staticTemplateReferences[i];
            return null;
        }

        public virtual bool HasIntrospectiveField(string fieldName)
        {
            return _introspectiveFieldMap.ContainsKey(fieldName);
        }

        public virtual Scalar GetIntrospectiveField(string fieldName)
        {
            return (Scalar) _introspectiveFieldMap[fieldName];
        }
    }
}