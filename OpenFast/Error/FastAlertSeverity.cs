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

namespace OpenFAST.Error
{
	public sealed class FastAlertSeverity
	{
		public int Code
		{
			get
			{
				return code;
			}
			
		}
		public string Description
		{
			get
			{
				return description;
			}
			
		}
		public string ShortName
		{
			get
			{
				return shortName;
			}
			
		}
		public static readonly FastAlertSeverity FATAL = new FastAlertSeverity(1, "FATAL", "Fatal");
		public static readonly FastAlertSeverity ERROR = new FastAlertSeverity(2, "ERROR", "Error");
		public static readonly FastAlertSeverity WARN = new FastAlertSeverity(3, "WARN", "Warning");
		public static readonly FastAlertSeverity INFO = new FastAlertSeverity(4, "INFO", "Information");
		private int code;
		private string shortName;
		private string description;
		
		public FastAlertSeverity(int code, string shortName, string description)
		{
			this.code = code;
			this.shortName = shortName;
			this.description = description;
		}
	}
}