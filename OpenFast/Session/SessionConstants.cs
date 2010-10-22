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
using OpenFAST.Error;

namespace OpenFAST.Session
{
    public struct SessionConstants
    {
        public const string VendorId = "http://OpenFAST.org/OpenFAST/1.1";

        // Session Control Protocol (SCP) Errors
        public static readonly ErrorCode TemplateNotSupported;
        public static readonly ErrorCode TemplateUnknown;
        public static readonly ErrorCode Unauthorized;
        public static readonly ErrorCode ProtcolError;
        public static readonly ErrorCode Close;
        public static readonly ErrorCode Undefined;

        public static readonly ISessionProtocol Scp10;
        public static readonly ISessionProtocol Scp11;

        static SessionConstants()
        {
            TemplateNotSupported = new ErrorCode(
                ErrorType.Session, 11, Severity.Error, "TNOTSUPP", "Template not supported");
            TemplateUnknown = new ErrorCode(
                ErrorType.Session, 12, Severity.Error, "TUNKNOWN", "Template unknown");
            Unauthorized = new ErrorCode(
                ErrorType.Session, 13, Severity.Fatal, "EAUTH", "Unauthorized");
            ProtcolError = new ErrorCode(
                ErrorType.Session, 14, Severity.Error, "EPROTO", "Protocol Error");
            Close = new ErrorCode(
                ErrorType.Session, 15, Severity.Info, "CLOSE", "Session Closed");
            Undefined = new ErrorCode(
                ErrorType.Session, -1, Severity.Error, "UNDEFINED", "Undefined Alert Code");

            Scp10 = new SessionControlProtocol10();
            Scp11 = new SessionControlProtocol11();
        }
    }
}