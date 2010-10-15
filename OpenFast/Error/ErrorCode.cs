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
using System.Collections.Generic;

namespace OpenFAST.Error
{
    public sealed class ErrorCode : IEquatable<ErrorCode>
    {
        private static readonly Dictionary<int, ErrorCode> AlertCodes = new Dictionary<int, ErrorCode>();

        private readonly int _code;
        private readonly string _description;
        private readonly FastAlertSeverity _severity;
        private readonly string _shortName;
        private readonly ErrorType _type;

        public ErrorCode(ErrorType type, int code, string shortName, string description, FastAlertSeverity severity)
        {
            AlertCodes[code] = this;
            _type = type;
            _code = code;
            _shortName = shortName;
            _description = description;
            _severity = severity;
        }

        public int Code
        {
            get { return _code; }
        }

        public string Description
        {
            get { return _description; }
        }

        public string ShortName
        {
            get { return _shortName; }
        }

        public FastAlertSeverity Severity
        {
            get { return _severity; }
        }

        public ErrorType Type
        {
            get { return _type; }
        }

        #region IEquatable<ErrorCode> Members

        public bool Equals(ErrorCode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._code == _code && Equals(other._description, _description) &&
                   Equals(other._severity, _severity) && Equals(other._shortName, _shortName) &&
                   Equals(other._type, _type);
        }

        #endregion

        public void ThrowException(string message)
        {
            throw new FastException(message, this);
        }

        public static ErrorCode GetAlertCode(Message alertMsg)
        {
            return AlertCodes[alertMsg.GetInt(2)];
        }

        public override string ToString()
        {
            return _shortName + ": " + _description;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ErrorCode)) return false;
            return Equals((ErrorCode) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _code;
                result = (result*397) ^ (_description != null ? _description.GetHashCode() : 0);
                result = (result*397) ^ (_severity != null ? _severity.GetHashCode() : 0);
                result = (result*397) ^ (_shortName != null ? _shortName.GetHashCode() : 0);
                result = (result*397) ^ (_type != null ? _type.GetHashCode() : 0);
                return result;
            }
        }
    }
}