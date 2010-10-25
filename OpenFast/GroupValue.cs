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
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;
using OpenFAST.Utility;

namespace OpenFAST
{
    [Serializable]
    public class GroupValue : IFieldValue
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
                if (scalar != null && scalar.Operator.Equals(Operator.Constant) && !scalar.IsOptional)
                {
                    values[i] = scalar.DefaultValue;
                }
            }
        }

        public GroupValue(Group group)
            : this(group, new IFieldValue[group.FieldCount])
        {
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

        public int GetInt(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToInt();
        }

        public int GetInt(string fieldName)
        {
            // BAD ABSTRACTION
            Field fld;
            if (!_group.TryGetField(fieldName, out fld))
            {
                Scalar scalar;
                if (_group.TryGetIntrospectiveField(fieldName, out scalar))
                {
                    if (scalar.FASTType.Equals(FastType.Unicode) ||
                        scalar.FASTType.Equals(FastType.String) ||
                        scalar.FASTType.Equals(FastType.Ascii))
                        return GetString(scalar.Name).Length;

                    if (scalar.FASTType.Equals(FastType.ByteVector))
                        return GetBytes(scalar.Name).Length;
                }
            }

            return ((ScalarValue) GetValue(fld)).ToInt();
        }

        public bool GetBool(string fieldName)
        {
            if (!IsDefined(fieldName))
                return false;
            return GetScalar(fieldName).ToInt() != 0;
        }

        public long GetLong(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToLong();
        }

        public long GetLong(string fieldName)
        {
            return GetScalar(fieldName).ToLong();
        }

        public byte GetByte(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToByte();
        }

        public byte GetByte(string fieldName)
        {
            return GetScalar(fieldName).ToByte();
        }

        public short GetShort(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToShort();
        }

        public short GetShort(string fieldName)
        {
            return GetScalar(fieldName).ToShort();
        }

        public string GetString(int index)
        {
            return GetValue(index).ToString();
        }

        public string GetString(string fieldName)
        {
            IFieldValue value = GetValue(fieldName);
            return (value == null) ? null : value.ToString();
        }

        public double GetDouble(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToDouble();
        }

        public double GetDouble(string fieldName)
        {
            return GetScalar(fieldName).ToDouble();
        }

        public Decimal GetBigDecimal(int fieldIndex)
        {
            return GetScalar(fieldIndex).ToBigDecimal();
        }

        public Decimal GetBigDecimal(string fieldName)
        {
            return GetScalar(fieldName).ToBigDecimal();
        }

        public byte[] GetBytes(int fieldIndex)
        {
            return GetScalar(fieldIndex).Bytes;
        }

        public byte[] GetBytes(string fieldName)
        {
            return GetScalar(fieldName).Bytes;
        }

        public SequenceValue GetSequence(int fieldIndex)
        {
            return (SequenceValue) GetValue(fieldIndex);
        }

        public SequenceValue GetSequence(string fieldName)
        {
            return (SequenceValue) GetValue(fieldName);
        }

        public ScalarValue GetScalar(int fieldIndex)
        {
            return (ScalarValue) GetValue(fieldIndex);
        }

        public ScalarValue GetScalar(string fieldName)
        {
            return (ScalarValue) GetValue(fieldName);
        }

        public GroupValue GetGroup(int fieldIndex)
        {
            return (GroupValue) GetValue(fieldIndex);
        }

        public GroupValue GetGroup(string fieldName)
        {
            return (GroupValue) GetValue(fieldName);
        }

        public IFieldValue GetValue(int fieldIndex)
        {
            return _values[fieldIndex];
        }

        public IFieldValue GetValue(Field field)
        {
            return _values[_group.GetFieldIndex(field)];
        }

        public IFieldValue GetValue(string fieldName)
        {
            return _values[_group.GetFieldIndex(fieldName)];
        }

        public bool TryGetValue(string fieldName, out IFieldValue value)
        {
            int index;
            if (_group.TryGetFieldIndex(fieldName, out index))
            {
                value = _values[index];
                return true;
            }
            value = null;
            return false;
        }

        public Group Group
        {
            get { return _group; }
        }

        public void SetString(Field field, string value)
        {
            if (field == null) throw new ArgumentNullException("field", "Field must not be null [value=" + value + "]");
            SetFieldValue(field, field.CreateValue(value));
        }

        public void SetFieldValue(Field field, IFieldValue value)
        {
            SetFieldValue(_group.GetFieldIndex(field), value);
        }

        public virtual void SetFieldValue(int fieldIndex, IFieldValue value)
        {
            _values[fieldIndex] = value;
        }

        public void SetBitVector(int fieldIndex, BitVector vector)
        {
            _values[fieldIndex] = new BitVectorValue(vector);
        }

        public void SetByteVector(int fieldIndex, byte[] bytes)
        {
            _values[fieldIndex] = new ByteVectorValue(bytes);
        }

        public void SetByteVector(string fieldName, byte[] bytes)
        {
            SetFieldValue(fieldName, new ByteVectorValue(bytes));
        }

        public void SetDecimal(int fieldIndex, double value)
        {
            _values[fieldIndex] = new DecimalValue(value);
        }

        public void SetDecimal(string fieldName, double value)
        {
            SetFieldValue(fieldName, new DecimalValue(value));
        }

        public void SetDecimal(int fieldIndex, Decimal value)
        {
            _values[fieldIndex] = new DecimalValue(value);
        }

        public void SetDecimal(string fieldName, Decimal value)
        {
            SetFieldValue(fieldName, new DecimalValue(value));
        }

        public void SetInteger(string fieldName, int value)
        {
            SetFieldValue(fieldName, new IntegerValue(value));
        }

        public void SetInteger(int fieldIndex, int value)
        {
            _values[fieldIndex] = new IntegerValue(value);
        }

        public void SetBool(string fieldName, bool value)
        {
            SetFieldValue(fieldName, new IntegerValue(value ? 1 : 0));
        }

        public void SetLong(string fieldName, long value)
        {
            SetFieldValue(fieldName, new LongValue(value));
        }

        public void SetLong(int fieldIndex, long value)
        {
            _values[fieldIndex] = new LongValue(value);
        }

        public void SetString(int fieldIndex, string value)
        {
            _values[fieldIndex] = new StringValue(value);
        }

        public void SetString(string fieldName, string value)
        {
            var field = _group.GetField(fieldName);
            SetFieldValue(field, field.CreateValue(value));
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(_group).Append(" -> {");
            foreach (IFieldValue v in _values)
                builder.Append(v).Append(", ");
            builder.Append("}");

            return builder.ToString();
        }

        public void SetFieldValue(string fieldName, IFieldValue value)
        {
            Field fld;
            if (!_group.TryGetField(fieldName, out fld))
                throw new ArgumentOutOfRangeException("fieldName", fieldName,
                                                      "Field does not exist in group " + _group);

            SetFieldValue(_group.GetFieldIndex(fld), value);
        }

        public void SetFieldValue(string fieldName, string value)
        {
            var field = _group.GetField(fieldName);
            SetFieldValue(field, field.CreateValue(value));
        }

        public bool IsDefined(int fieldIndex)
        {
            return fieldIndex < _values.Length && _values[fieldIndex] != null;
        }

        public bool IsDefined(string fieldName)
        {
            IFieldValue ret;
            return TryGetValue(fieldName, out ret) && ret != null;
        }

        #region Equals

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            
            var other = obj as GroupValue;
            if (ReferenceEquals(null, other)) return false;
            
#warning BUG? for some reason we are ignoring this._groups
            return Util.ArrayEqualsSlow(_values, other._values, 0);
        }

        public override int GetHashCode()
        {
            unchecked
            {
#warning BUG? for some reason we are ignoring this._groups
                return Util.GetCollectionHashCode(_values);
            }
        }

        #endregion
    }
}