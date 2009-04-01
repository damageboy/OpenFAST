using System;
using OpenFAST;
using OpenFAST.Session;

namespace TCPServer
{
    public class FASTMessageListener : MessageListener
    {
        //When the client sends a message this will print that on the console.
        public void OnMessage(Session session, Message message)
        {
            //Console.WriteLine("RCV TID:" + message.Template.Name);//UNCOMMENT
        }
    }
}
