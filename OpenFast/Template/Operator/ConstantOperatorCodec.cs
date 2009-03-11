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
using BitVectorBuilder = OpenFAST.BitVectorBuilder;
using FieldValue = OpenFAST.FieldValue;
using ScalarValue = OpenFAST.ScalarValue;
using Scalar = OpenFAST.Template.Scalar;
using FASTType = OpenFAST.Template.Type.FASTType;

namespace openfast.Template.Operator
{
    [Serializable]
    sealed class ConstantOperatorCodec:OperatorCodec
    {
        internal ConstantOperatorCodec(Operator operator_Renamed, FASTType[] types):base(operator_Renamed, types)
        {
        }

        public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field, BitVectorBuilder presenceMapBuilder)
        {
            if (field.Optional)
                presenceMapBuilder.OnValueSkipOnNull = value_Renamed;
            return null; // Never encode constant value.
        }
        public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue, Scalar field)
        {
            return field.DefaultValue;
        }
        public override bool IsPresenceMapBitSet(byte[] encoding, FieldValue fieldValue)
        {
            return fieldValue != null;
        }
        public override bool ShouldDecodeType()
        {
            return false;
        }

        public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
        {
            if (!field.Optional)
            {
                return field.DefaultValue;
            }
            return null;
        }

        public override bool UsesPresenceMapBit(bool optional)
        {
            return optional;
        }
        public override ScalarValue GetValueToEncode(ScalarValue value_Renamed, ScalarValue priorValue, Scalar field)
        {
            throw new NotSupportedException();
        }
        public override bool CanEncode(ScalarValue value_Renamed, Scalar field)
        {
            if (field.Optional && value_Renamed == null)
                return true;
            return field.DefaultValue.Equals(value_Renamed);
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