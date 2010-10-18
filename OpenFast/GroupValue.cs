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
using System.Text;
using OpenFAST.Template;
using OpenFAST.Template.Operator;
using OpenFAST.Utility;
using Type = OpenFAST.Template.Type.FASTType;

namespace OpenFAST
{
    [Serializable]
    public class GroupValue : IFieldValue, IEquatable<GroupValue>
    {
        private readonly Group _group;
        private readonly IFieldValue[] _values;

        public GroupValue(Group group, IFieldValue[] values)
        {
            if (group == null) throw new ArgumentNullException("group");
            if (values == null) throw new ArgumentNullException("values");

            _group = group;
            _values = values;

            Field[] flds = group.Fields;
            for (int i = 0; i < flds.Length; i++)
            {
                var scalar = flds[i] as Scalar;
                if (scalar != null && scalar.Operator.Equals(Operator.CONSTANT) && !scalar.IsOptional)
                {
                    values[i] = scalar.DefaultValue;
                }
            }
        }

        public GroupValue(Group group)
            : this(group, new IFieldValue[group.FieldCount])
        {
        }

        public virtual int FieldCount
        {
            get { return _values.Length; }
        }

        public IFieldValue[] Values
        {
            get { return _values; }
        }

        #region IFieldValue Members

        public virtual IFieldValue Copy()
        {
            var copies = new IFieldValue[_values.Length];
            for (int i = 0; i < copies.Length; i++)
                copies[i] = _values[i].Copy();
            return new GroupValue(_group, copies);
        }

        #endregion

        public virtual int GetInt(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToInt();
        }

        public virtual int GetInt(string fieldName)
        {
            // BAD ABSTRACTION
            Field fld;
            if (!_group.TryGetField(fieldName, out fld))
            {
                Scalar scalar;
                if (_group.TryGetIntrospectiveField(fieldName, out scalar))
                {
                    if (scalar.Type.Equals(Type.UNICODE) ||
                        scalar.Type.Equals(Type.STRING) ||
                        scalar.Type.Equals(Type.ASCII))
                        return GetString(scalar.Name).Length;

                    if (scalar.Type.Equals(Type.BYTE_VECTOR))
                        return GetBytes(scalar.Name).Length;
                }
            }

            return ((ScalarValue) GetValue(fld)).ToInt();
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
            return _values[fieldIndex];
        }

        public virtual IFieldValue GetValue(Field field)
        {
            return _values[_group.GetFieldIndex(field)];
        }

        public virtual IFieldValue GetValue(string fieldName)
        {
            return _values[_group.GetFieldIndex(fieldName)];
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
            _values[fieldIndex] = value;
        }

        public virtual void SetBitVector(int fieldIndex, BitVector vector)
        {
            _values[fieldIndex] = new BitVectorValue(vector);
        }

        public virtual void SetByteVector(int fieldIndex, byte[] bytes)
        {
            _values[fieldIndex] = new ByteVectorValue(bytes);
        }

        public virtual void SetByteVector(string fieldName, byte[] bytes)
        {
            SetFieldValue(fieldName, new ByteVectorValue(bytes));
        }

        public virtual void SetDecimal(int fieldIndex, double value)
        {
            _values[fieldIndex] = new DecimalValue(value);
        }

        public virtual void SetDecimal(string fieldName, double value)
        {
            SetFieldValue(fieldName, new DecimalValue(value));
        }

        public virtual void SetDecimal(int fieldIndex, Decimal value)
        {
            _values[fieldIndex] = new DecimalValue(value);
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
            _values[fieldIndex] = new IntegerValue(value);
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
            _values[fieldIndex] = new LongValue(value);
        }

        public virtual void SetString(int fieldIndex, string value)
        {
            _values[fieldIndex] = new StringValue(value);
        }

        public virtual void SetString(string fieldName, string value)
        {
            SetFieldValue(fieldName, _group.GetField(fieldName).CreateValue(value));
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(_group).Append(" -> {");
            foreach (var v in _values)
                builder.Append(v).Append(", ");
            builder.Append("}");

            return builder.ToString();
        }

        public virtual void SetFieldValue(string fieldName, IFieldValue value)
        {
            Field fld;
            if (!_group.TryGetField(fieldName, out fld))
                throw new ArgumentOutOfRangeException("fieldName", fieldName,
                                                      "Field does not exist in group " + _group);

            SetFieldValue(_group.GetFieldIndex(fld), value);
        }

        public virtual void SetFieldValue(string fieldName, string value)
        {
            SetFieldValue(fieldName, _group.GetField(fieldName).CreateValue(value));
        }

        public virtual bool IsDefined(int fieldIndex)
        {
            return fieldIndex < _values.Length && _values[fieldIndex] != null;
        }

        public virtual bool IsDefined(string fieldName)
        {
            return GetValue(fieldName) != null;
        }

        #region Equals

        public bool Equals(GroupValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            // BUG? for some reason we are ignoring this._groups
            return Util.ArrayEqualsSlow(_values, other._values, 0);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GroupValue)) return false;
            return Equals((GroupValue) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // BUG? for some reason we are ignoring this._groups
                return Util.ArrayHashCode(_values);
            }
        }

//
//        public override int GetHashCode()
//        {
//            return _values.GetHashCode();
//        }
//

        #endregion
    }
}