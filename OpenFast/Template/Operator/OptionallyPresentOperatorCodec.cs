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
    public abstract class OptionallyPresentOperatorCodec : OperatorCodec
    {
        protected internal OptionallyPresentOperatorCodec(Operator op, FASTType[] types)
            : base(op, types)
        {
        }

        public override ScalarValue DecodeEmptyValue(ScalarValue priorValue, Scalar field)
        {
            return priorValue == ScalarValue.Undefined ? GetInitialValue(field) : GetEmptyValue(priorValue);
        }

        public override ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue, Scalar field)
        {
            if (value != null)
            {
                return GetValueToEncode(value, priorValue, field.DefaultValue);
            }

            if (field.IsOptional)
            {
                if (((priorValue == ScalarValue.Undefined) && !field.DefaultValue.IsUndefined) ||
                    ((priorValue != ScalarValue.Undefined) && (priorValue != null)))
                {
                    return ScalarValue.Null;
                }
            }
            else
            {
                Global.HandleError(FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT,
                                   "The field \"" + field + " is not present.");
            }

            return null;
        }

        protected internal abstract ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue,
                                                                 ScalarValue defaultValue);

        protected internal abstract ScalarValue GetInitialValue(Scalar field);

        protected internal abstract ScalarValue GetEmptyValue(ScalarValue priorValue);
    }
}