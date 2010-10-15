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
using System.Text;
using OpenFAST.Template;
using OpenFAST.Template.Operator;
using OpenFAST.util;
using Type = OpenFAST.Template.Type.FASTType;

namespace OpenFAST
{
    [Serializable]
    public class GroupValue : IFieldValue
    {
        private readonly Group _group;
        protected internal IFieldValue[] Values;

        public GroupValue(Group group, IFieldValue[] values)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            _group = group;
            Values = values;

            for (int i = 0; i < group.FieldCount; i++)
            {
                if (!(group.GetField(i) is Scalar)) continue;
                var scalar = ((Scalar) group.GetField(i));
                if (scalar.Operator.Equals(Operator.CONSTANT) && !scalar.Optional)
                {
                    values[i] = scalar.DefaultValue;
                }
            }
        }

        public GroupValue(Group group) : this(group, new IFieldValue[group.FieldCount])
        {
        }

        public virtual int FieldCount
        {
            get { return Values.Length; }
        }

        #region FieldValue Members

        public virtual IFieldValue Copy()
        {
            var copies = new IFieldValue[Values.Length];
            for (int i = 0; i < copies.Length; i++)
            {
                copies[i] = Values[i].Copy();
            }
            return new GroupValue(_group, Values);
        }

        #endregion

        public virtual int GetInt(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToInt();
        }

        public virtual int GetInt(string fieldName)
        {
            // BAD ABSTRACTION
            if (!_group.HasField(fieldName))
            {
                if (_group.HasIntrospectiveField(fieldName))
                {
                    Scalar scalar = _group.GetIntrospectiveField(fieldName);
                    if (scalar.Type.Equals(Type.UNICODE) || scalar.Type.Equals(Type.STRING) ||
                        scalar.Type.Equals(Type.ASCII))
                        return GetString(scalar.Name).Length;
                    if (scalar.Type.Equals(Type.BYTE_VECTOR))
                        return GetBytes(scalar.Name).Length;
                }
            }
            return GetScalar(fieldName).ToInt();
        }

        public virtual bool GetBool(string fieldName)
        {
            if (!IsDefined(fieldName))
                return false;
            return GetScalar(fieldName).ToInt() != 0;
        }

        public virtual long GetLong(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToLong();
        }

        public virtual long GetLong(string fieldName)
        {
            return GetScalar(fieldName).ToLong();
        }

        public virtual byte GetByte(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToByte();
        }

        public virtual byte GetByte(string fieldName)
        {
            return GetScalar(fieldName).ToByte();
        }

        public virtual short GetShort(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToShort();
        }

        public virtual short GetShort(string fieldName)
        {
            return GetScalar(fieldName).ToShort();
        }

        public virtual string GetString(int index)
        {
            return GetValue(index).ToString();
        }

        public virtual string GetString(string fieldName)
        {
            IFieldValue value = GetValue(fieldName);
            return (value == null) ? null : value.ToString();
        }

        public virtual double GetDouble(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToDouble();
        }

        public virtual double GetDouble(string fieldName)
        {
            return GetScalar(fieldName).ToDouble();
        }

        public virtual Decimal GetBigDecimal(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToBigDecimal();
        }

        public virtual Decimal GetBigDecimal(string fieldName)
        {
            return GetScalar(fieldName).ToBigDecimal();
        }

        public virtual byte[] GetBytes(int fieldIndex)
        {
            return GetScalar(fieldIndex).Bytes;
        }

        public virtual byte[] GetBytes(string fieldName)
        {
            return GetScalar(fieldName).Bytes;
        }

        public virtual SequenceValue GetSequence(int fieldIndex)
        {
            return (SequenceValue) GetValue(fieldIndex);
        }

        public virtual SequenceValue GetSequence(string fieldName)
        {
            return (SequenceValue) GetValue(fieldName);
        }

        public virtual ScalarValue GetScalar(int fieldIndex)
        {
            return (ScalarValue) GetValue(fieldIndex);
        }

        public virtual ScalarValue GetScalar(string fieldName)
        {
            return (ScalarValue) GetValue(fieldName);
        }

        public virtual GroupValue GetGroup(int fieldIndex)
        {
            return (GroupValue) GetValue(fieldIndex);
        }

        public virtual GroupValue GetGroup(string fieldName)
        {
            return (GroupValue) GetValue(fieldName);
        }

