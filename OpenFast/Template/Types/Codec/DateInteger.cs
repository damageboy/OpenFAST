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
using OpenFAST.Utility;

namespace OpenFAST.Template.Types.Codec
{
    public sealed class DateInteger : TypeCodec
    {
        public override ScalarValue Decode(Stream inStream)
        {
            long longValue = Uint.Decode(inStream).ToLong();
            var year = (int) (longValue/10000);
            var month = (int) ((longValue - (year*10000))/100);
            var day = (int) (longValue%100);
            return new DateValue(new DateTime(year, month, day));
        }

        public override byte[] EncodeValue(ScalarValue value)
        {
            DateTime date = ((DateValue) value).Value;
            int intValue = Util.DateToInt(date);
            return Uint.Encode(new IntegerValue(intValue));
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