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
    public sealed class DeltaDecimalOperatorCodec:AlwaysPresentOperatorCodec
    {
        internal DeltaDecimalOperatorCodec():base(Operator.DELTA, new[]{FASTType.DECIMAL})
        {
        }
		
        public override ScalarValue GetValueToEncode(ScalarValue val, ScalarValue priorVal, Scalar field)
        {
            if (priorVal == null)
            {
                Global.HandleError(Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field " + field + " must have a priorValue defined.");
                return null;
            }
			
            if (val == null)
            {
                if (field.Optional)
                {
                    return ScalarValue.NULL;
                }
                Global.HandleError(Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "");
                return null;
            }
			
            if (priorVal.Undefined && field.DefaultValue.Undefined)
            {
                return val;
            }
			
            var priorValue = priorVal.Undefined?(DecimalValue) field.DefaultValue:(DecimalValue) priorVal;
            var value_Renamed = (DecimalValue) val;
			
            return new DecimalValue(value_Renamed.mantissa - priorValue.mantissa, value_Renamed.exponent - priorValue.exponent);
        }
		
        public override ScalarValue DecodeValue(ScalarValue val, ScalarValue priorVal, Scalar field)
        {
            if (priorVal == null)
            {
                Global.HandleError(Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field " + field + " must have a priorValue defined.");
                return null;
            }
			
            if (val == null)
            {
                return null;
            }
			
            DecimalValue priorValue;
			
            if (priorVal.Undefined)
            {
                if (field.DefaultValue.Undefined)
                {
                    priorValue = (DecimalValue) field.BaseValue;
                }
                else
                {
                    priorValue = (DecimalValue) field.DefaultValue;
                }
            }
            else
            {
                priorValue = (DecimalValue) priorVal;
            }
			
            var value_Renamed = (DecimalValue) val;
			
            return new DecimalValue(value_Renamed.mantissa + priorValue.mantissa, value_Renamed.exponent + priorValue.exponent);
        }
		
        public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
        {
            if (field.DefaultValue.Undefined)
            {
                if (field.Optional)
                {
                    return ScalarValue.NULL;
                }
                if (previousValue.Undefined)
                {
                    throw new SystemException("Mandatory fields without a previous value or default value must be present.");
                }
                return previousValue;
            }
            return field.DefaultValue;
        }

        public  override bool Equals(Object obj)
        {
            return obj != null && obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}