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
using OpenFAST.Error;
using OpenFAST.Template.Types;

namespace OpenFAST.Template.Operators
{
    public abstract class OptionallyPresentOperatorCodec : OperatorCodec
    {
        protected internal OptionallyPresentOperatorCodec(Operator op, IEnumerable<FastType> types)
            : base(op, types)
        {
        }

        public override ScalarValue DecodeEmptyValue(ScalarValue priorValue, Scalar field)
        {
            return ScalarValue.Undefined.Equals(priorValue) ? GetInitialValue(field) : GetEmptyValue(priorValue);
        }

        public override ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue, Scalar field)
        {
            if (value != null)
            {
                return GetValueToEncode(value, priorValue, field.DefaultValue);
            }

            if (field.IsOptional)
            {
                bool e = ScalarValue.Undefined.Equals(priorValue);
                if ((e && !field.DefaultValue.IsUndefined) || (!e && priorValue != null))
                {
                    return ScalarValue.Null;
                }
            }
            else
            {
                Global.ErrorHandler.OnError(null, DynError.MandatoryFieldNotPresent, "The field '{0} is not present.",
                                            field);
            }

            return null;
        }

        protected abstract ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue,
                                                                 ScalarValue defaultValue);

        protected abstract ScalarValue GetInitialValue(Scalar field);

        protected abstract ScalarValue GetEmptyValue(ScalarValue priorValue);
    }
}