        public virtual IFieldValue GetValue(int fieldIndex)
        {
            return Values[fieldIndex];
        }

        public virtual IFieldValue GetValue(string fieldName)
        {
            if (!_group.HasField(fieldName))
            {
                throw new ArgumentException("The field \"" + fieldName + "\" does not exist in group " + _group);
            }

            return Values[_group.GetFieldIndex(fieldName)];
        }

        public virtual Group GetGroup()
        {
            return _group;
        }

        public virtual void SetString(Field field, string value)
        {
            if (field == null) throw new ArgumentNullException("field", "Field must not be null [value=" + value + "]");
            SetFieldValue(field, field.CreateValue(value));
        }

        public virtual void SetFieldValue(Field field, IFieldValue value)
        {
            SetFieldValue(_group.GetFieldIndex(field), value);
        }

        public virtual void SetFieldValue(int fieldIndex, IFieldValue value)
        {
            Values[fieldIndex] = value;
        }

        public virtual void SetBitVector(int fieldIndex, BitVector vector)
        {
            Values[fieldIndex] = new BitVectorValue(vector);
        }

        public virtual void SetByteVector(int fieldIndex, byte[] bytes)
        {
            Values[fieldIndex] = new ByteVectorValue(bytes);
        }

        public virtual void SetByteVector(string fieldName, byte[] bytes)
        {
            SetFieldValue(fieldName, new ByteVectorValue(bytes));
        }

        public virtual void SetDecimal(int fieldIndex, double value)
        {
            Values[fieldIndex] = new DecimalValue(value);
        }

        public virtual void SetDecimal(string fieldName, double value)
        {
            SetFieldValue(fieldName, new DecimalValue(value));
        }

        public virtual void SetDecimal(int fieldIndex, Decimal value)
        {
            Values[fieldIndex] = new DecimalValue(value);
        }

        public virtual void SetDecimal(string fieldName, Decimal value)
        {
            SetFieldValue(fieldName, new DecimalValue(value));
        }

        public virtual void SetInteger(string fieldName, int value)
        {
            SetFieldValue(fieldName, new IntegerValue(value));
        }

        public virtual void SetInteger(int fieldIndex, int value)
        {
            Values[fieldIndex] = new IntegerValue(value);
        }

        public virtual void SetBool(string fieldName, bool value)
        {
            SetFieldValue(fieldName, new IntegerValue(value ? 1 : 0));
        }

        public virtual void SetLong(string fieldName, long value)
        {
            SetFieldValue(fieldName, new LongValue(value));
        }

        public virtual void SetLong(int fieldIndex, long value)
        {
            Values[fieldIndex] = new LongValue(value);
        }

        public virtual void SetString(int fieldIndex, string value)
        {
            Values[fieldIndex] = new StringValue(value);
        }

        public virtual void SetString(string fieldName, string value)
        {
            SetFieldValue(fieldName, _group.GetField(fieldName).CreateValue(value));
        }

        public override bool Equals(Object other)
        {
            if (other == this)
            {
                return true;
            }

            if ((other == null) || !(other is GroupValue))
            {
                return false;
            }

            return Equals((GroupValue) other);
        }

        private bool Equals(GroupValue other)
        {
            if (Values.Length != other.Values.Length)
            {
                return false;
            }

            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i] == null)
                {
                    if (other.Values[i] != null)
                        return false;
                }
                else if (!Values[i].Equals(other.Values[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return Values.GetHashCode();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(_group).Append(" -> {");
            for (int i = 0; i < Values.Length; i++)
            {
                builder.Append(Values[i]).Append(", ");
            }

            builder.Append("}");
            return builder.ToString();
        }

        public virtual void SetFieldValue(string fieldName, IFieldValue value)
        {
            if (!_group.HasField(fieldName))
            {
                throw new ArgumentException("The field " + fieldName + " does not exist in group " + _group);
            }
            int index = _group.GetFieldIndex(fieldName);
            SetFieldValue(index, value);
        }

        public virtual void SetFieldValue(string fieldName, string value)
        {
            SetFieldValue(fieldName, _group.GetField(fieldName).CreateValue(value));
        }

        public virtual bool IsDefined(int fieldIndex)
        {
            return fieldIndex < Values.Length && Values[fieldIndex] != null;
        }

        public virtual bool IsDefined(string fieldName)
        {
            return GetValue(fieldName) != null;
        }
    }
}