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
using OpenFAST.Error;
using OpenFAST.Template.Type.Codec;
using OpenFAST.Utility;

namespace OpenFAST.Template.Type
{
    [Serializable]
    public abstract class IntegerType : SimpleType
    {
        protected internal long MaxValue;
        protected internal long MinValue;

        protected IntegerType(string typeName, long minValue, long maxValue, TypeCodec codec, TypeCodec nullableCodec)
            : base(typeName, codec, nullableCodec)
        {
            MinValue = minValue;
            MaxValue = maxValue;
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
                Global.HandleError(FastConstants.S3_INITIAL_VALUE_INCOMP,
                                   "The value \"" + value + "\" is not compatable with type " + this);
                return null;
            }

            if (Util.IsBiggerThanInt(longValue))
                return new LongValue(longValue);

            return new IntegerValue((int) longValue);
        }

        public override bool IsValueOf(ScalarValue previousValue)
        {
            return previousValue is IntegerValue || previousValue is LongValue;
        }

        public override void ValidateValue(ScalarValue value)
        {
            if (value == null || value.Undefined)
                return;

            if (value.ToLong() > MaxValue || value.ToLong() < MinValue)
            {
                Global.HandleError(FastConstants.D2_INT_OUT_OF_RANGE,
                                   "The value " + value + " is out of range for type " + this);
            }
        }
    }
}