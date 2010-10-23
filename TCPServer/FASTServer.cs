using System;
using OpenFAST;
using OpenFAST.Error;
using OpenFAST.Session;
using OpenFAST.Session.Tcp;

namespace TCPServer
{
    public class FASTServer
    {
        private readonly ISessionProtocol _scpSessionProtocol = SessionConstants.Scp11;
        private readonly FASTSessionHandler _sessionHandler;

        public FASTServer()
        {
            var endpoint = new TcpEndpoint(16121);

            _sessionHandler = new FASTSessionHandler();

            var fs = new FastServer("test", _scpSessionProtocol, endpoint);

            Global.ErrorHandler = new ServerErrorHandler();
            fs.SessionHandler = _sessionHandler;

            fs.Listen();
        }

        #region Nested type: ServerErrorHandler

        public class ServerErrorHandler : IErrorHandler
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