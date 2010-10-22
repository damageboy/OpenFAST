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
using System.Globalization;
using System.IO;
using OpenFAST.Error;

namespace OpenFAST.Template.Type.Codec
{
    [Serializable]
    public sealed class DateString : TypeCodec
    {
        private readonly string _format;
        private readonly DateTimeFormatInfo _formatter;

        public DateString(string format)
        {
            _format = format;
            _formatter = new DateTimeFormatInfo();
        }

        public override ScalarValue Decode(Stream inStream)
        {
            string str = Ascii.Decode(inStream).ToString();

            DateTime result;
            if (DateTime.TryParseExact(str, _format, _formatter, DateTimeStyles.None, out result))
                return new DateValue(result);

            Global.HandleError(FastConstants.ParseError,
                               string.Format("'{0}' could not be parsed as DateTime with '{1}' format", str, _format));
            return null;
        }

        public override byte[] EncodeValue(ScalarValue value)
        {
#warning BUG? This used to format all values using "d-MMM-yy h:mm:ss tt" format, and now it uses constructor parameter instead
            return
                Ascii.Encode(
                    new StringValue(
                        (((DateValue) value).Value).ToString(_format, _formatter)));
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