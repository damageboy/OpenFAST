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

namespace OpenFAST.Error
{
    public struct ErrorHandlerFields
    {
        public static readonly IErrorHandler Default = new DefaultErrorHandler();
        public static readonly IErrorHandler Null = new NullErrorHandler();
    }

    public class DefaultErrorHandler : IErrorHandler
    {
        #region IErrorHandler Members

        public virtual void Error(ErrorCode code, string message)
        {
            code.ThrowException(message);
        }

        public virtual void Error(ErrorCode code, string message, Exception t)
        {
            throw new FastException(message, code, t);
        }

        #endregion
    }

    public class NullErrorHandler : IErrorHandler
    {
        #region IErrorHandler Members

        public virtual void Error(ErrorCode code, string message)
        {
        }

        public virtual void Error(ErrorCode code, string message, Exception t)
        {
        }

        #endregion
    }

    public interface IErrorHandler
    {
        void Error(ErrorCode code, string message);
        void Error(ErrorCode code, string message, Exception t);
    }
}