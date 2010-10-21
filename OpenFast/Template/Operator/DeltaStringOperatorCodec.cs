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
using OpenFAST.Utility;

namespace OpenFAST.Template.Operator
{
    [Serializable]
    internal sealed class DeltaStringOperatorCodec : AlwaysPresentOperatorCodec
    {
        internal DeltaStringOperatorCodec()
            : base(Operator.DELTA, new[] { FASTType.ASCII, FASTType.STRING, FASTType.UNICODE, FASTType.BYTE_VECTOR })
        {
        }

        public override bool DecodeEmptyValueNeedsPrevious
        {
            get { return false; }
        }

        public override ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue, Scalar field)
        {
            if (value == null)
                return ScalarValue.Null;

            if (priorValue == null)
            {
                Global.HandleError(FastConstants.D6_MNDTRY_FIELD_NOT_PRESENT,
                                   "The field " + field + " must have a priorValue defined.");
                return null;
            }

            ScalarValue v = (priorValue.IsUndefined) ? field.BaseValue : priorValue;
            return Util.GetDifference(value.Bytes, v.Bytes);
            //return Util.GetDifference((StringValue) value, (StringValue) v);
        }

        public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue priorValue, Scalar field)
        {
            if (newValue == null || newValue.IsNull)
                return null;

            var diffValue = (TwinValue) newValue;
            ScalarValue v = (priorValue.IsUndefined) ? field.BaseValue : priorValue;
            if (diffValue.First.ToInt() > v.ToString().Length)
            {
                Global.HandleError(FastConstants.D7_SUBTRCTN_LEN_LONG,
                                   "The string diff <" + diffValue + "> cannot be applied to the base value '" +
                                   v + "' because the subtraction length is too long.");
            }
            byte[] bytes = Util.ApplyDifference(v, diffValue);
            return field.Type.GetValue(bytes);
            //return Util.ApplyDifference((StringValue) v, diffValue);
        }

        public override ScalarValue DecodeEmptyValue(ScalarValue priorValue, Scalar field)
        {
            throw new InvalidOperationException("As of FAST v1.1 Delta values must be present in stream");
        }

        #region Equals

        public override bool Equals(Object obj)
        {
            return obj != null && obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}