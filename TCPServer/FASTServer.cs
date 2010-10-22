using System;
using OpenFAST;
using OpenFAST.Error;
using OpenFAST.Session;
using OpenFAST.Session.Tcp;

namespace TCPServer
{
    public class FASTServer
    {
        public ISessionProtocol ScpSessionProtocol = SessionConstants.Scp11;
        public FASTSessionHandler SessionHandler;

        public FASTServer()
        {
            var endpoint = new TcpEndpoint(16121);

            SessionHandler = new FASTSessionHandler();

            var fs = new FastServer("test", ScpSessionProtocol, endpoint);

            Global.ErrorHandler = new ServerErrorHandler();
            fs.SessionHandler = SessionHandler;

            fs.Listen();
        }

        #region Nested type: ServerErrorHandler

        public class ServerErrorHandler : IErrorHandler
        {
            #region ErrorHandler Members

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