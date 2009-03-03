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
using ErrorCode = OpenFAST.Error.ErrorCode;
using ErrorType = OpenFAST.Error.ErrorType;
using FastAlertSeverity = OpenFAST.Error.FastAlertSeverity;

namespace OpenFAST.Session
{
	public struct SessionConstants
    {
		public readonly static ErrorType SESSION;

		// Session Control Protocol (SCP) Errors
		public readonly static ErrorCode TEMPLATE_NOT_SUPPORTED;
		public readonly static ErrorCode TEMPLATE_UNKNOWN;
		public readonly static ErrorCode UNAUTHORIZED;
		public readonly static ErrorCode PROTCOL_ERROR;
		public readonly static ErrorCode CLOSE;
		public readonly static ErrorCode UNDEFINED;
		public readonly static SessionProtocol SCP_1_0;
		public readonly static SessionProtocol SCP_1_1;
		public readonly static string VENDOR_ID = "http://OpenFAST.org/OpenFAST/1.1";

		static SessionConstants()
		{
			SESSION = new ErrorType("Session");
			TEMPLATE_NOT_SUPPORTED = new ErrorCode(OpenFAST.Session.SessionConstants.SESSION, 11, "TNOTSUPP", "Template not supported", FastAlertSeverity.ERROR);
			TEMPLATE_UNKNOWN = new ErrorCode(OpenFAST.Session.SessionConstants.SESSION, 12, "TUNKNOWN", "Template unknown", FastAlertSeverity.ERROR);
			UNAUTHORIZED = new ErrorCode(OpenFAST.Session.SessionConstants.SESSION, 13, "EAUTH", "Unauthorized", FastAlertSeverity.FATAL);
			PROTCOL_ERROR = new ErrorCode(OpenFAST.Session.SessionConstants.SESSION, 14, "EPROTO", "Protocol Error", FastAlertSeverity.ERROR);
			CLOSE = new ErrorCode(OpenFAST.Session.SessionConstants.SESSION, 15, "CLOSE", "Session Closed", FastAlertSeverity.INFO);
			UNDEFINED = new ErrorCode(OpenFAST.Session.SessionConstants.SESSION, - 1, "UNDEFINED", "Undefined Alert Code", FastAlertSeverity.ERROR);
			SCP_1_0 = new SessionControlProtocol_1_0();
			SCP_1_1 = new SessionControlProtocol_1_1();
		}
	}
}