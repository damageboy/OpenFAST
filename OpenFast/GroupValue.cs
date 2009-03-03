using System;
using Field = OpenFAST.Template.Field;
using Group = OpenFAST.Template.Group;
using LongValue = OpenFAST.Template.LongValue;
using Scalar = OpenFAST.Template.Scalar;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using Type = OpenFAST.Template.Type.FASTType;
using ArrayIterator = OpenFAST.util.ArrayIterator;
using System.Text;

namespace OpenFAST
{
	
	[Serializable]
	public class GroupValue : FieldValue
	{
		virtual public int FieldCount
		{
			get
			{
				return values.Length;
			}
			
		}
		private const long serialVersionUID = 1L;

        protected internal FieldValue[] values;
		
		private Group group;
		
		public GroupValue(Group group, FieldValue[] values)
		{
			if (group == null)
			{
				throw new System.NullReferenceException();
			}
			
			this.group = group;
			this.values = values;
			
			for (int i = 0; i < group.FieldCount; i++)
			{
				if (group.GetField(i) is Scalar)
				{
					Scalar scalar = ((Scalar) group.GetField(i));
					if (scalar.Operator.Equals(Operator.CONSTANT) && !scalar.Optional)
					{
						values[i] = scalar.DefaultValue;
					}
				}
			}
		}
		
		public GroupValue(Group group):this(group, new FieldValue[group.FieldCount])
		{
		}
		
		public virtual System.Collections.IEnumerator Iterator()
		{
			return new ArrayIterator(values);
		}
		
		public virtual int GetInt(int fieldIndex)
		{
			return GetScalar(fieldIndex).ToInt();
		}
		
		public virtual int GetInt(string fieldName)
		{
			// BAD ABSTRACTION
			if (!group.HasField(fieldName))
			{
				if (group.HasIntrospectiveField(fieldName))
				{
					Scalar scalar = group.GetIntrospectiveField(fieldName);
					if (scalar.Type.Equals(Type.UNICODE) || scalar.Type.Equals(Type.STRING) || scalar.Type.Equals(Type.ASCII))
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
			FieldValue value_Renamed = GetValue(fieldName);
			return (value_Renamed == null)?null:value_Renamed.ToString();
		}
		
		public virtual double GetDouble(int fieldIndex)
		{
			return GetScalar(fieldIndex).ToDouble();
		}
		
		public virtual double GetDouble(string fieldName)
		{
			return GetScalar(fieldName).ToDouble();
		}
		
		public virtual System.Decimal GetBigDecimal(int fieldIndex)
		{
			return GetScalar(fieldIndex).ToBigDecimal();
		}
		
		public virtual System.Decimal GetBigDecimal(string fieldName)
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
		
		public virtual FieldValue GetValue(int fieldIndex)
		{
			return values[fieldIndex];
		}
		
		public virtual FieldValue GetValue(string fieldName)
		{
			if (!group.HasField(fieldName))
			{
				throw new System.ArgumentException("The field \"" + fieldName + "\" does not exist in group " + group);
			}
			
			return values[group.GetFieldIndex(fieldName)];
		}
		
		public virtual Group GetGroup()
		{
			return group;
		}
		
		public virtual void  SetString(Field field, string value_Renamed)
		{
			if (field == null)
				throw new System.ArgumentException("Field must not be null [value=" + value_Renamed + "]");
			SetFieldValue(field, field.CreateValue(value_Renamed));
		}
		
		public virtual void  SetFieldValue(Field field, FieldValue value_Renamed)
		{
			SetFieldValue(group.GetFieldIndex(field), value_Renamed);
		}
		
		public virtual void  SetFieldValue(int fieldIndex, FieldValue value_Renamed)
		{
			values[fieldIndex] = value_Renamed;
		}
		
		public virtual void  SetBitVector(int fieldIndex, BitVector vector)
		{
			values[fieldIndex] = new BitVectorValue(vector);
		}
		
		public virtual void  SetByteVector(int fieldIndex, byte[] bytes)
		{
			values[fieldIndex] = new ByteVectorValue(bytes);
		}
		
		public virtual void  SetByteVector(string fieldName, byte[] bytes)
		{
			SetFieldValue(fieldName, new ByteVectorValue(bytes));
		}
		
		public virtual void  SetDecimal(int fieldIndex, double value_Renamed)
		{
			values[fieldIndex] = new DecimalValue(value_Renamed);
		}
		
		public virtual void  SetDecimal(string fieldName, double value_Renamed)
		{
			SetFieldValue(fieldName, new DecimalValue(value_Renamed));
		}
		
		public virtual void  SetDecimal(int fieldIndex, System.Decimal value_Renamed)
		{
			values[fieldIndex] = new DecimalValue(value_Renamed);
		}
		
		public virtual void  SetDecimal(string fieldName, System.Decimal value_Renamed)
		{
			SetFieldValue(fieldName, new DecimalValue(value_Renamed));
		}
		
		public virtual void  SetInteger(string fieldName, int value_Renamed)
		{
			SetFieldValue(fieldName, new IntegerValue(value_Renamed));
		}
		
		public virtual void  SetInteger(int fieldIndex, int value_Renamed)
		{
			values[fieldIndex] = new IntegerValue(value_Renamed);
		}
		
		public virtual void  SetBool(string fieldName, bool value_Renamed)
		{
			SetFieldValue(fieldName, new IntegerValue(value_Renamed?1:0));
		}
		
		public virtual void  SetLong(string fieldName, long value_Renamed)
		{
			SetFieldValue(fieldName, new LongValue(value_Renamed));
		}
		
		public virtual void  SetLong(int fieldIndex, long value_Renamed)
		{
			values[fieldIndex] = new LongValue(value_Renamed);
		}
		
		public virtual void  SetString(int fieldIndex, string value_Renamed)
		{
			values[fieldIndex] = new StringValue(value_Renamed);
		}
		
		public virtual void  SetString(string fieldName, string value_Renamed)
		{
			SetFieldValue(fieldName, group.GetField(fieldName).CreateValue(value_Renamed));
		}
		
		public  override bool Equals(System.Object other)
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
			if (values.Length != other.values.Length)
			{
				return false;
			}
			
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] == null)
				{
					if (other.values[i] != null)
						return false;
				}
				else if (!values[i].Equals(other.values[i]))
				{
					return false;
				}
			}
			
			return true;
		}
		
		public override int GetHashCode()
		{
			return values.GetHashCode();
		}
		
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(group).Append(" -> {");
			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i]).Append(", ");
			}

            builder.Append("}");
			return builder.ToString();
		}
		
		public virtual void  SetFieldValue(string fieldName, FieldValue value_Renamed)
		{
			if (!group.HasField(fieldName))
			{
				throw new System.ArgumentException("The field " + fieldName + " does not exist in group " + group);
			}
			int index = group.GetFieldIndex(fieldName);
			SetFieldValue(index, value_Renamed);
		}
		
		public virtual void  SetFieldValue(string fieldName, string value_Renamed)
		{
			SetFieldValue(fieldName, group.GetField(fieldName).CreateValue(value_Renamed));
		}
		
		public virtual bool IsDefined(int fieldIndex)
		{
			return fieldIndex < values.Length && values[fieldIndex] != null;
		}
		
		public virtual bool IsDefined(string fieldName)
		{
			return GetValue(fieldName) != null;
		}
		
		public virtual FieldValue Copy()
		{
			FieldValue[] copies = new FieldValue[values.Length];
			for (int i = 0; i < copies.Length; i++)
			{
				copies[i] = values[i].Copy();
			}
			return new GroupValue(group, this.values);
		}
	}
}