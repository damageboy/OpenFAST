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
using OpenFAST.Error;

namespace OpenFAST
{
    public static class Global
    {
        private static IErrorHandler _errorHandler = ErrorHandlerFields.Default;
        private static int _currentImplicitId = (int) ((DateTime.Now.Ticks - 621355968000000000)/10000%10000);

        public static IErrorHandler ErrorHandler
        {
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _errorHandler = value;
            }
        }

        public static void HandleError(ErrorCode error, string message)
        {
            _errorHandler.Error(error, message);
        }

        public static void HandleError(ErrorCode error, string message, Exception source)
        {
            _errorHandler.Error(error, message, source);
        }

        public static QName CreateImplicitName(QName name)
        {
            return new QName(name + "@" + _currentImplicitId++, name.Namespace);
        }
    }
}