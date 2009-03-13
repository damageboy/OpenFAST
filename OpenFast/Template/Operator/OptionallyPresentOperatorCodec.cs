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
using FASTType = OpenFAST.Template.Type.FASTType;

namespace OpenFAST.Template.Operator
{
    [Serializable]
    public abstract class OptionallyPresentOperatorCodec:OperatorCodec
    {
        protected internal OptionallyPresentOperatorCodec(Operator operator_Renamed, FASTType[] types):base(operator_Renamed, types)
        {
        }
		
        public override ScalarValue DecodeEmptyValue(ScalarValue priorValue, Scalar field)
        {
            return priorValue == ScalarValue.UNDEFINED ? GetInitialValue(field) : GetEmptyValue(priorValue);
        }

        public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field)
        {
            if (value_Renamed != null)
            {
                return GetValueToEncode(value_Renamed, priorValue, field.DefaultValue);
            }
			
            if (field.Optional)
            {
                if (((priorValue == ScalarValue.UNDEFINED) && !field.DefaultValue.Undefined) || ((priorValue != ScalarValue.UNDEFINED) && (priorValue != null)))
                {
                    return ScalarValue.NULL;
                }
            }
            else
            {
                Global.HandleError(Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field \"" + field + " is not present.");
            }
			
            return null;
        }
		
        protected internal abstract ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, ScalarValue defaultValue);
		
        protected internal abstract ScalarValue GetInitialValue(Scalar field);
		
        protected internal abstract ScalarValue GetEmptyValue(ScalarValue priorValue);
    }
}