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
using OpenFAST.Template.Type;

namespace OpenFAST.Template.Operator
{
    [Serializable]
    internal sealed class IncrementIntegerOperatorCodec : OperatorCodec
    {
        internal IncrementIntegerOperatorCodec(Operator op, FASTType[] types)
            : base(op, types)
        {
        }

        public override bool DecodeNewValueNeedsPrevious
        {
            get { return false; }
        }

        public override ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue, Scalar field)
        {
            if (priorValue == null)
                return value;

            if (value == null)
            {
                if (!field.IsOptional)
                    throw new ArgumentNullException("value");

                if (priorValue == ScalarValue.Undefined && field.DefaultValue.IsUndefined)
                    return null;

                return ScalarValue.Null;
            }

            if (priorValue.IsUndefined)
                return value.Equals(field.DefaultValue) ? null : value;

            var inc = ((NumericValue) priorValue).Increment();
            return !value.Equals(inc) ? value : null;
        }

        public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue priorValue, Scalar field)
        {
            return newValue;
        }

        public override ScalarValue DecodeEmptyValue(ScalarValue priorValue, Scalar field)
        {
            if (priorValue == null)
                return null;

            if (priorValue.IsUndefined)
            {
                if (!field.DefaultValue.IsUndefined)
                    return field.DefaultValue;

                if (!field.IsOptional)
                    throw new InvalidOperationException(
                        "Field with operator increment must send a value if no previous value existed.");

                return null;
            }

            return ((NumericValue) priorValue).Increment();
        }

        #region Equals

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == GetType(); //POINTP
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}