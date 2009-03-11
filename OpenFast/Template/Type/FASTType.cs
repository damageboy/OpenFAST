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
using Operator = openfast.Template.Operator.Operator;
using TypeCodec = OpenFAST.Template.Type.Codec.TypeCodec;
using Util = OpenFAST.util.Util;
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
		public static IDictionary RegisteredTypeMap
		{
			get
			{
				return TYPE_NAME_MAP;
			}
			
		}
        private static readonly IDictionary TYPE_NAME_MAP = new Hashtable();
		private readonly string name;

	    protected FASTType(string typeName)
		{
			name = typeName;
			TYPE_NAME_MAP[typeName] = this;
		}

		public static FASTType GetType(string typeName)
		{
			if (!TYPE_NAME_MAP.Contains(typeName))
			{
				throw new ArgumentException("The type named " + typeName + " does not exist.  Existing types are " + Util.CollectionToString(new SupportClass.HashSetSupport(TYPE_NAME_MAP.Keys)));
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
		public static readonly FASTType U64 = new UnsignedIntegerType(64, Int64.MaxValue);
		public static readonly FASTType I8 = new SignedIntegerType(8,  SByte.MinValue,  SByte.MaxValue);
		public static readonly FASTType I16 = new SignedIntegerType(16, Int16.MinValue, Int16.MaxValue);
		public static readonly FASTType I32 = new SignedIntegerType(32, Int32.MinValue, Int32.MaxValue);
		public static readonly FASTType I64 = new SignedIntegerType(64, Int64.MinValue, Int64.MaxValue);
        public static readonly FASTType STRING;
		public static readonly FASTType ASCII;
		public static readonly FASTType UNICODE;
		public static readonly FASTType BYTE_VECTOR = new ByteVectorType();
		public static readonly FASTType DECIMAL = new DecimalType();

        static FASTType[] staticAllTypes;
        public static FASTType[] ALL_TYPES()
        {
            if (staticAllTypes == null)
            {
                staticAllTypes = new[] { U8, U16, U32, U64, I8, I16, I32, I64, STRING, ASCII, UNICODE, BYTE_VECTOR, DECIMAL };

            }
            return staticAllTypes;
        }
        
		public static readonly FASTType[] INTEGER_TYPES = new[]{U8, U16, U32, U64, I8, I16, I32, I64};
		public  override bool Equals(Object obj)
		{
			if (obj == null)
				return false;
			return obj.GetType().Equals(GetType());
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