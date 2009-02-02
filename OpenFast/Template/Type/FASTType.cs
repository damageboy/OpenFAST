using System;
using Global = OpenFAST.Global;
using ScalarValue = OpenFAST.ScalarValue;
using LongValue = OpenFAST.Template.LongValue;
using Operator = OpenFAST.Template.operator_Renamed.Operator;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;
using Util = OpenFAST.util.Util;
using OpenFAST;
using System.Collections;

namespace OpenFAST.Template.Type
{
	
	[Serializable]
	public abstract class FASTType
	{
		virtual public string Name
		{
			get
			{
				return name;
			}
			
		}
		public abstract ScalarValue DefaultValue{get;}
		public static System.Collections.IDictionary RegisteredTypeMap
		{
			get
			{
				return TYPE_NAME_MAP;
			}
			
		}
        private static readonly System.Collections.IDictionary TYPE_NAME_MAP = new Hashtable();
		private string name;
		
		public FASTType(string typeName)
		{
			this.name = typeName;
			TYPE_NAME_MAP[typeName] = this;
		}

		public static FASTType GetType(string typeName)
		{
			if (!TYPE_NAME_MAP.Contains(typeName))
			{
				throw new System.ArgumentException("The type named " + typeName + " does not exist.  Existing types are " + Util.CollectionToString(new SupportClass.HashSetSupport(TYPE_NAME_MAP.Keys)));
			}
			return (FASTType) TYPE_NAME_MAP[typeName];
		}

		public override string ToString()
		{
			return name;
		}
		public virtual string Serialize(ScalarValue value_Renamed)
		{
			return value_Renamed.ToString();
		}
		public abstract TypeCodec GetCodec(Operator operator_Renamed, bool optional);
		public abstract ScalarValue GetValue(string value_Renamed);
		public abstract bool IsValueOf(ScalarValue previousValue);
		public virtual void  ValidateValue(ScalarValue value_Renamed)
		{
		}

		public static readonly FASTType U8 = new UnsignedIntegerType(8, 256);
		public static readonly FASTType U16 = new UnsignedIntegerType(16, 65536);
		public static readonly FASTType U32 = new UnsignedIntegerType(32, 4294967295L);
		public static readonly FASTType U64 = new UnsignedIntegerType(64, System.Int64.MaxValue);
		public static readonly FASTType I8 = new SignedIntegerType(8,  System.SByte.MinValue,  System.SByte.MaxValue);
		public static readonly FASTType I16 = new SignedIntegerType(16, System.Int16.MinValue, System.Int16.MaxValue);
		public static readonly FASTType I32 = new SignedIntegerType(32, System.Int32.MinValue, System.Int32.MaxValue);
		public static readonly FASTType I64 = new SignedIntegerType(64, System.Int64.MinValue, System.Int64.MaxValue);
        public static readonly FASTType STRING;
		public static readonly FASTType ASCII;
		public static readonly FASTType UNICODE;
		public static readonly FASTType BYTE_VECTOR = new ByteVectorType();
		public static readonly FASTType DECIMAL = new DecimalType();

        static FASTType[] staticAllTypes = null;
        public static FASTType[] ALL_TYPES()//OVERLOOK
        {
            if (staticAllTypes == null)
            {
                staticAllTypes = new FASTType[] { U8, U16, U32, U64, I8, I16, I32, I64, STRING, ASCII, UNICODE, BYTE_VECTOR, DECIMAL };

            }
            return staticAllTypes;
        }
        
		public static readonly FASTType[] INTEGER_TYPES = new FASTType[]{U8, U16, U32, U64, I8, I16, I32, I64};
		public  override bool Equals(System.Object obj)
		{
			if (obj == null)
				return false;
			return obj.GetType().Equals(this.GetType());
		}
		public override int GetHashCode()
		{
			return name.GetHashCode();
		}
		static FASTType()
		{
			STRING = new StringType("string", TypeCodec.ASCII, TypeCodec.NULLABLE_ASCII);
			ASCII = new StringType("ascii", TypeCodec.ASCII, TypeCodec.NULLABLE_ASCII);
			UNICODE = new StringType("unicode", TypeCodec.UNICODE, TypeCodec.NULLABLE_UNICODE);
		}
	}
}