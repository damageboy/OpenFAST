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
    sealed class TailOperatorCodec:OperatorCodec
    {
        internal TailOperatorCodec(Operator operator_Renamed, FASTType[] types):base(operator_Renamed, types)
        {
        }
		
        public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field)
        {
            if (value_Renamed == null)
            {
                if (priorValue == null)
                    return null;
                if (priorValue.Undefined && field.DefaultValue.Undefined)
                    return null;
                return ScalarValue.NULL;
            }
			
            if (priorValue == null)
            {
                return value_Renamed;
            }
			
            if (priorValue.Undefined)
            {
                priorValue = field.BaseValue;
            }
			
            var index = 0;
			
            var val = value_Renamed.Bytes;
            var prior = priorValue.Bytes;
			
            if (val.Length > prior.Length)
                return value_Renamed;
            if (val.Length < prior.Length)
            {
                Global.HandleError(Error.FastConstants.D3_CANT_ENCODE_VALUE, "The value " + val + " cannot be encoded by a tail operator with previous value " + priorValue);
            }
			
            while (index < val.Length && val[index] == prior[index])
                index++;
            if (val.Length == index)
                return null;
            
            return (ScalarValue) field.CreateValue(System.Text.Encoding.UTF8.GetString(val, index, val.Length - index));
        }
		
        public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue, Scalar field)
        {
            StringValue base_Renamed;
			
            if ((previousValue == null) && !field.Optional)
            {
                Global.HandleError(Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "");
                return null;
            }
            if ((previousValue == null) || previousValue.Undefined)
            {
                base_Renamed = (StringValue) field.BaseValue;
            }
            else
            {
                base_Renamed = (StringValue) previousValue;
            }

            if (newValue == null || newValue.Null)
            {
                if (field.Optional)
                {
                    return null;
                }
                throw new ArgumentException("");
            }

            var delta = ((StringValue) newValue).value_Renamed;
            var length = Math.Max(base_Renamed.value_Renamed.Length - delta.Length, 0);
            var root = base_Renamed.value_Renamed.Substring(0, (length) - (0));
			
            return new StringValue(root + delta);
        }
		
        public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
        {
            var value_Renamed = previousValue;
            if (value_Renamed != null && value_Renamed.Undefined)
                value_Renamed = (field.DefaultValue.Undefined)?null:field.DefaultValue;
            if (value_Renamed == null && !field.Optional)
            {
                Global.HandleError(Error.FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT, "The field " + field + " was not present.");
            }
            return value_Renamed;
        }

        public override bool Equals(object obj)//POINTP
        {
            return obj != null && obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}