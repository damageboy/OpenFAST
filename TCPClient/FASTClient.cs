using OpenFAST;
using OpenFAST.Session;
using OpenFAST.Session.Tcp;
using System;
using OpenFAST.Error;

namespace TCPClient
{
    public class FASTClient
    {
        public class ClientErrorHandler : ErrorHandler
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

        private readonly FastClient fc;
        private Session ses;

        public FASTClient(string Host, int Port)
        {
            fc = new FastClient("client", SessionConstants.SCP_1_1, new TcpEndpoint(Host, Port));
        }

        public void Connect()
        {
            ses = fc.Connect();
            ses.ErrorHandler = new ClientErrorHandler();
            ses.MessageHandler = new FASTClientMessageHandler();
        }

        private readonly Random rnd = new Random();
        public void SendMessage(string symbol)
        {
            var message = new Message(ses.MessageOutputStream.GetTemplateRegistry().get_Renamed("Arbitry"));
            message.SetInteger(1, rnd.Next()%1000);
            message.SetInteger(2, rnd.Next() % 1000);
            message.SetInteger(3, rnd.Next() % 1000);
            message.SetInteger(4, rnd.Next() % 1000);
            message.SetInteger(6, rnd.Next() % 1000);
            ses.MessageOutputStream.WriteMessage(message);
        }
    }
}