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
using ErrorHandler = OpenFAST.Error.ErrorHandler;

namespace OpenFAST
{
	
	public sealed class Global
	{
		public static ErrorHandler ErrorHandler
		{
			set
			{
				if (value == null)
				{
					throw new System.NullReferenceException();
				}
				
				Global.errorHandler = value;
			}
			
		}
		private static ErrorHandler errorHandler = OpenFAST.Error.ErrorHandler_Fields.DEFAULT;
		private static int currentImplicitId = (int) ((System.DateTime.Now.Ticks - 621355968000000000) / 10000 % 10000);
		
		public static void  HandleError(ErrorCode error, string message)
		{
			errorHandler.Error(error, message);
		}
		
		public static void  HandleError(ErrorCode error, string message, System.Exception source)
		{
			errorHandler.Error(error, message, source);
		}
		
		public static QName CreateImplicitName(QName name)
		{
			return new QName(name + "@" + currentImplicitId++, name.Namespace);
		}
		
		private Global()
		{
		}
	}
}