using System;
using OpenFAST.Error;
using OpenFAST.Sessions;
using OpenFAST.Sessions.Tcp;

namespace OpenFAST.TCPServer
{
    public class FastServer
    {
        private readonly ISessionProtocol _scpSessionProtocol = SessionConstants.Scp11;
        private readonly SessionHandler _sessionHandler;

        public FastServer()
        {
            var endpoint = new TcpEndpoint(16121);

            _sessionHandler = new SessionHandler();

            var fs = new Sessions.FastServer("test", _scpSessionProtocol, endpoint);

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