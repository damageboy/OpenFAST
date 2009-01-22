using System;
using BitVectorReader = OpenFAST.BitVectorReader;
using QName = OpenFAST.QName;
using BitVectorBuilder = OpenFAST.BitVectorBuilder;
using Context = OpenFAST.Context;
using FieldValue = OpenFAST.FieldValue;
using Global = OpenFAST.Global;
using GroupValue = OpenFAST.GroupValue;
using IntegerValue = OpenFAST.IntegerValue;
using ScalarValue = OpenFAST.ScalarValue;
using SequenceValue = OpenFAST.SequenceValue;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template
{
	
	
	[Serializable]
	public class Sequence:Field, FieldSet
	{
		virtual public int FieldCount
		{
			get
			{
				return group.FieldCount;
			}
			
		}
		virtual public Scalar Length
		{
			get
			{
				return length;
			}
			
		}
		override public System.Type ValueType
		{
			get
			{
				return typeof(SequenceValue);
			}
			
		}
		override public string TypeName
		{
			get
			{
				return "sequence";
			}
			
		}
		virtual public Group Group
		{
			get
			{
				return group;
			}
			
		}
		virtual public bool ImplicitLength
		{
			get
			{
				return implicitLength;
			}
			
		}
		virtual public QName TypeReference
		{
			get
			{
				return group.TypeReference;
			}
			
			set
			{
				this.group.TypeReference = value;
			}
			
		}
		private const long serialVersionUID = 1L;
		private Group group;
		private Scalar length;
		private bool implicitLength;
		
		public Sequence(QName name, Field[] fields, bool optional):this(name, CreateLength(name, optional), fields, optional)
		{
			implicitLength = true;
		}
		
		public Sequence(string name, Field[] fields, bool optional):this(new QName(name), fields, optional)
		{
		}
		
		public Sequence(QName name, Scalar length, Field[] fields, bool optional):base(name, optional)
		{
			this.group = new Group(name, fields, optional);
			
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
		
		private static Scalar CreateLength(QName name, bool optional)
		{
			return new Scalar(Global.CreateImplicitName(name), FASTType.U32, Operator.NONE, ScalarValue.UNDEFINED, optional);
		}
		
		public virtual Field GetField(int index)
		{
			return group.GetField(index);
		}

		public override bool UsesPresenceMapBit()
		{
			return length.UsesPresenceMapBit();
		}
		
		public override bool IsPresenceMapBitSet(sbyte[] encoding, FieldValue fieldValue)
		{
			return length.IsPresenceMapBitSet(encoding, fieldValue);
		}
		
		public override sbyte[] Encode(FieldValue value_Renamed, Group template, Context context, BitVectorBuilder presenceMapBuilder)
		{
			if (HasTypeReference())
				context.CurrentApplicationType = TypeReference;
			if (value_Renamed == null)
			{
				return length.Encode(null, template, context, presenceMapBuilder);
			}
			
			System.IO.MemoryStream buffer = new System.IO.MemoryStream();
			SequenceValue val = (SequenceValue) value_Renamed;
			int len = val.Length;
			
			try
			{
				sbyte[] temp_sbyteArray;
				temp_sbyteArray = length.Encode(new IntegerValue(len), template, context, presenceMapBuilder);
				buffer.Write(SupportClass.ToByteArray(temp_sbyteArray), 0, temp_sbyteArray.Length);
				
				System.Collections.IEnumerator iter = val.Iterator();

                while (iter.MoveNext())
				{
					sbyte[] temp_sbyteArray2;
					temp_sbyteArray2 = group.Encode((FieldValue) iter.Current, template, context);
					buffer.Write(SupportClass.ToByteArray(temp_sbyteArray2), 0, temp_sbyteArray2.Length);
				}
			}
			catch (System.IO.IOException e)
			{
				Global.HandleError(OpenFAST.Error.FastConstants.IO_ERROR, "An IO error occurred while encoding " + this, e);
			}
			
			return SupportClass.ToSByteArray(buffer.ToArray());
		}
		
		public override FieldValue Decode(System.IO.Stream in_Renamed, Group template, Context context, BitVectorReader pmapReader)
		{
			SequenceValue sequenceValue = new SequenceValue(this);
			FieldValue lengthValue = length.Decode(in_Renamed, template, context, pmapReader);
			
			if ((lengthValue == ScalarValue.NULL) || (lengthValue == null))
			{
				return null;
			}
			
			int len = ((IntegerValue) lengthValue).value_Renamed;
			
			for (int i = 0; i < len; i++)
				sequenceValue.Add((GroupValue) group.Decode(in_Renamed, template, context, BitVectorReader.INFINITE_TRUE));
			
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
			int prime = 31;
			int result = 1;
			result = prime * result + ((group == null)?0:group.GetHashCode());
			result = prime * result + ((length == null)?0:length.GetHashCode());
			return result;
		}
		
		public  override bool Equals(System.Object obj)
		{
			if (this == obj)
				return true;
			if (obj == null || GetType() != obj.GetType())
				return false;

			Sequence other = (Sequence) obj;
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
		
		public override string GetAttribute(QName name)
		{
			return group.GetAttribute(name);
		}
	}
}