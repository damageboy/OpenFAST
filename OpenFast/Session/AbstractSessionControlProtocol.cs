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
using OpenFAST.Template;

namespace OpenFAST.Session
{
    public abstract class AbstractSessionControlProtocol : ISessionProtocol
    {
        internal const int FAST_RESET_TEMPLATE_ID = 120;

        internal static readonly MessageTemplate FAST_RESET_TEMPLATE = new MessageTemplate("Reset", new Field[] {});

        internal static readonly Message RESET;

        static AbstractSessionControlProtocol()
        {
            RESET = new RESETMessageBase(FAST_RESET_TEMPLATE);
        }

        #region ISessionProtocol Members

        public virtual Message ResetMessage
        {
            get { return RESET; }
        }

        public abstract Message CloseMessage { get; }

        public abstract Message CreateTemplateDefinitionMessage(MessageTemplate param1);
        public abstract Session OnNewConnection(string param1, IConnection param2);

        public abstract Session Connect(string param1, IConnection param2, ITemplateRegistry inboundRegistry,
                                        ITemplateRegistry outboundRegistry, IMessageListener messageListener,
                                        ISessionListener sessionListener);

        public abstract void HandleMessage(Session param1, Message param2);
        public abstract void OnError(Session param1, ErrorCode param2, string param3);
        public abstract bool SupportsTemplateExchange();
        public abstract void ConfigureSession(Session param1);
        public abstract Message CreateTemplateDeclarationMessage(MessageTemplate param1, int param2);
        public abstract bool IsProtocolMessage(Message param1);

        #endregion

        #region Nested type: RESETMessageBase

        [Serializable]
        public class RESETMessageBase : Message
        {
            internal RESETMessageBase(MessageTemplate Param1)
                : base(Param1)
            {
            }
        }

        #endregion
    }
}