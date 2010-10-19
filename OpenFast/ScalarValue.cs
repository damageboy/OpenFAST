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

namespace OpenFAST
{
    [Serializable]
    public class ScalarValue : IFieldValue
    {
        public static readonly ScalarValue Undefined;
        public static readonly ScalarValue Null;

        static ScalarValue()
        {
            Undefined = new UndefinedScalarValue();
            Null = new NullScalarValue();
        }

        public virtual bool IsUndefined
        {
            get { return false; }
        }

        public virtual bool IsNull
        {
            get { return false; }
        }

        public virtual byte[] Bytes
        {
            get { throw new NotSupportedException(); }
        }

        #region IFieldValue Members

        public virtual IFieldValue Copy()
        {
            return this; // immutable objects don't need actual copies.
        }

        #endregion

        public virtual bool EqualsValue(string defaultValue)
        {
            return false;
        }

        public virtual byte ToByte()
        {
            throw new NotSupportedException();
        }

        public virtual short ToShort()
        {
            throw new NotSupportedException();
        }

        public virtual int ToInt()
        {
            throw new NotSupportedException();
        }

        public virtual long ToLong()
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            throw new NotSupportedException();
        }

        public virtual double ToDouble()
        {
            throw new NotSupportedException();
        }

        public virtual Decimal ToBigDecimal()
        {
            throw new NotSupportedException();
        }

        #region Nested type: NullScalarValue

        [Serializable]
        public sealed class NullScalarValue : ScalarValue, IEquatable<NullScalarValue>
        {
            public override bool IsNull
            {
                get { return true; }
            }

            public override string ToString()
            {
                return "NULL";
            }

            #region Equals

            public bool Equals(NullScalarValue other)
            {
                if (ReferenceEquals(null, other)) return false;
                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != typeof (NullScalarValue)) return false;
                return true;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            #endregion
        }

        #endregion

        #region Nested type: UndefinedScalarValue

        [Serializable]
        public sealed class UndefinedScalarValue : ScalarValue, IEquatable<UndefinedScalarValue>
        {
            public override bool IsUndefined
            {
                get { return true; }
            }

            public override string ToString()
            {
                return "UNDEFINED";
            }

            #region Equals

            public bool Equals(UndefinedScalarValue other)
            {
                if (ReferenceEquals(null, other)) return false;
                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != typeof (UndefinedScalarValue)) return false;
                return true;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            #endregion
        }

        #endregion
    }
}