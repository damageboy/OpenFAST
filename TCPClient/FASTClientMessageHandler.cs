using System;
using OpenFAST.Session;
using OpenFAST;
namespace TCPClient
{
    public class FASTClientMessageHandler : MessageListener
    {
        //When the server sends a message this will print that on the console.
        public void OnMessage(Session session, Message message)
        {
            //Console.WriteLine("RCV TID:" + message.Template.Name);//UNCOMMENT
        }
    }
}
