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
using System.IO;
using OpenFAST.Error;
using OpenFAST.Template.Type;

namespace OpenFAST.Template
{
    [Serializable]
    public class Sequence : Field, FieldSet
    {
        private readonly Group group;
        private readonly bool implicitLength;
        private readonly Scalar length;

        public Sequence(QName name, Field[] fields, bool optional)
            : this(name, CreateLength(name, optional), fields, optional)
        {
            implicitLength = true;
        }

        public Sequence(string name, Field[] fields, bool optional) : this(new QName(name), fields, optional)
        {
        }

        public Sequence(QName name, Scalar length, Field[] fields, bool optional) : base(name, optional)
        {
            group = new Group(name, fields, optional);

            if (length == null)
            {
                this.length = CreateLength(name, optional);
                implicitLength = true;
            }
            else
            {
                this.length = length;
            }
        }

        public virtual Scalar Length
        {
            get { return length; }
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
            get { return group; }
        }

        public virtual bool ImplicitLength
        {
            get { return implicitLength; }
        }

        public virtual QName TypeReference
        {
            get { return group.TypeReference; }

            set { group.TypeReference = value; }
        }

        #region FieldSet Members

        public virtual int FieldCount
        {
            get { return group.FieldCount; }
        }

        public virtual Field GetField(int index)
        {
            return group.GetField(index);
        }

        #endregion

        private static Scalar CreateLength(QName name, bool optional)
        {
            return new Scalar(Global.CreateImplicitName(name), FASTType.U32, Operator.Operator.NONE,
                              ScalarValue.UNDEFINED, optional);
        }

        public override bool UsesPresenceMapBit()
        {
            return length.UsesPresenceMapBit();
        }

        public override bool IsPresenceMapBitSet(byte[] encoding, FieldValue fieldValue)
        {
            return length.IsPresenceMapBitSet(encoding, fieldValue);
        }

        public override byte[] Encode(FieldValue value_Renamed, Group encodeTemplate, Context context,
                                      BitVectorBuilder presenceMapBuilder)
        {
            if (HasTypeReference())
                context.CurrentApplicationType = TypeReference;
            if (value_Renamed == null)
            {
                return length.Encode(null, encodeTemplate, context, presenceMapBuilder);
            }

            var buffer = new MemoryStream();
            var val = (SequenceValue) value_Renamed;
            int len = val.Length;

            try
            {
                byte[] temp_byteArray = length.Encode(new IntegerValue(len), encodeTemplate, context, presenceMapBuilder);
                buffer.Write(temp_byteArray, 0, temp_byteArray.Length);

                IEnumerator iter = val.Iterator();

                while (iter.MoveNext())
                {
                    byte[] temp_byteArray2 = group.Encode((FieldValue) iter.Current, encodeTemplate, context);
                    buffer.Write(temp_byteArray2, 0, temp_byteArray2.Length);
                }
            }
            catch (IOException e)
            {
                Global.HandleError(FastConstants.IO_ERROR, "An IO error occurred while encoding " + this, e);
            }

            return buffer.ToArray();
        }

        public override FieldValue Decode(Stream in_Renamed, Group decodeTemplate, Context context,
                                          BitVectorReader pmapReader)
        {
            var sequenceValue = new SequenceValue(this);
            FieldValue lengthValue = length.Decode(in_Renamed, decodeTemplate, context, pmapReader);

            if ((lengthValue == ScalarValue.NULL) || (lengthValue == null))
            {
                return null;
            }

            int len = ((IntegerValue) lengthValue).value_Renamed;

            for (int i = 0; i < len; i++)
                sequenceValue.Add(
                    (GroupValue) group.Decode(in_Renamed, decodeTemplate, context, BitVectorReader.INFINITE_TRUE));

            return sequenceValue;
        }

        public override FieldValue CreateValue(string value_Renamed)
        {
            return new SequenceValue(this);
        }

        public virtual bool HasField(string fieldName)
        {
            return group.HasField(fieldName);
        }

        public virtual bool HasTypeReference()
        {
            return group.HasTypeReference();
        }

        public override string ToString()
        {
            return name.Name;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime*result + ((group == null) ? 0 : group.GetHashCode());
            result = prime*result + ((length == null) ? 0 : length.GetHashCode());
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (Sequence) obj;
            if (!group.Equals(other.group))
                return false;
            if (ImplicitLength != other.ImplicitLength)
                return false;
            if (!ImplicitLength && !length.Equals(other.length))
                return false;
            return true;
        }

        public override bool HasAttribute(QName attributeName)
        {
            return group.HasAttribute(attributeName);
        }

        public override string GetAttribute(QName qname)
        {
            return group.GetAttribute(qname);
        }
    }
}