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
using JetBrains.Annotations;
using OpenFAST.Error;
using OpenFAST.Template.Type.Codec;

namespace OpenFAST.Template.Type
{
    [Serializable]
    internal sealed class DecimalType : SimpleType
    {
        internal DecimalType()
            : base("decimal", TypeCodec.SfScaledNumber, TypeCodec.NullableSfScaledNumber)
        {
        }

        public override ScalarValue DefaultValue
        {
            get { return new DecimalValue(0.0); }
        }

        public override ScalarValue GetVal(string value)
        {
            double dbl;
            if (Double.TryParse(value, out dbl))
                return new DecimalValue(dbl);

            Global.ErrorHandler.OnError(null, StaticError.InitialValueIncomp,
                                        "The value '{0}' is not compatible with type {1}", value, this);
            return null;
        }

        public override bool IsValueOf(ScalarValue priorValue)
        {
            return priorValue is DecimalValue;
        }
    }
}
