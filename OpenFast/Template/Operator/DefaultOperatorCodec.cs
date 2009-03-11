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
using ScalarValue = OpenFAST.ScalarValue;
using Scalar = OpenFAST.Template.Scalar;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace openfast.Template.Operator
{
    [Serializable]
    sealed class DefaultOperatorCodec:OperatorCodec
    {
        internal DefaultOperatorCodec(Operator operator_Renamed, FASTType[] types):base(operator_Renamed, types)
        {
        }
		
        public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field)
        {
            if (value_Renamed == null)
            {
                return field.DefaultValue.Undefined ? null : ScalarValue.NULL;
            }

            return value_Renamed.Equals(field.DefaultValue)?null:value_Renamed;
        }
		
        public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue, Scalar field)
        {
            return newValue;
        }
		
        public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
        {
            return field.DefaultValue.Undefined ? null : field.DefaultValue;
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