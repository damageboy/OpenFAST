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
using OpenFAST.Error;
using OpenFAST.Template.Type.Codec;
using OpenFAST.Utility;

namespace OpenFAST.Template.Type
{
    [Serializable]
    public abstract class IntegerType : SimpleType
    {
        private readonly long _maxValue;
        private readonly long _minValue;

        protected IntegerType(string typeName, long minValue, long maxValue, TypeCodec codec, TypeCodec nullableCodec)
            : base(typeName, codec, nullableCodec)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public override ScalarValue DefaultValue
        {
            get { return new IntegerValue(0); }
        }

        public override ScalarValue GetVal(string value)
        {
            long longValue;
            if (!Int64.TryParse(value, out longValue))
            {
                Global.ErrorHandler.OnError(null, StaticError.S3InitialValueIncomp,
                                            "The value '{0}' is not compatable with type {1}", value, this);
                return null;
            }

            if (Util.IsBiggerThanInt(longValue))
                return new LongValue(longValue);

            return new IntegerValue((int) longValue);
        }

        public override bool IsValueOf(ScalarValue priorValue)
        {
            return priorValue is IntegerValue || priorValue is LongValue;
        }

        public override void ValidateValue(ScalarValue value)
        {
            if (value == null || value.IsUndefined)
                return;

            if (value.ToLong() > _maxValue || value.ToLong() < _minValue)
            {
                Global.ErrorHandler.OnError(null, DynError.D2IntOutOfRange, "The value {0} is out of range for type {1}",
                                            value, this);
            }
        }

        #region Equals

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;

            var other = obj as IntegerType;
            if (ReferenceEquals(null, other)) return false;

            return base.Equals(other) && other._maxValue == _maxValue && other._minValue == _minValue;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ _maxValue.GetHashCode();
                result = (result*397) ^ _minValue.GetHashCode();
                return result;
            }
        }

        #endregion
    }
}