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
using OpenFAST.Template.Type;

namespace OpenFAST.Template.Operator
{
    [Serializable]
    internal sealed class ConstantOperatorCodec : OperatorCodec
    {
        internal ConstantOperatorCodec(Operator op, FASTType[] types)
            : base(op, types)
        {
        }

        public override bool ShouldDecodeType
        {
            get { return false; }
        }

        public override ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue, Scalar field,
                                                     BitVectorBuilder presenceMapBuilder)
        {
            if (field.IsOptional)
                presenceMapBuilder.OnValueSkipOnNull = value;
            return null; // Never encode constant value.
        }

        public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue previousValue, Scalar field)
        {
            return field.DefaultValue;
        }

        public override bool IsPresenceMapBitSet(byte[] encoding, IFieldValue fieldValue)
        {
            return fieldValue != null;
        }

        public override ScalarValue DecodeEmptyValue(ScalarValue previousValue, Scalar field)
        {
            if (!field.IsOptional)
            {
                return field.DefaultValue;
            }
            return null;
        }

        public override bool UsesPresenceMapBit(bool optional)
        {
            return optional;
        }

        public override ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue, Scalar field)
        {
            throw new NotSupportedException();
        }

        public override bool CanEncode(ScalarValue value, Scalar field)
        {
            if (field.IsOptional && value == null)
                return true;
            return field.DefaultValue.Equals(value);
        }

        public override bool Equals(Object obj)
        {
            return obj != null && obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}