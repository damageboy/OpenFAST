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

namespace OpenFAST.Template.Type.Codec
{
    [Serializable]
    public sealed class StringDelta : TypeCodec
    {
        public static ScalarValue DefaultValue
        {
            get { return new StringValue(""); }
        }

        public override ScalarValue Decode(Stream inStream)
        {
            ScalarValue subtractionLength = Integer.Decode(inStream);
            ScalarValue difference = Ascii.Decode(inStream);

            return new TwinValue(subtractionLength, difference);
        }

        public override byte[] EncodeValue(ScalarValue value)
        {
            if (value == null || value == ScalarValue.Null)
                throw new ArgumentNullException("value", "Cannot have null values for non-nullable string delta");

            var diff = (TwinValue) value;
            byte[] subtractionLength = Integer.Encode(diff.First);
            byte[] difference = Ascii.Encode(diff.Second);
            var encoded = new byte[subtractionLength.Length + difference.Length];
            Array.Copy(subtractionLength, 0, encoded, 0, subtractionLength.Length);
            Array.Copy(difference, 0, encoded, subtractionLength.Length, difference.Length);

            return encoded;
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