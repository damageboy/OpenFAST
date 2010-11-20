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
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenFAST.Error;
using OpenFAST.Template.Types.Codec;
using OpenFAST.Utility;

namespace OpenFAST.Template
{
    public class Group : Field
    {
        private readonly Field[] _fieldDefinitions;
        private readonly Dictionary<string, Field> _fieldIdMap;
        private readonly Dictionary<Field, int> _fieldIndexMap;
        private readonly Dictionary<QName, Field> _fieldNameMap;
        private readonly Field[] _fields;
        private readonly Dictionary<string, Scalar> _introspectiveFieldMap;
        private readonly StaticTemplateReference[] _staticTemplateReferences;
        private readonly bool _usesPresenceMapRenamedField;
        private string _childNamespace;
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

            _childNamespace = "";

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
            _usesPresenceMapRenamedField = _fields.Any(t => t.UsesPresenceMapBit);
            _staticTemplateReferences = references.ToArray();
        }

        private int MaxPresenceMapSize
        {
            get { return _fields.Length*2; }
        }

        public int FieldCount
        {
            get { return _fields.Length; }
        }

        public override Type ValueType
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
            set
            {
                ThrowOnReadonly();
                _typeReference = value;
            }
        }

        public string ChildNamespace
        {
            get { return _childNamespace; }
            set
            {
                ThrowOnReadonly();
                _childNamespace = value;
            }
        }

        public StaticTemplateReference[] StaticTemplateReferences
        {
            get { return _staticTemplateReferences; }
        }

        public Field[] FieldDefinitions
        {
            get { return _fieldDefinitions; }
        }

        public bool HasTypeReference
        {
            get { return TypeReference != null; }
        }

        public override bool UsesPresenceMapBit
        {
            get { return IsOptional; }
        }

        protected virtual bool UsesPresenceMap
        {
            get { return _usesPresenceMapRenamedField; }
        }

        // BAD ABSTRACTION
        private static Dictionary<string, Scalar> ConstructInstrospectiveFields(Field[] fields)
        {
            var map = new Dictionary<string, Scalar>();
            foreach (Field t in fields)
            {
                var s = t as Scalar;
                if (s != null)
                {
                    string attr;
                    if (t.TryGetAttribute(FastConstants.LengthField, out attr))
                        map[attr] = s;
                }
            }

            return map;
        }


        public override byte[] Encode(IFieldValue value, Group encodeTemplate, Context context,
                                      BitVectorBuilder presenceMapBuilder)
        {
            byte[] encoding = Encode(value, encodeTemplate, context);
            if (IsOptional)
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
                return ByteUtil.EmptyByteArray;
            }

            var groupValue = (GroupValue) value;
            if (context.TraceEnabled)
            {
                context.EncodeTrace.GroupStart(this);
            }
            var presenceMapBuilder = new BitVectorBuilder(template.MaxPresenceMapSize);
            try
            {
                var fieldEncodings = new byte[_fields.Length][];

                for (int fieldIndex = 0; fieldIndex < _fields.Length; fieldIndex++)
                {
                    IFieldValue fieldValue = groupValue.GetValue(fieldIndex);
                    Field field = _fields[fieldIndex];
                    if (!field.IsOptional && fieldValue == null)
                    {
                        Global.ErrorHandler.OnError(null, DynError.GeneralError, "Mandatory field {0} is null", field);
                        // BUG? error is ignored?
                    }
                    byte[] encoding = field.Encode(fieldValue, field.MessageTemplate ?? template, context, presenceMapBuilder);
                    fieldEncodings[fieldIndex] = encoding;
                }
                var buffer = new MemoryStream();

                if (UsesPresenceMap)
                {
                    byte[] pmap = presenceMapBuilder.BitVector.TruncatedBytes;
                    if (context.TraceEnabled)
                        context.EncodeTrace.Pmap(pmap);
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
                    context.EncodeTrace.GroupEnd();
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
                if (!UsesPresenceMapBit || pmapReader.Read())
                {
                    if (context.TraceEnabled)
                        context.DecodeTrace.GroupStart(this);

                    var groupValue = new GroupValue(this, DecodeFieldValues(inStream, decodeTemplate, context));

                    if (context.TraceEnabled)
                        context.DecodeTrace.GroupEnd();

                    return groupValue;
                }
                return null;
            }
            catch (DynErrorException e)
            {
                throw new DynErrorException(e, e.Error, "Error occurred while decoding {0}", this);
            }
        }


        private IFieldValue[] DecodeFieldValues(Stream inStream, Group template,
                                                Context context)
        {
            if (!UsesPresenceMap)
            {
                return DecodeFieldValues(inStream, template, BitVectorReader.Null, context);
            }
            BitVector pmap = ((BitVectorValue) TypeCodec.BitVector.Decode(inStream)).Value;
            if (context.TraceEnabled)
                context.DecodeTrace.Pmap(pmap.Bytes);
            if (pmap.IsOverlong)
            {
                Global.ErrorHandler.OnError(null, RepError.PmapOverlong,
                                            "The presence map {0} for the group {1} is overlong.", pmap, this);
            }
            return DecodeFieldValues(inStream, template, new BitVectorReader(pmap), context);
        }

        protected IFieldValue[] DecodeFieldValues(Stream inStream, Group template,
                                                  BitVectorReader pmapReader, Context context)
        {
            var values = new IFieldValue[_fields.Length];
            int start = this is MessageTemplate ? 1 : 0;

            for (int fieldIndex = start; fieldIndex < _fields.Length; fieldIndex++)
            {
                var field = _fields[fieldIndex];
                values[fieldIndex] = field.Decode(inStream, field.MessageTemplate ?? template, context, pmapReader);
            }

            if (pmapReader.HasMoreBitsSet)
            {
                Global.ErrorHandler.OnError(null, RepError.PmapTooManyBits,
                                            "The presence map {0} has too many bits for the group {1}", pmapReader, this);
            }

            return values;
        }

        public override bool IsPresenceMapBitSet(byte[] encoding, IFieldValue fieldValue)
        {
            return encoding.Length != 0;
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
            return new QName(fieldName, ChildNamespace);
        }

        private static Dictionary<Field, int> ConstructFieldIndexMap(Field[] fields)
        {
            var map = new Dictionary<Field, int>(fields.Length);
            for (int i = 0; i < fields.Length; i++)
                map[fields[i]] = i;
            return map;
        }

        public int GetFieldIndex(string fieldName)
        {
            return _fieldIndexMap[GetField(fieldName)];
        }

        public bool TryGetFieldIndex(string fieldName, out int index)
        {
            index = -1;
            Field field;
            if (_fieldNameMap.TryGetValue(QnameWithChildNs(fieldName), out field))
            {
                if (field != null)
                {
                    return _fieldIndexMap.TryGetValue(field, out index);
                }
            }
            return false;
        }

        public int GetFieldIndex(Field field)
        {
            int index;
            if (!_fieldIndexMap.TryGetValue(field, out index))
                throw new ArgumentOutOfRangeException(
                    "field", field, "Field does not exist in group " + this);
            return index;
        }

        public Sequence GetSequence(string fieldName)
        {
            return (Sequence) GetField(fieldName);
        }

        public Scalar GetScalar(string fieldName)
        {
            return (Scalar) GetField(fieldName);
        }

        public Scalar GetScalar(int index)
        {
            return (Scalar) _fields[index];
        }

        public Group GetGroup(string fieldName)
        {
            return (Group) GetField(fieldName);
        }

        public bool HasField(string fieldName)
        {
            return HasField(QnameWithChildNs(fieldName));
        }

        public bool HasField(QName fieldName)
        {
            return _fieldNameMap.ContainsKey(fieldName);
        }

        public override string ToString()
        {
            return Name;
        }

        public StaticTemplateReference GetStaticTemplateReference(string qname)
        {
            return GetStaticTemplateReference(new QName(qname, QName.Namespace));
        }

        public StaticTemplateReference GetStaticTemplateReference(QName qname)
        {
            for (int i = 0; i < _staticTemplateReferences.Length; i++)
                if (_staticTemplateReferences[i].QName.Equals(qname))
                    return _staticTemplateReferences[i];
            return null;
        }

        public bool TryGetIntrospectiveField(string fieldName, out Scalar field)
        {
            return _introspectiveFieldMap.TryGetValue(fieldName, out field);
        }

        #region Equals

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;

            var other = obj as Group;
            if (ReferenceEquals(null, other)) return false;
            return base.Equals(other) && Util.ArrayEqualsSlow(other._fields, _fields, 0);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ Util.GetCollectionHashCode(_fields);
                return result;
            }
        }

        #endregion
    }
}