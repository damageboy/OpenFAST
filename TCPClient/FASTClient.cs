using System;
using OpenFAST.Error;
using OpenFAST.Sessions;
using OpenFAST.Sessions.Tcp;

namespace OpenFAST.TCPClient
{
    public class FastClient
    {
        private readonly Sessions.FastClient _fc;
        private readonly Random _rnd = new Random();
        private Sessions.Session _ses;

        public FastClient(string host, int port)
        {
            _fc = new Sessions.FastClient("client", SessionConstants.Scp11, new TcpEndpoint(host, port));
        }

        public void Connect()
        {
            _ses = _fc.Connect();
            _ses.ErrorHandler = new ClientErrorHandler();
            _ses.MessageHandler = new ClientMessageHandler();
        }

        public void SendMessage(string symbol)
        {
            var message = new Message(_ses.MessageOutputStream.TemplateRegistry["Arbitry"]);
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
            public void OnError(Exception exception, StaticError error, string format, params object[] args)
            {
                Console.WriteLine(format, args);
            }

            public void OnError(Exception exception, DynError error, string format, params object[] args)
            {
                Console.WriteLine(format, args);
            }

            public void OnError(Exception exception, RepError error, string format, params object[] args)
            {
                Console.WriteLine(format, args);
            }
        }

        #endregion
    }
}