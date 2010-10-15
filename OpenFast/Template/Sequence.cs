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
using System.IO;
using OpenFAST.Error;
using OpenFAST.Template.Type;

namespace OpenFAST.Template
{
    [Serializable]
    public class Sequence : Field, IFieldSet
    {
        private readonly Group _group;
        private readonly bool _implicitLength;
        private readonly Scalar _length;

        public Sequence(QName name, Field[] fields, bool optional)
            : this(name, CreateLength(name, optional), fields, optional)
        {
            _implicitLength = true;
        }

        public Sequence(string name, Field[] fields, bool optional) : this(new QName(name), fields, optional)
        {
        }

        public Sequence(QName name, Scalar length, Field[] fields, bool optional) : base(name, optional)
        {
            _group = new Group(name, fields, optional);

            if (length == null)
            {
                _length = CreateLength(name, optional);
                _implicitLength = true;
            }
            else
            {
                _length = length;
            }
        }

        public virtual Scalar Length
        {
            get { return _length; }
        }

        public override System.Type ValueType
        {
            get { return typeof (SequenceValue); }
        }

        public override string TypeName
        {
            get { return "sequence"; }
        }

        public virtual Group Group
        {
            get { return _group; }
        }

        public virtual bool ImplicitLength
        {
            get { return _implicitLength; }
        }

        public virtual QName TypeReference
        {
            get { return _group.TypeReference; }

            set { _group.TypeReference = value; }
        }

        #region IFieldSet Members

        public virtual int FieldCount
        {
            get { return _group.FieldCount; }
        }

        public virtual Field GetField(int index)
        {
            return _group.GetField(index);
        }

        #endregion

        private static Scalar CreateLength(QName name, bool optional)
        {
            return new Scalar(Global.CreateImplicitName(name), FASTType.U32, Operator.Operator.NONE,
                              ScalarValue.UNDEFINED, optional);
        }

        public override bool UsesPresenceMapBit()
        {
            return _length.UsesPresenceMapBit();
        }

        public override bool IsPresenceMapBitSet(byte[] encoding, IFieldValue fieldValue)
        {
            return _length.IsPresenceMapBitSet(encoding, fieldValue);
        }

        public override byte[] Encode(IFieldValue value, Group encodeTemplate, Context context,
                                      BitVectorBuilder presenceMapBuilder)
        {
            if (HasTypeReference())
                context.CurrentApplicationType = TypeReference;
            if (value == null)
                return _length.Encode(null, encodeTemplate, context, presenceMapBuilder);

            var buffer = new MemoryStream();
            var val = (SequenceValue) value;
            int len = val.Length;

            try
            {
                byte[] tmp = _length.Encode(new IntegerValue(len), encodeTemplate, context, presenceMapBuilder);
                buffer.Write(tmp, 0, tmp.Length);

                foreach (GroupValue v in val.Elements)
                {
                    tmp = _group.Encode(v, encodeTemplate, context);
                    buffer.Write(tmp, 0, tmp.Length);
                }
            }
            catch (IOException e)
            {
                Global.HandleError(FastConstants.IO_ERROR, "An IO error occurred while encoding " + this, e);
            }

            return buffer.ToArray();
        }

        public override IFieldValue Decode(Stream inStream, Group decodeTemplate, Context context,
                                           BitVectorReader pmapReader)
        {
            var sequenceValue = new SequenceValue(this);
            IFieldValue lengthValue = _length.Decode(inStream, decodeTemplate, context, pmapReader);

            if ((lengthValue == ScalarValue.NULL) || (lengthValue == null))
            {
                return null;
            }

            int len = ((IntegerValue) lengthValue).Value;

            for (int i = 0; i < len; i++)
                sequenceValue.Add(
                    (GroupValue) _group.Decode(inStream, decodeTemplate, context, BitVectorReader.INFINITE_TRUE));

            return sequenceValue;
        }

        public override IFieldValue CreateValue(string value)
        {
            return new SequenceValue(this);
        }

        public virtual bool HasField(string fieldName)
        {
            return _group.HasField(fieldName);
        }

        public virtual bool HasTypeReference()
        {
            return _group.HasTypeReference();
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime*result + ((_group == null) ? 0 : _group.GetHashCode());
            result = prime*result + ((_length == null) ? 0 : _length.GetHashCode());
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (Sequence) obj;
            if (!_group.Equals(other._group))
                return false;
            if (ImplicitLength != other.ImplicitLength)
                return false;
            if (!ImplicitLength && !_length.Equals(other._length))
                return false;
            return true;
        }

        public override bool HasAttribute(QName attributeName)
        {
            return _group.HasAttribute(attributeName);
        }

        public override bool TryGetAttribute(QName qname, out string value)
        {
            if (_group != null)
                return _group.TryGetAttribute(qname, out value);

            value = null;
            return false;
        }
    }
}