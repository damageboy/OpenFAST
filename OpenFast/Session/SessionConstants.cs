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
using OpenFAST.Error;

namespace OpenFAST.Session
{
    public struct SessionConstants
    {
        public static readonly ErrorType SESSION;

        // Session Control Protocol (SCP) Errors
        public static readonly ErrorCode TEMPLATE_NOT_SUPPORTED;
        public static readonly ErrorCode TEMPLATE_UNKNOWN;
        public static readonly ErrorCode UNAUTHORIZED;
        public static readonly ErrorCode PROTCOL_ERROR;
        public static readonly ErrorCode CLOSE;
        public static readonly ErrorCode UNDEFINED;
        public static readonly SessionProtocol SCP_1_0;
        public static readonly SessionProtocol SCP_1_1;
        public static readonly string VENDOR_ID = "http://OpenFAST.org/OpenFAST/1.1";

        static SessionConstants()
        {
            SESSION = new ErrorType("Session");
            TEMPLATE_NOT_SUPPORTED = new ErrorCode(SESSION, 11, "TNOTSUPP", "Template not supported",
                                                   FastAlertSeverity.ERROR);
            TEMPLATE_UNKNOWN = new ErrorCode(SESSION, 12, "TUNKNOWN", "Template unknown", FastAlertSeverity.ERROR);
            UNAUTHORIZED = new ErrorCode(SESSION, 13, "EAUTH", "Unauthorized", FastAlertSeverity.FATAL);
            PROTCOL_ERROR = new ErrorCode(SESSION, 14, "EPROTO", "Protocol Error", FastAlertSeverity.ERROR);
            CLOSE = new ErrorCode(SESSION, 15, "CLOSE", "Session Closed", FastAlertSeverity.INFO);
            UNDEFINED = new ErrorCode(SESSION, - 1, "UNDEFINED", "Undefined Alert Code", FastAlertSeverity.ERROR);
            SCP_1_0 = new SessionControlProtocol_1_0();
            SCP_1_1 = new SessionControlProtocol_1_1();
        }
    }
}