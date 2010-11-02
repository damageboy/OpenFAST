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
using System.Collections.Generic;
using System.Text;
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types.Codec;
using OpenFAST.Utility;

namespace OpenFAST.Template.Types
{
    public abstract class FastType
    {
        private static readonly Dictionary<string, FastType> TypeNameMap = new Dictionary<string, FastType>();

        public static readonly FastType U8 = new UnsignedIntegerType(8, 256);
        public static readonly FastType U16 = new UnsignedIntegerType(16, 65536);
        public static readonly FastType U32 = new UnsignedIntegerType(32, 4294967295L);
        public static readonly FastType U64 = new UnsignedIntegerType(64, Int64.MaxValue);
        public static readonly FastType I8 = new SignedIntegerType(8, SByte.MinValue, SByte.MaxValue);
        public static readonly FastType I16 = new SignedIntegerType(16, Int16.MinValue, Int16.MaxValue);
        public static readonly FastType I32 = new SignedIntegerType(32, Int32.MinValue, Int32.MaxValue);
        public static readonly FastType I64 = new SignedIntegerType(64, Int64.MinValue, Int64.MaxValue);
        public static readonly FastType String = new TypeString("string", TypeCodec.Ascii, TypeCodec.NullableAscii);
        public static readonly FastType Ascii = new TypeString("ascii", TypeCodec.Ascii, TypeCodec.NullableAscii);

        public static readonly FastType Unicode = new UtfTypeString("unicode", TypeCodec.Unicode,
                                                                        TypeCodec.NullableUnicode);

        public static readonly FastType ByteVector = new ByteVectorType();
        public static readonly FastType Decimal = new DecimalType();

        public static readonly FastType[] IntegerTypes = new[] { U8, U16, U32, U64, I8, I16, I32, I64 };

        private static FastType[] _staticAllTypes;

        private readonly string _name;

        protected FastType(string typeName)
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

        public static Dictionary<string, FastType> RegisteredTypeMap
        {
            get { return TypeNameMap; }
        }

        public static FastType GetType(string typeName)
        {
            FastType value;
            if (TypeNameMap.TryGetValue(typeName, out value))
                return value;

            throw new ArgumentOutOfRangeException(
                "typename", typeName,
                "The type does not exist.  Existing types are " + Util.CollectionToString(TypeNameMap.Keys));
        }

        public override string ToString()
        {
            return _name;
        }

        [Obsolete("need?")] // BUG? Do we need this?
        public virtual string Serialize(ScalarValue value)
        {
            return value.ToString();
        }

        public abstract TypeCodec GetCodec(Operator op, bool optional);
        public abstract ScalarValue GetValue(string value);
        public abstract bool IsValueOf(ScalarValue priorValue);

        public virtual void ValidateValue(ScalarValue value)
        {
        }

        public static FastType[] AllTypes()
        {
            return _staticAllTypes ??
                   (_staticAllTypes = new[]
                                          {
                                              U8, U16, U32, U64, I8, I16, I32, I64, String, Ascii,
                                              Unicode, ByteVector, Decimal
                                          });
        }

        public virtual ScalarValue GetValue(byte[] bytes)
        {
            throw new UnsupportedOperationException();
        }

        #region Equals

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;

            var other = obj as FastType;
            if (ReferenceEquals(null, other)) return false;
            return Equals(other._name, _name);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        #endregion

        #region Nested type: TypeString

        private sealed class TypeString : StringType
        {
            public TypeString(string typeName, TypeCodec codec, TypeCodec nullableCodec)
                : base(typeName, codec, nullableCodec)
            {
            }

            public override ScalarValue GetValue(byte[] bytes)
            {
                return new StringValue(Encoding.ASCII.GetString(bytes));
            }
        }

        #endregion

        #region Nested type: UtfTypeString

        private sealed class UtfTypeString : StringType
        {
            public UtfTypeString(string typeName, TypeCodec codec, TypeCodec nullableCodec)
                : base(typeName, codec, nullableCodec)
            {
            }

            public override ScalarValue GetValue(byte[] bytes)
            {
                return new StringValue(Encoding.UTF8.GetString(bytes));
            }
        }

        #endregion
    }
}