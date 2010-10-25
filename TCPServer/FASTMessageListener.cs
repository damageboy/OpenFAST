using OpenFAST.Sessions;

namespace OpenFAST.TCPServer
{
    public class FASTMessageListener : IMessageListener
    {
        //When the client sends a message this will print that on the console.

        #region MessageListener Members

        public void OnMessage(Session session, Message message)
        {
            //Console.WriteLine("RCV TID:" + message.Template.Name);//UNCOMMENT
        }

        #endregion
    }
}