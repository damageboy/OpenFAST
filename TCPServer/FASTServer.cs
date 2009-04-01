using System;
using OpenFAST.Session;
using OpenFAST.Error;
using OpenFAST;

namespace TCPServer
{
    public class FASTServer
    {
        public class ServerErrorHandler:ErrorHandler
        {
            public void Error(ErrorCode code, string message)
            {
                Console.WriteLine(message);
                
            }

            public void Error(ErrorCode code, string message, Exception t)
            {
                Console.WriteLine(message);
            }
        }
        public FASTSessionHandler sessionHandler;

        public SessionProtocol SCPSessionProtocol = SessionConstants.SCP_1_1;
        public FASTServer()
        {
            var endpoint = new OpenFAST.Session.Tcp.TcpEndpoint(16121);

            sessionHandler = new FASTSessionHandler();

            var fs = new FastServer("test", SCPSessionProtocol, endpoint);
            
            Global.ErrorHandler = new ServerErrorHandler();
            fs.SessionHandler = sessionHandler;
            
            fs.Listen();
        }
    }
}
