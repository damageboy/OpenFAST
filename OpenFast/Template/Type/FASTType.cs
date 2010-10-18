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
using System.Collections.Generic;
using OpenFAST.Template.Type.Codec;
using OpenFAST.Utility;

namespace OpenFAST.Template.Type
{
    [Serializable]
    public abstract class FASTType : IEquatable<FASTType>
    {
        private static readonly Dictionary<string, FASTType> TypeNameMap = new Dictionary<string, FASTType>();

        public static readonly FASTType U8 = new UnsignedIntegerType(8, 256);
        public static readonly FASTType U16 = new UnsignedIntegerType(16, 65536);
        public static readonly FASTType U32 = new UnsignedIntegerType(32, 4294967295L);
        public static readonly FASTType U64 = new UnsignedIntegerType(64, Int64.MaxValue);
        public static readonly FASTType I8 = new SignedIntegerType(8, SByte.MinValue, SByte.MaxValue);
        public static readonly FASTType I16 = new SignedIntegerType(16, Int16.MinValue, Int16.MaxValue);
        public static readonly FASTType I32 = new SignedIntegerType(32, Int32.MinValue, Int32.MaxValue);
        public static readonly FASTType I64 = new SignedIntegerType(64, Int64.MinValue, Int64.MaxValue);
        public static readonly FASTType STRING;
        public static readonly FASTType ASCII;
        public static readonly FASTType UNICODE;
        public static readonly FASTType BYTE_VECTOR = new ByteVectorType();
        public static readonly FASTType DECIMAL = new DecimalType();

        private static FASTType[] _staticAllTypes;
        public static readonly FASTType[] INTEGER_TYPES = new[] {U8, U16, U32, U64, I8, I16, I32, I64};

        private readonly string _name;

        static FASTType()
        {
            STRING = new StringType("string", TypeCodec.ASCII, TypeCodec.NULLABLE_ASCII);
            ASCII = new StringType("ascii", TypeCodec.ASCII, TypeCodec.NULLABLE_ASCII);
            UNICODE = new StringType("unicode", TypeCodec.UNICODE, TypeCodec.NULLABLE_UNICODE);
        }

        protected FASTType(string typeName)
        {
            if (typeName == null) throw new ArgumentNullException("typeName");
            _name = typeName;
            TypeNameMap[typeName] = this;
        }

        public virtual string Name
        {
            get { return _name; }
        }

        public abstract ScalarValue DefaultValue { get; }

        public static Dictionary<string, FASTType> RegisteredTypeMap
        {
            get { return TypeNameMap; }
        }

        public static FASTType GetType(string typeName)
        {
            FASTType value;
            if (!TypeNameMap.TryGetValue(typeName, out value))
            {
                throw new ArgumentException("The type named " + typeName + " does not exist.  Existing types are " +
                                            Util.CollectionToString(TypeNameMap.Keys));
            }
            return value;
        }

        public override string ToString()
        {
            return _name;
        }

        public virtual string Serialize(ScalarValue value)
        {
            return value.ToString();
        }

        public abstract TypeCodec GetCodec(Operator.Operator op, bool optional);
        public abstract ScalarValue GetValue(string value);
        public abstract bool IsValueOf(ScalarValue previousValue);

        public virtual void ValidateValue(ScalarValue value)
        {
        }

        public static FASTType[] ALL_TYPES()
        {
            return _staticAllTypes ??
                   (_staticAllTypes = new[]
                                         {
                                             U8, U16, U32, U64, I8, I16, I32, I64, STRING, ASCII,
                                             UNICODE, BYTE_VECTOR, DECIMAL
                                         });
        }

        #region Equals

        public virtual bool Equals(FASTType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;   // if derived class does not implement Equals, it should still work.
            return Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FASTType);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        #endregion
    }
}