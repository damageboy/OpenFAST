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
    public sealed class CopyOperatorCodec : OptionallyPresentOperatorCodec
    {
        internal CopyOperatorCodec() : base(Operator.COPY, FASTType.ALL_TYPES())
        {
        }

        protected internal override ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue,
                                                                 ScalarValue defaultValue)
        {
            if ((priorValue == ScalarValue.Undefined) && value.Equals(defaultValue))
            {
                return null;
            }

            return (value.Equals(priorValue)) ? null : value;
        }

        protected internal override ScalarValue GetInitialValue(Scalar field)
        {
            if (!field.DefaultValue.IsUndefined)
                return field.DefaultValue;

            if (field.IsOptional)
                return null;

            Global.HandleError(FastConstants.D5_NO_DEFAULT_VALUE, "No default value for " + field);

            return null;
        }

        protected internal override ScalarValue GetEmptyValue(ScalarValue priorValue)
        {
            return priorValue;
        }

        public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue priorValue, Scalar field)
        {
            return newValue;
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