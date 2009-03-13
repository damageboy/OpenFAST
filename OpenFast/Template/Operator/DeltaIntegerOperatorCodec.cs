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
    sealed class DeltaIntegerOperatorCodec:AlwaysPresentOperatorCodec
    {
        internal DeltaIntegerOperatorCodec(Operator operator_Renamed, FASTType[] types):base(operator_Renamed, types)
        {
        }
		
        public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field)
        {
            if (priorValue == null)
            {
                Global.HandleError(Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field " + field + " must have a priorValue defined.");
                return null;
            }
			
            if (value_Renamed == null)
            {
                if (field.Optional)
                {
                    return ScalarValue.NULL;
                }
                throw new ArgumentException("Mandatory fields can't be null.");
            }

            if (priorValue.Undefined)
            {
                priorValue = field.BaseValue;
            }
			
            return ((NumericValue) value_Renamed).Subtract((NumericValue) priorValue);
        }
		
        public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue, Scalar field)
        {
            if (previousValue == null)
            {
                Global.HandleError(Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field " + field + " must have a priorValue defined.");
                return null;
            }
			
            if ((newValue == null) || newValue.Null)
            {
                return null;
            }
			
            if (previousValue.Undefined)
            {
                previousValue = field.DefaultValue.Undefined ? field.BaseValue : field.DefaultValue;
            }
			
            return ((NumericValue) newValue).Add((NumericValue) previousValue);
        }
		
        public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
        {
            if (previousValue.Undefined)
            {
                if (field.DefaultValue.Undefined)
                {
                    if (field.Optional)
                    {
                        return ScalarValue.UNDEFINED;
                    }
                    Global.HandleError(Error.FastConstants.D5_NO_DEFAULT_VALUE, "");
                }
                else
                {
                    return field.DefaultValue;
                }
            }
			
            return previousValue;
        }
		
        public  override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == GetType();//POINTP
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}