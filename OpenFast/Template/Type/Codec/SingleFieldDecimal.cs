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
using System.IO;
using OpenFAST.Error;

namespace OpenFAST.Template.Type.Codec
{
    [Serializable]
    internal sealed class SingleFieldDecimal : TypeCodec
    {
        public static ScalarValue DefaultValue
        {
            get { return new DecimalValue(0.0); }
        }

        public override byte[] EncodeValue(ScalarValue v)
        {
            if (v == ScalarValue.NULL)
            {
                return NULL_VALUE_ENCODING;
            }

            var buffer = new MemoryStream();
            var value = (DecimalValue) v;

            try
            {
                if (Math.Abs(value.Exponent) > 63)
                {
                    Global.HandleError(FastConstants.R1_LARGE_DECIMAL,
                                       "Encountered exponent of size " + value.Exponent);
                }

                byte[] tmp = INTEGER.Encode(new IntegerValue(value.Exponent));
                buffer.Write(tmp, 0, tmp.Length);

                tmp = INTEGER.Encode(new LongValue(value.Mantissa));
                buffer.Write(tmp, 0, tmp.Length);
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }

            return buffer.ToArray();
        }

        public override ScalarValue Decode(Stream inStream)
        {
            int exponent = ((IntegerValue) INTEGER.Decode(inStream)).Value;

            if (Math.Abs(exponent) > 63)
            {
                Global.HandleError(FastConstants.R1_LARGE_DECIMAL, "Encountered exponent of size " + exponent);
            }

            long mantissa = INTEGER.Decode(inStream).ToLong();
            var decimalValue = new DecimalValue(mantissa, exponent);

            return decimalValue;
        }

        public static ScalarValue FromString(string value)
        {
            return new DecimalValue(Double.Parse(value));
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