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
using System.Text;
using OpenFAST.Error;
using OpenFAST.Template.Type;

namespace OpenFAST.Template.Operator
{
    [Serializable]
    internal sealed class TailOperatorCodec : OperatorCodec
    {
        internal TailOperatorCodec(Operator op, FASTType[] types)
            : base(op, types)
        {
        }

        public override ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue, Scalar field)
        {
            if (value == null)
            {
                if (priorValue == null)
                    return null;
                if (priorValue.IsUndefined && field.DefaultValue.IsUndefined)
                    return null;
                return ScalarValue.Null;
            }

            if (priorValue == null)
            {
                return value;
            }

            if (priorValue.IsUndefined)
            {
                priorValue = field.BaseValue;
            }

            int index = 0;

            byte[] val = value.Bytes;
            byte[] prior = priorValue.Bytes;

            if (val.Length > prior.Length)
                return value;
            if (val.Length < prior.Length)
            {
                Global.HandleError(FastConstants.D3CantEncodeValue,
                                   "The value " + val + " cannot be encoded by a tail operator with previous value " +
                                   priorValue);
            }

            while (index < val.Length && val[index] == prior[index])
                index++;
            if (val.Length == index)
                return null;

            return (ScalarValue) field.CreateValue(Encoding.UTF8.GetString(val, index, val.Length - index));
        }

        public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue priorValue, Scalar field)
        {
            if (priorValue == null && !field.IsOptional)
            {
                Global.HandleError(FastConstants.D6MndtryFieldNotPresent, "");
                return null;
            }

            var baseValue = (StringValue) (priorValue == null || priorValue.IsUndefined
                                               ? field.BaseValue
                                               : priorValue);

            if (newValue == null || newValue.IsNull)
            {
                if (!field.IsOptional)
                    throw new ArgumentException("");

                return null;
            }

            string delta = ((StringValue) newValue).Value;
            int length = Math.Max(baseValue.Value.Length - delta.Length, 0);
            string root = baseValue.Value.Substring(0, (length) - (0));

            return new StringValue(root + delta);
        }

        public override ScalarValue DecodeEmptyValue(ScalarValue priorValue, Scalar field)
        {
            ScalarValue value = priorValue;
            if (value != null && value.IsUndefined)
                value = (field.DefaultValue.IsUndefined) ? null : field.DefaultValue;
            if (value == null && !field.IsOptional)
            {
                Global.HandleError(FastConstants.D6MndtryFieldNotPresent,
                                   "The field " + field + " was not present.");
            }
            return value;
        }

        #region Equals

        public override bool Equals(object obj) //POINTP
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