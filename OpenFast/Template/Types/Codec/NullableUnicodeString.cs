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
using System.IO;
using System.Text;

namespace OpenFAST.Template.Types.Codec
{
    [Serializable]
    public sealed class NullableUnicodeString : NotStopBitEncodedTypeCodec
    {
        internal NullableUnicodeString()
        {
        }

        public static ScalarValue DefaultValue
        {
            get { return new StringValue(""); }
        }

        public override byte[] EncodeValue(ScalarValue value)
        {
            if (value.IsNull)
                return NullableByteVectorType.EncodeValue(ScalarValue.Null);

            byte[] utf8encoding = Encoding.UTF8.GetBytes(((StringValue) value).Value);
            return NullableByteVectorType.Encode(new ByteVectorValue(utf8encoding));
        }

        public override ScalarValue Decode(Stream inStream)
        {
            ScalarValue decodedValue = NullableByteVectorType.Decode(inStream);
            if (decodedValue == null)
                return null;
            var value = (ByteVectorValue) decodedValue;
            return new StringValue(Encoding.UTF8.GetString(value.Value));
        }

        public static ScalarValue FromString(string value)
        {
            return new StringValue(value);
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