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
using OpenFAST.Template.Type;

namespace OpenFAST.Template.Operator
{
    [Serializable]
    public sealed class DeltaDecimalOperatorCodec : AlwaysPresentOperatorCodec
    {
        internal DeltaDecimalOperatorCodec()
            : base(Operator.DELTA, new[] {FASTType.DECIMAL})
        {
        }

        public override ScalarValue GetValueToEncode(ScalarValue val, ScalarValue priorVal, Scalar field)
        {
            if (priorVal == null)
            {
                Global.HandleError(FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT,
                                   "The field " + field + " must have a priorValue defined.");
                return null;
            }

            if (val == null)
            {
                if (field.IsOptional)
                {
                    return ScalarValue.Null;
                }
                Global.HandleError(FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "");
                return null;
            }

            if (priorVal.IsUndefined && field.DefaultValue.IsUndefined)
            {
                return val;
            }

            DecimalValue priorValue = priorVal.IsUndefined ? (DecimalValue) field.DefaultValue : (DecimalValue) priorVal;

            var v = (DecimalValue) val;
            return new DecimalValue(v.Mantissa - priorValue.Mantissa,
                                    v.Exponent - priorValue.Exponent);
        }

        public override ScalarValue DecodeValue(ScalarValue val, ScalarValue priorVal, Scalar field)
        {
            if (priorVal == null)
            {
                Global.HandleError(FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT,
                                   "The field " + field + " must have a priorValue defined.");
                return null;
            }

            if (val == null)
                return null;

            var priorValue = (DecimalValue) (priorVal.IsUndefined
                                                 ? (field.DefaultValue.IsUndefined
                                                        ? field.BaseValue
                                                        : field.DefaultValue)
                                                 : priorVal);
            var v = (DecimalValue) val;

            return new DecimalValue(v.Mantissa + priorValue.Mantissa,
                                    v.Exponent + priorValue.Exponent);
        }

        public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
        {
            if (field.DefaultValue.IsUndefined)
            {
                if (field.IsOptional)
                    return ScalarValue.Null;

                if (previousValue.IsUndefined)
                    throw new ArgumentException(
                        "Mandatory fields without a previous value or default value must be present.",
                        "previousValue");

                return previousValue;
            }
            return field.DefaultValue;
        }

        public override bool Equals(Object obj)
        {
            return obj != null && obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}