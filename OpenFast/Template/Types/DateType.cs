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
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types.Codec;

namespace OpenFAST.Template.Types
{
    public sealed class DateType : FastType, IEquatable<DateType>
    {
        private readonly TypeCodec _dateCodec;
        private readonly string _format;
        private readonly DateTimeFormatInfo _formatProvider;

        public DateType(string format, DateTimeFormatInfo formatProvider, TypeCodec dateCodec) : base("date")
        {
            if (format == null) throw new ArgumentNullException("format");
            _format = format;
            _formatProvider = formatProvider;
            _dateCodec = dateCodec;
        }

        public override ScalarValue DefaultValue
        {
            get
            {
                var tempAux = new DateTime(0);
                return new DateValue(tempAux);
            }
        }

        public override bool IsValueOf(ScalarValue priorValue)
        {
            return priorValue is DateValue;
        }

        public override TypeCodec GetCodec(Operator op, bool optional)
        {
            return _dateCodec;
        }

        public override ScalarValue GetValue(string value)
        {
            if (value == null)
                return ScalarValue.Undefined;

            try
            {
                DateTime tempAux = DateTime.Parse(value, _formatProvider);
                return new DateValue(tempAux);
            }
            catch (FormatException e)
            {
                throw new RuntimeException(e);
            }
        }

        [Obsolete("need?")] // BUG? Do we need this?
        public override string Serialize(ScalarValue value)
        {
            return (((DateValue) value).Value).ToString(_format, _formatProvider);
        }

        #region Equals

        public bool Equals(DateType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other._dateCodec, _dateCodec) && other._format.Equals(_format) &&
                   Equals(other._formatProvider, _formatProvider);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as DateType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ (_dateCodec != null ? _dateCodec.GetHashCode() : 0);
                result = (result*397) ^ _format.GetHashCode();
                result = (result*397) ^ (_formatProvider != null ? _formatProvider.GetHashCode() : 0);
                return result;
            }
        }

        #endregion
    }
}