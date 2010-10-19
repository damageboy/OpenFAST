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
    internal sealed class DeltaIntegerOperatorCodec : AlwaysPresentOperatorCodec
    {
        internal DeltaIntegerOperatorCodec(Operator op, FASTType[] types)
            : base(op, types)
        {
        }

        public override ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue, Scalar field)
        {
            if (priorValue == null)
            {
                Global.HandleError(FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT,
                                   "The field " + field + " must have a priorValue defined.");
                return null;
            }

            if (value == null)
            {
                if (field.IsOptional)
                {
                    return ScalarValue.Null;
                }
                throw new ArgumentException("Mandatory fields can't be null.");
            }

            if (priorValue.IsUndefined)
            {
                priorValue = field.BaseValue;
            }

            return ((NumericValue) value).Subtract((NumericValue) priorValue);
        }

        public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue, Scalar field)
        {
            if (previousValue == null)
            {
                Global.HandleError(FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT,
                                   "The field " + field + " must have a priorValue defined.");
                return null;
            }

            if ((newValue == null) || newValue.IsNull)
            {
                return null;
            }

            if (previousValue.IsUndefined)
            {
                previousValue = field.DefaultValue.IsUndefined ? field.BaseValue : field.DefaultValue;
            }

            return ((NumericValue) newValue).Add((NumericValue) previousValue);
        }

        public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
        {
            if (previousValue.IsUndefined)
            {
                if (field.DefaultValue.IsUndefined)
                {
                    if (field.IsOptional)
                    {
                        return ScalarValue.Undefined;
                    }
                    Global.HandleError(FastConstants.D5_NO_DEFAULT_VALUE, "");
                }
                else
                {
                    return field.DefaultValue;
                }
            }

            return previousValue;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == GetType(); //POINTP
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}