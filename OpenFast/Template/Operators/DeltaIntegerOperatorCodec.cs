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
using OpenFAST.Template.Types;

namespace OpenFAST.Template.Operators
{
    [Serializable]
    internal sealed class DeltaIntegerOperatorCodec : AlwaysPresentOperatorCodec
    {
        internal DeltaIntegerOperatorCodec(Operator op, FastType[] types)
            : base(op, types)
        {
        }

        public override ScalarValue GetValueToEncode(ScalarValue value, ScalarValue priorValue, Scalar field)
        {
            if (priorValue == null)
            {
                Global.ErrorHandler.OnError(null, DynError.MandatoryFieldNotPresent,
                                            "The field {0} must have a priorValue defined.", field);
                return null;
            }

            if (value == null)
            {
                if (field.IsOptional)
                {
                    return ScalarValue.Null;
                }
                throw new ArgumentException("Mandatory fields can't be null.");
            }

            if (priorValue.IsUndefined)
            {
                priorValue = field.BaseValue;
            }

            return ((NumericValue) value).Subtract((NumericValue) priorValue);
        }

        public override ScalarValue DecodeValue(ScalarValue newValue, ScalarValue priorValue, Scalar field)
        {
            if (priorValue == null)
            {
                Global.ErrorHandler.OnError(null, DynError.MandatoryFieldNotPresent,
                                            "The field {0} must have a priorValue defined.", field);
                return null;
            }

            if ((newValue == null) || newValue.IsNull)
            {
                return null;
            }

            if (priorValue.IsUndefined)
            {
                priorValue = field.DefaultValue.IsUndefined ? field.BaseValue : field.DefaultValue;
            }

            return ((NumericValue) newValue).Add((NumericValue) priorValue);
        }

        public override ScalarValue DecodeEmptyValue(ScalarValue priorValue, Scalar field)
        {
            if (priorValue.IsUndefined)
            {
                if (field.DefaultValue.IsUndefined)
                {
                    if (field.IsOptional)
                    {
                        return ScalarValue.Undefined;
                    }
                    Global.ErrorHandler.OnError(null, DynError.NoDefaultValue, "");
                }
                else
                {
                    return field.DefaultValue;
                }
            }

            return priorValue;
        }

        #region Equals

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == GetType(); //POINTP
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}