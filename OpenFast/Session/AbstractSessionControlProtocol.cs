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
        internal static readonly MessageTemplate FastResetTemplate = new MessageTemplate("Reset", new Field[0]);
        private static readonly Message Reset = new ResetMessageObj(FastResetTemplate);

        #region ISessionProtocol Members

        public virtual Message ResetMessage
        {
            get { return Reset; }
        }

        public abstract Message CloseMessage { get; }

        public abstract Message CreateTemplateDefinitionMessage(MessageTemplate param1);
        public abstract Session OnNewConnection(string param1, IConnection param2);

        public abstract Session Connect(string param1, IConnection param2, ITemplateRegistry inboundRegistry,
                                        ITemplateRegistry outboundRegistry, IMessageListener messageListener,
                                        ISessionListener sessionListener);

        public abstract void HandleMessage(Session param1, Message param2);
        public abstract void OnError(Session param1, DynError param2, string param3);
        public abstract bool SupportsTemplateExchange { get; }
        public abstract void ConfigureSession(Session param1);
        public abstract Message CreateTemplateDeclarationMessage(MessageTemplate param1, int param2);
        public abstract bool IsProtocolMessage(Message param1);

        #endregion

        #region Nested type: ResetMessageObj

        [Serializable]
        private class ResetMessageObj : Message
        {
            internal ResetMessageObj(MessageTemplate template)
                : base(template)
            {
            }
        }

        #endregion
    }
}