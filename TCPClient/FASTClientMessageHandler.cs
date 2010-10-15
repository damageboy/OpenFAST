using OpenFAST;
using OpenFAST.Session;

namespace TCPClient
{
    public class FASTClientMessageHandler : IMessageListener
    {
        //When the server sends a message this will print that on the console.

        #region MessageListener Members

        public void OnMessage(Session session, Message message)
        {
            //Console.WriteLine("RCV TID:" + message.Template.Name);//UNCOMMENT
        }

        #endregion
    }
}