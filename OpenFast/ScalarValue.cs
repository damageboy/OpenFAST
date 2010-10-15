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

namespace OpenFAST
{
    [Serializable]
    public class ScalarValue : IFieldValue
    {
        public static readonly ScalarValue UNDEFINED;
        public static readonly ScalarValue NULL;

        static ScalarValue()
        {
            UNDEFINED = new UndefinedScalarValue();
            NULL = new NullScalarValue();
        }

        public virtual bool Undefined
        {
            get { return false; }
        }

        public virtual bool Null
        {
            get { return false; }
        }

        public virtual byte[] Bytes
        {
            get { throw new NotSupportedException(); }
        }

        #region FieldValue Members

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
        public sealed class NullScalarValue : ScalarValue
        {
            public override bool Null
            {
                get { return true; }
            }

            public override string ToString()
            {
                return "NULL";
            }
        }

        #endregion

        #region Nested type: UndefinedScalarValue

        [Serializable]
        public sealed class UndefinedScalarValue : ScalarValue
        {
            public override bool Undefined
            {
                get { return true; }
            }

            public override string ToString()
            {
                return "UNDEFINED";
            }
        }

        #endregion
    }
}