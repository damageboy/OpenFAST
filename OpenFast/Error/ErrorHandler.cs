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
using JetBrains.Annotations;

namespace OpenFAST.Error
{
    public static class ErrorHandlerFields
    {
        public static readonly IErrorHandler Default = new DefaultErrorHandler();
        public static readonly IErrorHandler Null = new NullErrorHandler();
    }

    public class DefaultErrorHandler : IErrorHandler
    {
        #region IErrorHandler Members

        public void OnError(Exception exception, StaticError error, string format, params object[] args)
        {
            throw new StatErrorException(exception, error, format, args);
        }

        public void OnError(Exception exception, DynError error, string format, params object[] args)
        {
            throw new DynErrorException(exception, error, format, args);
        }

        public void OnError(Exception exception, RepError error, string format, params object[] args)
        {
            throw new RepErrorException(exception, error, format, args);
        }

        #endregion
    }

    public sealed class NullErrorHandler : IErrorHandler
    {
        #region IErrorHandler Members

        public void OnError(Exception exception, StaticError error, string format, params object[] args)
        {
        }

        public void OnError(Exception exception, DynError error, string format, params object[] args)
        {
        }

        public void OnError(Exception exception, RepError error, string format, params object[] args)
        {
        }

        #endregion
    }

    public interface IErrorHandler
    {
        [StringFormatMethod("format")]
        void OnError(Exception exception, StaticError error, string format, params object[] args);

        [StringFormatMethod("format")]
        void OnError(Exception exception, DynError error, string format, params object[] args);

        [StringFormatMethod("format")]
        void OnError(Exception exception, RepError error, string format, params object[] args);
    }
}