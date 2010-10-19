using System;
using OpenFAST;
using OpenFAST.Error;
using OpenFAST.Session;
using OpenFAST.Session.Tcp;

namespace TCPClient
{
    public class FASTClient
    {
        private readonly FastClient _fc;
        private readonly Random _rnd = new Random();
        private Session _ses;

        public FASTClient(string host, int port)
        {
            _fc = new FastClient("client", SessionConstants.SCP_1_1, new TcpEndpoint(host, port));
        }

        public void Connect()
        {
            _ses = _fc.Connect();
            _ses.ErrorHandler = new ClientErrorHandler();
            _ses.MessageHandler = new FASTClientMessageHandler();
        }

        public void SendMessage(string symbol)
        {
            var message = new Message(_ses.MessageOutputStream.GetTemplateRegistry().GetTemplate("Arbitry"));
            message.SetInteger(1, _rnd.Next()%1000);
            message.SetInteger(2, _rnd.Next()%1000);
            message.SetInteger(3, _rnd.Next()%1000);
            message.SetInteger(4, _rnd.Next()%1000);
            message.SetInteger(6, _rnd.Next()%1000);
            _ses.MessageOutputStream.WriteMessage(message);
        }

        #region Nested type: ClientErrorHandler

        private class ClientErrorHandler : IErrorHandler
        {
            #region IErrorHandler Members

            public void Error(ErrorCode code, string message)
            {
                Console.WriteLine(message);
            }

            public void Error(ErrorCode code, string message, Exception t)
            {
                Console.WriteLine(message);
            }

            #endregion
        }

        #endregion
    }
}